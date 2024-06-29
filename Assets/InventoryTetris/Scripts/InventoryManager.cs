using System.Collections.Generic;
using UnityEngine;

public class InventoryManagerOld : MonoBehaviour {

    [SerializeField] private Transform _outerInventoryTetrisBackground;
    [SerializeField] private InventoryTetris _inventoryTetris;
    [SerializeField] private InventoryTetris _outerInventoryTetris;
    [SerializeField] private Transform _inventoryTetrisBackground;
    [SerializeField] private List<string> addItemTetrisSaveList;
    private bool _isInnerOpened;
    private bool _isOuterOpened;

    private int addItemTetrisSaveListIndex;

    private void Start() {
        _isInnerOpened = false;
        _outerInventoryTetrisBackground.gameObject.SetActive(false);
        _inventoryTetrisBackground.gameObject.SetActive(false);
    }

    private void Update() {
        // if (Input.GetKeyDown(KeyCode.Tab))
        // {
        //     if (IsInnerOpened)
        //         CloseInner();
        //     else
        //         OpenInner();
        //         
        //     addItemTetrisSaveListIndex = (addItemTetrisSaveListIndex + 1) % addItemTetrisSaveList.Count;
        // }

        if (Input.GetKeyDown(KeyCode.O)) {
            _outerInventoryTetrisBackground.gameObject.SetActive(!_outerInventoryTetrisBackground.gameObject.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.J)) {
            _outerInventoryTetris.Load(addItemTetrisSaveList[addItemTetrisSaveListIndex]);
            addItemTetrisSaveListIndex = (addItemTetrisSaveListIndex + 1) % addItemTetrisSaveList.Count;
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log(_inventoryTetris.Save());
        }
    }

    public bool IsInnerOpened
    {
        get => _isInnerOpened;
        set
        {
            if(!_isInnerOpened && value)
                OpenInner();
            else if (_isInnerOpened && !value)
                CloseInner();
            
            addItemTetrisSaveListIndex = (addItemTetrisSaveListIndex + 1) % addItemTetrisSaveList.Count;
        }
    }

    public bool IsOuterOpened { get => _isOuterOpened; set => _isOuterOpened = value; }
    public void CloseInner()
    {
        _isInnerOpened = false;
        _inventoryTetrisBackground.gameObject.SetActive(false);
        _inventoryTetris.Save();
    }
    public void OpenInner()
    {
        _isInnerOpened = true;
        _inventoryTetrisBackground.gameObject.SetActive(true);
    }
    public void CloseOuter()
    {
        IsOuterOpened = false;
        _outerInventoryTetrisBackground.gameObject.SetActive(false);
        _outerInventoryTetris.Save();
    }
    public void OpenOuter()
    {
        IsOuterOpened = true;
        _outerInventoryTetrisBackground.gameObject.SetActive(true);
    }
}
