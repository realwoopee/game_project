using System;
using UnityEngine;

public partial class InputManager
{
    public Vector2 move;
    public void MoveInput(Vector2 newMoveDirection) => move = newMoveDirection;
    public Vector2 look;
    public void LookInput(Vector2 newLookDirection) => look = newLookDirection;
    public bool sprint;
    public void SprintInput(bool newSprintState) => sprint = newSprintState;
    public event Action OnInteractPressed;
    public event Action OnInteractReleased;
    public bool interactHeld;
    private bool _lastInteractState = false;

    public void InteractInput(bool newValue)
    {
        switch ((_lastInteractState, newValue))
        {
            case (false, true):
                interactHeld = true;
                OnInteractPressed?.Invoke();
                break;
            case (true, false):
                interactHeld = false;
                OnInteractReleased?.Invoke();
                break;
            default:
                break;
        }

        _lastInteractState = newValue;
    }

    public event Action OnAdsPressed;
    public event Action OnAdsReleased;
    public bool adsHeld;
    private bool _lastAdsState = false;

    public void AdsInput(bool newValue)
    {
        switch ((_lastAdsState, newValue))
        {
            case (false, true):
                adsHeld = true;
                OnAdsPressed?.Invoke();
                break;
            case (true, false):
                adsHeld = false;
                OnAdsReleased?.Invoke();
                break;
            default:
                break;
        }

        _lastAdsState = newValue;
    }

    public event Action OnShootPressed;
    public event Action OnShootReleased;
    public bool shootHeld;
    public bool shootPressed;
    private bool _lastShootState = false;

    public void ShootInput(bool newValue)
    {
        switch ((_lastShootState, newValue))
        {
            case (false, true):
                shootPressed = true;
                shootHeld = true;
                OnShootPressed?.Invoke();
                break;
            case (true, false):
                shootHeld = false;
                OnShootReleased?.Invoke();
                break;
            default:
                break;
        }

        _lastShootState = newValue;
    }

    public event Action OnReloadPressed;
    public event Action OnReloadReleased;
    public bool reloadHeld;
    private bool _lastReloadState = false;

    public void ReloadInput(bool newValue)
    {
        switch ((_lastReloadState, newValue))
        {
            case (false, true):
                reloadHeld = true;
                OnReloadPressed?.Invoke();
                break;
            case (true, false):
                reloadHeld = false;
                OnReloadReleased?.Invoke();
                break;
            default:
                break;
        }

        _lastReloadState = newValue;
    }
    
    public event Action OnHealPressed;
    public event Action OnHealReleased;
    public bool healHeld;
    private bool _lastHealState = false;

    public void HealInput(bool newValue)
    {
        switch ((_lastHealState, newValue))
        {
            case (false, true):
                healHeld = true;
                OnHealPressed?.Invoke();
                break;
            case (true, false):
                healHeld = false;
                OnHealReleased?.Invoke();
                break;
            default:
                break;
        }

        _lastHealState = newValue;
    }
}