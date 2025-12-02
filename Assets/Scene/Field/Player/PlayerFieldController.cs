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

      foreach (var map in actions.actionMaps) map.Enable();
      
      foreach (var (key, handler) in actionHandlers)
      {
        var action = actions.FindAction(key);
        if (action == null) continue;
        
        actions.FindAction(key).performed += handler;
        actions.FindAction(key).canceled += handler;
      }
    }

    private void OnDestroy()
    {
      foreach (var (key, handler) in actionHandlers)
      {
        actions.FindAction(key).performed -= handler;
        actions.FindAction(key).canceled -= handler;
      }
    }
    
    #endregion
    
    #region Handler

    private void RegisterHandlers()
    {
      actionHandlers = new Dictionary<string, Action<InputAction.CallbackContext>>
      {
        {"Player/Move", OnMove},
        {"UI/Click", OnLeftClick}
      };
    }
    
    private void OnMove(InputAction.CallbackContext ctx)
    {
      PlayerObject.MoveCommand(new MovePacket(ctx));
    }
    
    private void OnLeftClick(InputAction.CallbackContext ctx)
    {
      if (ctx.ReadValue<float>() == 1)
      {
        PlayerObject.ShootCommand(Camera.main != null
          ? Camera.main.ScreenToWorldPoint(Input.mousePosition)
          : Vector3.zero);
      }
    }
    
    #endregion
  }
}