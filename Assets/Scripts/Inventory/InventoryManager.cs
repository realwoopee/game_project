using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private PlayerInventoryController _playerInventoryController;
    [SerializeField] private CraftingInventoryController _craftingInventoryController;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetPlayerInventoryState(InventoryState inventoryState)
    {
        _playerInventoryController.playerInventoryState = inventoryState;
    }
    
    public void SetPlayerInventoryVisible(bool state)
    {
        _playerInventoryController.SetVisible(state);
    }

    public void SetCraftingVisible(bool state)
    {
        _craftingInventoryController.SetVisible(state);
    }
}
