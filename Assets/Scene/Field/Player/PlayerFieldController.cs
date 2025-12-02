using System;
using System.Collections.Generic;
using System.Linq;
using PixelCollector.Networking.Client;
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
    private Dictionary<string, Action<InputAction.CallbackContext>> actionHandlers;
    
    public PlayerBaseModule PlayerObject
    {
      get
      {
        if (!playerObject)
          playerObject = NetClientManager.LocalPlayer;

        return playerObject;
      }
      
      set => playerObject = value;
    }
    
    #region Unity Callback
    
    private void Awake()
    {
      RegisterHandlers();

      foreach (var action in actions.actionMaps.SelectMany(map => map.actions))
      {
        if (!actionHandlers.TryGetValue(action.name, out var handler)) continue;
        
        action.performed += handler;
        action.canceled += handler;
      }
    }

    private void OnDestroy()
    {
      foreach (var action in actions.actionMaps.SelectMany(map => map.actions))
      {
        if (!actionHandlers.TryGetValue(action.name, out var handler)) continue;
        
        action.performed -= handler;
        action.canceled -= handler;
      }
    }
    
    #endregion
    
    #region Handler

    private void RegisterHandlers()
    {
      actionHandlers = new Dictionary<string, Action<InputAction.CallbackContext>>
      {
        {"Player/Move", OnMove}
      };
    }
    
    private void OnMove(InputAction.CallbackContext ctx)
    {
      PlayerObject.MoveCommand(new MovePacket(ctx));
    }
    
    #endregion
  }
}