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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHoverEnter()
    {
        if (Outlineable)
        {
            var outline = Outlineable.GetComponent<Outline>();
            if (!outline)
                outline = Outlineable.AddComponent<Outline>();
            outline.enabled = true;
        }
    }

    public void OnHoverExit()
    {
        if (Outlineable)
        {
            var outline = Outlineable.GetComponent<Outline>();
            if (!outline)
                outline = Outlineable.AddComponent<Outline>();
            outline.enabled = false;
        }
    }

    public void OnClick()
    {
        Debug.Log("Pressed on a hoverable");
    }
}
