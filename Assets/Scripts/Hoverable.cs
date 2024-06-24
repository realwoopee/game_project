using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public interface IHoverable
{
    void OnHoverEnter();
    void OnHoverExit();
    void OnClick();
}

public class Hoverable : MonoBehaviour, IHoverable
{
    public GameObject outlineable;
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
        gameObject.layer = LayerMask.NameToLayer("Hoverable");
        _outline = outlineable.GetComponent<Outline>();
        if (!_outline)
        {
            _outline = outlineable.AddComponent<Outline>();
            _outline.enabled = false;
        }
    }

    public void OnHoverEnter()
    {
        if (outlineable && _outline)
        {
            _outline.enabled = true;
        }
    }

    public void OnHoverExit()
    {
        if (outlineable && _outline)
        {
            _outline.enabled = false;
        }
    }

    public void OnClick()
    {
        Debug.Log("Pressed on a hoverable");
    }
}
