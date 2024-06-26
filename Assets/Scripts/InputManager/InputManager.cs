using System;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputManager : MonoBehaviour
{
    public void Start()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindActionMap("Player").Enable(); 
        playerInput.actions.FindActionMap("Vehicle").Disable();
        playerInput.actions.FindActionMap("Inventory").Enable();
    }

    public void LateUpdate()
    {
        shootPressed = false;
    }

    public bool cursorHidden;
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            SetCursorHiddenState(cursorHidden);
    }

    private void SetCursorHiddenState(bool isHidden) => Cursor.visible = !isHidden;
}