using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoverable
{
    void OnHoverEnter();
    void OnHoverExit();
    void OnClick();
}

public class Hoverable : MonoBehaviour, IHoverable
{
    public GameObject Outlineable;
    private Outline _outline;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        _outline = Outlineable.GetComponent<Outline>();
        if (!_outline)
            _outline = Outlineable.AddComponent<Outline>();
    }

    public void OnHoverEnter()
    {
        if (Outlineable && _outline)
        {
            _outline.enabled = true;
        }
    }

    public void OnHoverExit()
    {
        if (Outlineable && _outline)
        {
            _outline.enabled = false;
        }
    }

    public void OnClick()
    {
        Debug.Log("Pressed on a hoverable");
    }
}
