using MagicLeap.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class MarkerController : MonoBehaviour
{
    public static MarkerController controller;
    private MagicLeapInputs mlInputs;
    private MagicLeapInputs.ControllerActions controllerActions;
    [SerializeField] private TMP_Text _calibrationText;
    [SerializeField] private GameObject _offsetUI;
    public bool isBumperDown = false;
    public Transform RayOrigin;
    Ray r;
    RaycastHit hit;
    float currentOffset = 0.005f;
    bool canRotateController = false;
    private void Awake()
    {
        controller = this;
    }
    void Start()
    {
        mlInputs = new MagicLeapInputs();
        mlInputs.Enable();
        controllerActions = new MagicLeapInputs.ControllerActions(mlInputs);
        controllerActions.Trigger.started += OnTriggerDown;
        controllerActions.Trigger.performed += OnTriggerUpdate;
        controllerActions.Trigger.canceled += OnTriggerUp;
        controllerActions.TouchpadPosition.performed += HandleOnTouchpad;
        controllerActions.Bumper.performed += OnBumperDown;
        _lockPlacementButton.onClick.AddListener(OnLockPlacementClicked);
        _followButton.onClick.AddListener(OnFollowClicked);
    }
    public LayerMask realease;
    public LayerMask holding;
    public LayerMask currentMask;
    public float maxDistance = 3;
    public bool isTargetRotate = false;
    private void OnTriggerDown(InputAction.CallbackContext obj)
    {

        r.origin = RayOrigin.position;
        r.direction = RayOrigin.forward;
        if (Physics.Raycast(r, out hit, maxDistance, currentMask))
        {
            if(hit.transform.gameObject==RMMRDA.instance.currentSessionLine && targetMoving==null)
            {
                targetMoving = hit.transform.gameObject;
                targetMoving.GetComponent<MoveObject>().isMoving = true;
                targetMoving.GetComponent<Collider>().enabled = false;
                currentMask = holding;
            }
            if(hit.transform.gameObject.CompareTag("Rotate"))
            {
                isTargetRotate = true;
            }
        }
    }
    private void OnTriggerUpdate(InputAction.CallbackContext obj)
    {
        if (targetMoving != null)
        {
            targetMoving.GetComponent<MoveObject>().SetPosition(RayOrigin.transform.position);
            targetMoving.GetComponent<MoveObject>().isMoving = true;
            targetMoving.GetComponent<Collider>().enabled = false;
        }
        if (isTargetRotate)
        {
            Vector3 objLookAt = new Vector3(RayOrigin.transform.position.x, RMMRDA.instance.currentSessionLine.transform.position.y, RayOrigin.transform.position.z);

            RMMRDA.instance.currentSessionLine.GetComponent<MoveObject>().SetLookat(objLookAt);
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
        if (isTargetRotate) isTargetRotate = false;
        currentMask = realease;
    }
    public GameObject targetMoving;

    private void HandleOnTouchpad(InputAction.CallbackContext obj)
    {
        Vector2 triggerValue = obj.ReadValue<Vector2>();
        Vector2 swipeDirection = triggerValue;

        // Determine the swipe direction based on the touchpadGesture.Direction
        float swipeAngle = Mathf.Atan2(swipeDirection.y, swipeDirection.x) * Mathf.Rad2Deg;

        // Define a threshold to consider a swipe
        float swipeThreshold = 0.5f;

        // Check swipe directions
        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
        {
            // Horizontal swipe
            if (swipeDirection.x > swipeThreshold)
            {
                if (canRotateController)
                    RMMRDA.instance.UpdateOffset(0, 0, 0, 10);
                else
                    RMMRDA.instance.UpdateOffset(0.1f, 0, 0, 0);
                // Right swipe
                Debug.Log("Right Swipe");
            }
            else if (swipeDirection.x < -swipeThreshold)
            {
                if (canRotateController)
                    RMMRDA.instance.UpdateOffset(0, 0, 0, -10);
                else
                            RMMRDA.instance.UpdateOffset(-0.1f, 0, 0, 0);
                // Left swipe
                Debug.Log("Left Swipe");
            }
        }
        else
        {
            if (canRotateController) return;
            // Vertical swipe
            if (swipeDirection.y > swipeThreshold)
            {
                
                RMMRDA.instance.UpdateOffset(0, 0, 0.1f, 0);
                // Up swipe
                Debug.Log("Up Swipe");
            }
            else if (swipeDirection.y < -swipeThreshold)
            {
                RMMRDA.instance.UpdateOffset(0, 0, -0.1f, 0);
                // Down swipe
                Debug.Log("Down Swipe");
            }
        }
    }
    void OnBumperDown(InputAction.CallbackContext obj)
    {
        canRotateController = !canRotateController;
    }    
    public void MarkerCaribationCompleted()
    {
        _instructionsPanel.gameObject.SetActive(true);
        _calibrationPanel.gameObject.SetActive(false);
        RMMRDA.instance.OnTrackingObjectChanged?.Invoke(true);
    }
    public void ObjectPlaced()
    {
        _offsetUI.SetActive(true);
    }
    public void AddOffset()
    {
        RMMRDA.instance.UpdateOffset(0, currentOffset, 0, 0);
    }
    public void ReduceOffset()
    {

        RMMRDA.instance.UpdateOffset(0, -currentOffset, 0, 0);
    }
    public void AddX()
    {
        RMMRDA.instance.UpdateOffset(currentOffset, 0, 0, 0);
    }
    public void ReduceX()
    {
        RMMRDA.instance.UpdateOffset(-currentOffset, 0, 0, 0);
    }
    public void AddZ()
    {
        RMMRDA.instance.UpdateOffset(0, 0, currentOffset, 0);
    }
    public void ReduceZ()
    {
        RMMRDA.instance.UpdateOffset(0, 0, -currentOffset, 0);
    }
    public void AddRotation()
    {
        RMMRDA.instance.UpdateOffset(0, 0, 0, currentOffset);
    }
    public void ReduceRotation()
    {
        RMMRDA.instance.UpdateOffset(0, 0, 0,-currentOffset);
    }
    public void ResetPosAndRo()
    {
        RMMRDA.instance.ResetObject();
    }
    public void ChangeOffsetOption(int value)
    {
        switch(value)
        {
            case 0: currentOffset = 0.005f; break;
            case 1: currentOffset = 0.01f; break;
            case 2: currentOffset = 0.015f; break;
            default: currentOffset = 0.005f; break;
        }    
       
    }
    private void HandleOnBumper(InputAction.CallbackContext obj)
    {
        bool bumperDown = obj.ReadValueAsButton();
        Debug.Log("The Bumper is pressed down " + bumperDown);
        isBumperDown = bumperDown;
    }
    void OnDestroy()
    {
        mlInputs.Dispose();
    }
    [SerializeField] private PlaceInFront _PlaceInFront;
    [SerializeField] private Button _lockPlacementButton;
    [SerializeField] private Button _followButton;
    [SerializeField] private Image _instructionsPanel;
    [SerializeField] private Image _calibrationPanel;
    private void OnFollowClicked()
    {
        _PlaceInFront.PlaceOnUpdate = true;
        _followButton.gameObject.SetActive(false);
        _lockPlacementButton.gameObject.SetActive(true);
    }

    private void OnLockPlacementClicked()
    {
        _PlaceInFront.PlaceOnUpdate = false;
        _followButton.gameObject.SetActive(true);
        _lockPlacementButton.gameObject.SetActive(false);
    }
}
