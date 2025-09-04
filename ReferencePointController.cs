using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReferencePointController : MonoBehaviour
{
    private MagicLeapInputs mlInputs;
    private MagicLeapInputs.ControllerActions controllerActions;

    void Start()
    {
        mlInputs = new MagicLeapInputs();
        mlInputs.Enable();
        controllerActions = new MagicLeapInputs.ControllerActions(mlInputs);
        controllerActions.Bumper.performed += HandleOnBumper;
        controllerActions.Trigger.performed += HandleOnTrigger;
    }

    private void HandleOnBumper(InputAction.CallbackContext obj)
    {
        bool bumperDown = obj.ReadValueAsButton();
        Debug.Log("The Bumper is pressed down " + bumperDown);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Debug.LogError(hit.transform.gameObject.name +"?"+ hit.point +"?"+ hit.normal);
        }
    }
    private void HandleOnTrigger(InputAction.CallbackContext obj)
    {
        bool bumperDown = obj.ReadValueAsButton();
        Debug.Log("The trigger is pressed down " + bumperDown);
    }

    void OnDestroy()
    {
        mlInputs.Dispose();
    }
}
