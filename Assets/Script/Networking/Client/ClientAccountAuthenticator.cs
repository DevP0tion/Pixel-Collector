using Mirror;
using Mirror.Authenticators;
using PixelCollector.Networking.Packet;
using UnityEngine;

namespace PixelCollector.Networking.Client
{
  [AddComponentMenu("Network/Authenticators/Client Account Authenticator")]
  public class ClientAccountAuthenticator : NetworkAuthenticator
  {
    public string username;
    public string password;
    
    // client only
    
    /// <summary>
    /// Called on client from StartClient to initialize the Authenticator
    /// <para>Client message handlers should be registered in this method.</para>
    /// </summary>
    public override void OnStartClient()
    {
      // register a handler for the authentication response we expect from server
      NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
    }

    /// <summary>
    /// Called on client from StopClient to reset the Authenticator
    /// <para>Client message handlers should be unregistered in this method.</para>
    /// </summary>
    public override void OnStopClient()
    {
      // unregister the handler for the authentication response
      NetworkClient.UnregisterHandler<AuthResponseMessage>();
    }

    /// <summary>
    /// Called on client from OnClientConnectInternal when a client needs to authenticate
    /// </summary>
    public override void OnClientAuthenticate()
    {
      var authRequestMessage = new AuthPacket
      {
        username = username,
        password = password
      };

      NetworkClient.Send(authRequestMessage);
    }

    /// <summary>
    /// Called on client when the server's AuthResponseMessage arrives
    /// </summary>
    /// <param name="msg">The message payload</param>
    public void OnAuthResponseMessage(AuthResponseMessage msg)
    {
      if (msg.code == 100)
      {
        //Debug.Log($"Authentication Response: {msg.message}");

        // Authentication has been accepted
        ClientAccept();
      }
      else
      {
        Debug.LogError($"Authentication Response: {msg.message}");

        // Authentication has been rejected
        ClientReject();
      }
    }
  }
}