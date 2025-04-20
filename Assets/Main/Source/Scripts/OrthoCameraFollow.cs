using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrthoCameraFollow : MonoBehaviour
{
    public Transform focus;
    public float smoothTime = 0.3f;
    public float stickDistance = 1.0f;
    public Vector3 posOffset = Vector3.zero;
    private float velocity = 0.0f;
    [SerializeField]
    private Vector3 followAreaSize = Vector3.zero;
    public Vector3 targetPosition = Vector3.zero;
    public Quaternion targetRotation = Quaternion.identity;

    public float currentAngle = 0.0f;
    public float targetAngle = 0.0f;

    private PlayerInputs playerInputActions;

    public bool IsCameraMoving {
        get {
            return Mathf.Abs(targetAngle - currentAngle) >= 0.01;
        }
    }

    void Awake()
    {
        playerInputActions = new PlayerInputs();
        playerInputActions.Player.RotateCameraLeft.performed += OnRotateCameraLeft;
        playerInputActions.Player.RotateCameraRight.performed += OnRotateCameraRight;
        
        playerInputActions.Enable();
    }

    void OnDisable()
    {
        playerInputActions.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        targetRotation = transform.rotation;
        targetPosition = transform.position;
        
        currentAngle = targetAngle = 45 * Mathf.Deg2Rad;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // targetPos = Camera.main.ScreenToWorldPoint(target.transform.position);
        // targetPos.y = transform.position.y;
        // targetPos += posOffset;
        // transform.position = Vector3.SmoothDamp(this.transform.position, targetPos, ref velocity, smoothTime);
    }

    void LateUpdate()
    {
        currentAngle = Mathf.SmoothDamp(currentAngle, targetAngle, ref velocity, smoothTime);

        targetPosition.x = stickDistance * Mathf.Cos(currentAngle) + focus.position.x;
        targetPosition.y = transform.position.y;
        targetPosition.z = stickDistance * Mathf.Sin(currentAngle) + focus.position.z;

        //transform.position = Vector3.Slerp(transform.position, targetPosition, step);
        transform.position = targetPosition;
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
        //step = step + Time.deltaTime;
        transform.LookAt(focus);
    }

    void SetTargetRotation(float angle)
    {
        // targetPosition.x = stickDistance * Mathf.Cos(angle) + focus.position.x;
        // targetPosition.z = stickDistance * Mathf.Sin(angle) + focus.position.z;

        // targetRotation = Quaternion.LookRotation(focus.position - targetPosition);
        targetAngle += (angle * Mathf.Deg2Rad);
    }

    void OnRotateCameraLeft(InputAction.CallbackContext context)
    {
        SetTargetRotation(-90.0f);
    }

    void OnRotateCameraRight(InputAction.CallbackContext context)
    {
        SetTargetRotation(90.0f);
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawCube(Camera.main.ScreenToWorldPoint(Camera.main.transform.position), followAreaSize);
    // }
}
