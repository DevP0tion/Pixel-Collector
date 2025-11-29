using System.Collections.Generic;

public class IDPool
{
  private int nextID = 0;

  // 현재 사용 중인 ID 목록
  private readonly HashSet<int> allocatedIDs = new ();

  // 반환되어 재사용 가능한 ID 목록
  private readonly HashSet<int> recycledIDs = new ();

  /// <summary>
  /// 새로운 ID를 발급합니다. 재사용 가능한 ID가 있으면 그것을 반환하고, 없으면 새로 생성합니다.
  /// </summary>
  public int Get()
  {
    int id;

    if (recycledIDs.Count > 0)
    {
      using (var enumerator = recycledIDs.GetEnumerator())
      {
        enumerator.MoveNext();
        id = enumerator.Current;
      }
      recycledIDs.Remove(id);
    }
    else
    {
      id = nextID++;
    }

    allocatedIDs.Add(id);
    return id;
  }

  /// <summary>
  /// 지정된 ID 목록을 강제로 할당 상태로 등록합니다.
  /// 이미 사용 중이거나 재활용 가능한 ID는 중복 없이 관리되며,
  /// 필요한 경우 nextID 값을 갱신하여 자동 할당과 충돌하지 않도록 합니다.
  /// </summary>
  /// <param name="idList">
  /// 할당할 ID 정수 목록입니다. 중복된 값은 무시되며,
  /// 이미 할당된 ID는 다시 등록되지 않습니다.
  /// </param>
  /// <remarks>
  /// 주로 저장된 ID 상태 복구, 외부 시스템과의 동기화,
  /// 수동 예약 등의 목적에 사용됩니다.
  /// </remarks>
  public void Allocate(params int[] idList)
  {
    foreach (var id in idList)
    {
      if (allocatedIDs.Contains(id))
        continue; // 이미 할당됨

      recycledIDs.Remove(id); // 재사용 풀에 있으면 제거
      allocatedIDs.Add(id);   // 새로 할당

      if (id >= nextID)
        nextID = id + 1; // nextID를 항상 가장 큰 ID보다 크게 유지
    }
  }

  
  /// <summary>
  /// ID를 반환합니다. 중복 반환은 무시됩니다.
  /// </summary>
  public void Release(params int[] idList)
  {
    foreach (var id in idList)
    {
      if (!allocatedIDs.Contains(id)) continue;
      
      allocatedIDs.Remove(id);
      recycledIDs.Add(id);
    }
  }

  /// <summary>
  /// 현재 사용 중인 ID 수
  /// </summary>
  public int AllocatedCount => allocatedIDs.Count;

  /// <summary>
  /// 반환되어 재사용 가능한 ID 수
  /// </summary>
  public int RecycledCount => recycledIDs.Count;

  /// <summary>
  /// 총 발급된 ID 수 (중복 포함)
  /// </summary>
  public int TotalCount => nextID;
}