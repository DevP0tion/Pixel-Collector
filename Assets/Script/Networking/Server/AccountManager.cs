using System.Collections;
using System.Collections.Generic;
using Mirror;
using PixelCollector.Networking.Packet;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace PixelCollector.Networking.Server
{
  [AddComponentMenu("Network/Authenticators/Account Manager")]
  public class AccountManager : NetworkAuthenticator
  { 
    private readonly HashSet<NetworkConnectionToClient> connectionsPendingDisconnect = new ();
    [SerializeField] private SerializableDictionaryBase<string, string> accounts = new();

    /// <summary>
    /// 서버에서 StartServer로부터 호출되어 Authenticator를 초기화합니다.
    /// <para>서버 메시지 핸들러는 이 메서드에서 등록해야 합니다.</para>
    /// </summary>
    public override void OnStartServer()
    { 
      // 클라이언트로부터 올 것으로 예상되는 인증 요청에 대한 핸들러를 등록합니다
      NetworkServer.RegisterHandler<AuthPacket>(OnAuthRequestMessage, false);
    }

    /// <summary>
    /// 서버에서 StopServer로부터 호출되어 Authenticator를 재설정합니다.
    /// <para>서버 메시지 핸들러는 이 메서드에서 등록 해제해야 합니다.</para>
    /// </summary>
    public override void OnStopServer()
    {
      // 인증 요청 핸들러 등록을 해제합니다
      NetworkServer.UnregisterHandler<AuthPacket>();
    }

    /// <summary>
    /// 클라이언트가 인증해야 할 때 OnServerConnectInternal에서 서버 쪽으로 호출됩니다.
    /// </summary>
    /// <param name="conn">클라이언트 연결입니다.</param>
    public override void OnServerAuthenticate(NetworkConnectionToClient conn)
    {
      // 아무 작업도 하지 않습니다... 클라이언트의 인증 요청 메시지를 기다립니다
    }

    /// <summary>
    /// 클라이언트의 AuthRequestMessage가 도착했을 때 서버에서 호출됩니다.
    /// </summary>
    /// <param name="conn">클라이언트 연결입니다.</param>
    /// <param name="msg">메시지 페이로드입니다.</param>
    public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthPacket msg)
    {
      //Debug.Log($"인증 요청: {msg.authUsername} {msg.authPassword}");

      if (connectionsPendingDisconnect.Contains(conn)) return;

      // 인증 자격을 확인합니다(웹 서버, 데이터베이스, PlayFab API 등 적절한 방법 사용).
      if (accounts.TryGetValue(msg.username, out var password) && password == msg.password)
      {
        // 클라이언트에 성공 메시지를 생성하여 전송(다음 단계로 진행하도록 알림)
        var authResponseMessage = new AuthResponseMessage
        {
          code = 100,
          message = "Success"
        };

        conn.Send(authResponseMessage);

        // 인증 성공 처리
        ServerAccept(conn);
      }
      else
      {
        connectionsPendingDisconnect.Add(conn);

        // 클라이언트에 실패 메시지를 생성하여 전송(연결 해제를 알림)
        AuthResponseMessage authResponseMessage = new AuthResponseMessage
        {
          code = 200,
          message = "Invalid Credentials"
        };

        conn.Send(authResponseMessage);

        // NetworkConnection의 isAuthenticated를 false로 설정해야 합니다
        conn.isAuthenticated = false;

        // 응답 메시지가 전달되도록 1초 후에 클라이언트를 연결 해제합니다
        StartCoroutine(DelayedDisconnect(conn, 1f));
      }
    }

    IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
    {
      yield return new WaitForSeconds(waitTime);

      // 인증 실패 처리(연결 거부)
      ServerReject(conn);

      yield return null;

      // 보류 중인 연결 목록에서 제거
      connectionsPendingDisconnect.Remove(conn);
    }
  }
}