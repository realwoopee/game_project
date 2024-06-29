using System;

public partial class InputManager
{
    public event Action OnInventoryOpenClosePressed;
    public event Action OnInventoryOpenCloseReleased;
    public bool inventoryOpenCloseHeld;
    private bool _lastInventoryOpenCloseState = false;

    public void InventoryOpenCloseInput(bool newValue)
    {
        switch ((_lastInventoryOpenCloseState, newValue))
        {
            case (false, true):
                inventoryOpenCloseHeld = true;
                OnInventoryOpenClosePressed?.Invoke();
                break;
            case (true, false):
                inventoryOpenCloseHeld = false;
                OnInventoryOpenCloseReleased?.Invoke();
                break;
            default:
                break;
        }

        _lastInventoryOpenCloseState = newValue;
    }
}