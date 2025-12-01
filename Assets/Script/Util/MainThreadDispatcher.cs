using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelCollector.Util.Singletons;
using UnityEngine;

namespace PixelCollector.Util
{
  /// <summary>
  ///   백그라운드 스레드에서 메인 스레드로 작업을 디스패치하는 유틸리티 클래스입니다.
  /// </summary>
  public class MainThreadDispatcher : DDOLSingleton<MainThreadDispatcher>
  {
    private static readonly Queue<Action> executionQueue = new Queue<Action>();
    private void Update()
    {
      lock (executionQueue)
      {
        while (executionQueue.Count > 0)
        {
          executionQueue.Dequeue().Invoke();
        }
      }
    }

    /// <summary>
    ///   메인 스레드에서 액션을 실행합니다.
    /// </summary>
    /// <param name="action">실행할 액션</param>
    public void Enqueue(Action action)
    {
      lock (executionQueue)
      {
        executionQueue.Enqueue(action);
      }
    }
    
    public static void StaticEnqueue(Action action)
    {
      Instance.Enqueue(action);
    }

    /// <summary>
    ///   메인 스레드에서 함수를 실행하고 결과를 반환합니다.
    /// </summary>
    /// <typeparam name="T">반환 타입</typeparam>
    /// <param name="func">실행할 함수</param>
    /// <returns>함수 실행 결과</returns>
    public Task<T> EnqueueAsync<T>(Func<T> func)
    {
      var tcs = new TaskCompletionSource<T>();
      
      Enqueue(() =>
      {
        try
        {
          var result = func();
          tcs.SetResult(result);
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      });

      return tcs.Task;
    }

    public static Task<T> StaticEnqueueAsync<T>(Func<T> func)
    {
      return Instance.EnqueueAsync(func);
    }

    /// <summary>
    ///   메인 스레드에서 액션을 실행하고 완료될 때까지 기다립니다.
    /// </summary>
    /// <param name="action">실행할 액션</param>
    public Task EnqueueAsync(Action action)
    {
      var tcs = new TaskCompletionSource<bool>();
      
      Enqueue(() =>
      {
        try
        {
          action();
          tcs.SetResult(true);
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      });

      return tcs.Task;
    }

    public static Task StaticEnqueueAsync(Action action)
    {
      return Instance.EnqueueAsync(action);
    }
  }
}

