using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.InteractionSubsystems;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.Management;
public class TrackedPosition : MonoBehaviour
{
    private Camera _camera = null;

    private MagicLeapInputs mlInputs;
    private MagicLeapInputs.ControllerActions controllerActions;
    private XRRayInteractor xRRayInteractor;
    private XRInputSubsystem inputSubsystem;
    private readonly MLPermissions.Callbacks permissionCallbacks = new MLPermissions.Callbacks();
    public Transform RayOrigin;
    //public GameObject tracked;
    Ray r;
    RaycastHit hit;

    private void Start()
    {
        MLPermissions.RequestPermission(MLPermission.SpatialMapping, permissionCallbacks);
        inputSubsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRInputSubsystem>();
        inputSubsystem.trackingOriginUpdated += OnTrackingOriginChanged;

    }

    private void OnTrackingOriginChanged(XRInputSubsystem obj)
    {
        throw new NotImplementedException();
    }

    void Awake()
    {
        permissionCallbacks.OnPermissionGranted += OnPermissionGranted;
        permissionCallbacks.OnPermissionDenied += OnPermissionDenied;
        permissionCallbacks.OnPermissionDeniedAndDontAskAgain += OnPermissionDenied;


        MLDevice.RegisterGestureSubsystem();
        if (MLDevice.GestureSubsystemComponent == null)
        {
            Debug.LogError("MLDevice.GestureSubsystemComponent is not set. Disabling script.");
            enabled = false;
            return;
        }

        xRRayInteractor = FindObjectOfType<XRRayInteractor>();


        _camera = Camera.main;

        mlInputs = new MagicLeapInputs();
        mlInputs.Enable();
        controllerActions = new MagicLeapInputs.ControllerActions(mlInputs);

        controllerActions.Trigger.performed += OnTriggerDown;
        controllerActions.Trigger.canceled += OnTriggerUp;
        controllerActions.Bumper.performed += OnBumperDown;
        controllerActions.Menu.performed += OnMenuDown;

        MLDevice.GestureSubsystemComponent.onTouchpadGestureChanged += OnTouchpadGestureStart;
    }

    private void OnPermissionDenied(string permission)
    {
    }

    private void OnPermissionGranted(string permission)
    {
    }

    private void OnTouchpadGestureStart(GestureSubsystem.Extensions.TouchpadGestureEvent obj)
    {
    }

    private void OnMenuDown(InputAction.CallbackContext obj)
    {
    }

    private void OnBumperDown(InputAction.CallbackContext obj)
    {
    }

    public LayerMask realease;
    public LayerMask holding;
    public LayerMask currentMask;
    public float maxDistance = 3;
    private void OnTriggerDown(InputAction.CallbackContext obj)
    {
        
        r.origin = RayOrigin.position;
        r.direction = RayOrigin.forward;
        if (Physics.Raycast(r, out hit, maxDistance, currentMask))
        {
            
            if (targetMoving == null && hit.transform.GetComponent<MoveObject>() != null )
            {
                targetMoving = hit.transform.gameObject;
                targetMoving.GetComponent<MoveObject>().isMoving = true;
                targetMoving.GetComponent<Collider>().enabled = false;
                currentMask = holding;
            }
            else
            {
                if (targetMoving != null)
                {
                    targetMoving.GetComponent<MoveObject>().SetPosition(hit.point);
                }
            }
        }
        else
        {
            if (targetMoving != null)
            {
                targetMoving.GetComponent<MoveObject>().SetPosition(RayOrigin.transform.position + RayOrigin.forward);
                targetMoving.GetComponent<MoveObject>().isMoving = true;
                targetMoving.GetComponent<Collider>().enabled = false;
            }
        }

        
    }
    private void OnTriggerUp(InputAction.CallbackContext obj)
    {
        if (targetMoving != null) 
        {
            targetMoving.GetComponent<MoveObject>().isMoving = false;
            targetMoving.GetComponent<Collider>().enabled = true;
            targetMoving = null;
        }
        currentMask = realease;
    }
    public GameObject targetMoving;

}
