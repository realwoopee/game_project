using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    private GameObject _cursor;

    [SerializeField]
    [ItemCanBeNull]
    [CanBeNull] 
    private new Camera camera;

    [SerializeField] private LayerMask groundMask;
    
    // Start is called before the first frame update
    void Start()
    {
        if(!camera)
            camera = Camera.main;
        _cursor = GameObject.FindGameObjectWithTag("Cursor");
        Cursor.visible = false;
    }

    [CanBeNull] private GameObject _lastHitObject;
    private Vector3 _lastMousePosition;
    
    // Update is called once per frame
    void Update()
    {
        // if (Input.mousePresent)
        //     Cursor.visible = false;

        _lastMousePosition = Input.mousePosition;
    }
    
    void FixedUpdate()
    {
        
        var hit = GetCursorHit(_lastMousePosition);
        if(hit is null) return;

        _lastHitObject = ProcessHoverable(hit.Value, _lastHitObject);

        _cursor.transform.position = hit.Value.point;
    }

    [CanBeNull] GameObject ProcessHoverable(RaycastHit currentHit, [CanBeNull] GameObject lastHitObject)
    {
        var currentHitObject = currentHit.collider.gameObject;
        if (lastHitObject && currentHitObject == lastHitObject) return lastHitObject;
        
        if(lastHitObject)
        {
            var lastHoverable = lastHitObject!.GetComponent<Hoverable>();
            if (lastHoverable)
            {
                lastHoverable.OnHoverExit();
            }
        }
            
        var newHoverable = currentHitObject.GetComponent<Hoverable>();
        if (!newHoverable) return null;
        newHoverable.OnHoverEnter();

        return currentHitObject;

    }

    private RaycastHit? GetCursorHit(Vector3 mousePosition)
    {
        var ray = camera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundMask))
        {
            return hit;
        }

        return null;
    }
}
