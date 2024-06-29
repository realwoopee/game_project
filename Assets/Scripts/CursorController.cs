using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CursorController : MonoBehaviour
{
    private GameObject _cursor;

    [SerializeField]
    [ItemCanBeNull]
    [CanBeNull] 
    private new Camera camera;

    [SerializeField] private LayerMask groundMask;
    
    [field: SerializeField]
    [CanBeNull] public GameObject Highlighted { get; private set; }

    [Range(0, 50)]
    public float maxHighlightRange;
    public Vector3 LastMousePosition { get => _cursor.transform.position; }
    
    // Start is called before the first frame update
    void Start()
    {
        if(!camera)
            camera = Camera.main;
        _cursor = GameObject.FindGameObjectWithTag("Cursor");
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Highlighted = ProcessHoverable(null, Highlighted);
    }

    private Vector3 _lastMousePosition;

    // Update is called once per frame
    void LateUpdate()
    {
        // if (Input.mousePresent)
        //     Cursor.visible = false;

        _lastMousePosition = Input.mousePosition;
        
        var hit = GetCursorHit(_lastMousePosition);
        if (hit is null)
            return;
        
        Highlighted = ProcessHoverable(hit.Value.transform.gameObject, Highlighted);

        _cursor.transform.position = hit.Value.point;
    }

    [CanBeNull] GameObject ProcessHoverable(GameObject currentHit, [CanBeNull] GameObject lastHitObject)
    {
        var currentHitObject = currentHit;
        if (lastHitObject && currentHitObject == lastHitObject) return lastHitObject;
        
        if(lastHitObject)
        {
            var lastHoverable = lastHitObject!.GetComponent<Hoverable>();
            if (lastHoverable)
            {
                lastHoverable.OnHoverExit();
            }
        }

        if (!currentHitObject) return null;
        
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

    [CanBeNull]
    private GameObject HoverableHit(Vector3 targetPosition, Vector3 origin)
    {
        var ray = new Ray(origin, targetPosition - origin);
        Debug.DrawRay(origin, targetPosition - origin);
        return Physics.Raycast(ray, out var hit, Mathf.Infinity, groundMask) ? hit.transform.gameObject : null;
    }
}
