using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CursorControl : MonoBehaviour
{
    // Editor Fields
    [SerializeField]
    private float cursorSpeed = 1.0f;
    [SerializeField]
    private float cursorMagnetSmoothTime = 1.0f;

    // Player Input
    private PlayerInputs playerInputActions;
    private InputAction moveAction;

    // Component References
    private UnityEngine.UI.Image cursorImage;
    private OrthoCameraFollow orthoCameraFollow;
    private BoardMgr boardMgr;

    // Private
    private bool hasGamepadInput = false;
    private Vector3 cursorVelocity = Vector3.zero;
    private Vector2 moveInput = Vector2.zero;
    private GameObject currentHighlight;

    // private PlayerInputs playerInputs;

    void Awake()
    {
        // playerInputs = new PlayerInputs();
        // playerInputs.Player.Select.performed += OnSelect;
        // playerInputs.Enable();
    }

    // void OnDisable()
    // {
    //     playerInputs.Disable();
    // }

    // Start is called before the first frame update
    void Start()
    {
        playerInputActions = new PlayerInputs();
        playerInputActions.Player.MoveCursor.performed += OnMoveCursor;
        playerInputActions.Player.MoveCursor.canceled += OnMoveCursor;
        
        playerInputActions.Enable();

        // moveAction = playerInputActions.Player.MoveCursor;

        cursorImage = GetComponent<UnityEngine.UI.Image>();
        if(!Camera.main.gameObject.TryGetComponent<OrthoCameraFollow>(out orthoCameraFollow))
        {
            Debug.LogWarning("CursorControl could not find OrthoCameraFollow on camera object!");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 currentPos = transform.position;
        currentPos.x += moveInput.x * cursorSpeed * Time.deltaTime;
        currentPos.y += moveInput.y * cursorSpeed * Time.deltaTime;
        transform.position = currentPos;

        if(Mathf.Abs(moveInput.x) <= Mathf.Epsilon && Mathf.Abs(moveInput.y) <= Mathf.Epsilon)
        {
            hasGamepadInput = false;
        }
        else
        {
            hasGamepadInput = true;
        }

        // if(!hasGamepadInput && currentHighlight)
        // {
        //     cursorImage.enabled = false;
        // }
        // else
        // {
        //     cursorImage.enabled = true;
        // }
    }

    void Update()
    {
        // Hard lock cursor to highlight if camera is moving
        if(currentHighlight && orthoCameraFollow.IsCameraMoving)
        {
            Vector3 highlightObjPos = currentHighlight.transform.position;
            Vector3 highlightObjBounds = currentHighlight.GetComponent<BoxCollider>().bounds.extents;
            highlightObjPos.y += highlightObjBounds.y;

            Vector3 targetPos = Camera.main.WorldToScreenPoint(highlightObjPos);
            targetPos.z = 0.0f;

            transform.position = targetPos;

            return;
        }

        CheckCursorHighlight();

        // No movement input, move cursor towards highlight center
        if(currentHighlight && !hasGamepadInput)
        {
            Vector3 highlightObjPos = currentHighlight.transform.position;
            Vector3 highlightObjBounds = currentHighlight.GetComponent<BoxCollider>().bounds.extents;
            highlightObjPos.y += highlightObjBounds.y;

            Vector3 targetPos = Camera.main.WorldToScreenPoint(highlightObjPos);
            targetPos.z = 0.0f;

            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref cursorVelocity, cursorMagnetSmoothTime);

            // Quick and dirty hack to set visibility of cursor based on proximity to target position
            if(Vector3.Distance(transform.position, targetPos) >= 0.5)
            {
                cursorImage.enabled = true;
            }
            else
            {
                cursorImage.enabled = false;
            }
        }
        else
        {
            cursorImage.enabled = true;
        }
    }

    void OnMoveCursor(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void CheckCursorHighlight()
    {
        GameObject hit = CursorCast();
        GridSpace hitSpace;
        if(hit && hit.TryGetComponent<GridSpace>(out hitSpace))
        {
            BoardMgr.Instance.SetHighlight(hitSpace);
            currentHighlight = hit;
            // hitSpace.IsHighlighting = true;

            // // Move to New Highlight
            // if(currentHighlight && hit != currentHighlight)
            // {
            //     currentHighlight.GetComponent<GridSpace>().IsHighlighting = false;
            // }

            // currentHighlight = hit;
        } 
        else if(BoardMgr.Instance.CurrentHighlight)
        {
            // Reset highlight
            // currentHighlight.GetComponent<GridSpace>().IsHighlighting = false;
            // currentHighlight = null;
            BoardMgr.Instance.SetHighlight(null);
            currentHighlight = null;
        }
    }

    private GameObject CursorCast()
    {
        RaycastHit castResult;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(transform.position), out castResult, 100.0f))
        {
            return castResult.transform.gameObject;
        }

        return null;
    }

    // void OnDrawGizmos()
    // {
    //     if(currentHighlight)
    //     {
    //         Vector3 highlightObjPos = currentHighlight.transform.position;
    //         Vector3 highlightObjBounds = currentHighlight.GetComponent<MeshRenderer>().bounds.extents;
    //         highlightObjPos.y += highlightObjBounds.y;

    //         Gizmos.color = Color.red;
    //         Gizmos.DrawSphere(highlightObjPos, 0.1f);
    //     }
    // }
}
