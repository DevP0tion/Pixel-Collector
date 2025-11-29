using System;
using PixelCollector.Networking.Packet;
using PixelCollector.Unit.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelCollector.Scene.Field.Player
{
  public class PlayerFieldController : MonoBehaviour
  {
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private PlayerBaseModule playerObject;
    private InputAction move;
    
    #region Unity Callback
    
    private void Awake()
    {
      move = actions.FindAction("Player/Move");
      
      move.performed += OnMove;
      move.canceled += OnMove;
    }
    
    #endregion
    
    #region Handler

    private void OnMove(InputAction.CallbackContext ctx)
    {
      if (playerObject == null) return;
      playerObject.MoveCommand(new MovePacket(ctx));
    }
    
    #endregion
  }
}