using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
public class RMMRDA : MonoBehaviour
{

    public Action<bool> OnTrackingObjectChanged;

    #region singleton
    public static RMMRDA instance;
    #endregion
    #region public varible
    public GameObject currentSessionLine;
    #endregion
    #region Unity Function
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        GameManager.instance.currentObject.transform.SetParent(currentSessionLine.transform);
        GameManager.instance.currentObject.SetActive(true);
    }

    public void UpdateOffset(float offsetX, float offsetY, float offsetZ, float offsetRotation)
    {
        if (!currentSessionLine.activeInHierarchy) return;
        currentSessionLine.transform.Translate(new Vector3(offsetX, offsetY, offsetZ));
        Vector3 currentRotation = currentSessionLine.transform.rotation.eulerAngles + new Vector3(0, offsetRotation, 0); ;
        // Gán giá trị rotation mới
        currentSessionLine.transform.rotation = Quaternion.Euler(currentRotation);
        //currentSessionLine.GetComponentInChildren<TextMesh>().text = "" + currentSessionLine.transform.localEulerAngles + "  / " + currentSessionLine.transform.rotation + "  / " + currentSessionLine.transform.position;
    }
    public void ResetObject()
    {
        if (currentSessionLine.activeInHierarchy)
        {
            currentSessionLine.transform.position = lastPosition;
            currentSessionLine.transform.rotation = lastRotation;
        }
    }

    Vector3 lastPosition = Vector3.zero;
    Quaternion lastRotation = Quaternion.identity;
    public void SetPositionAndRotationLast(Vector3 _position, Quaternion _rotation)
    {
        lastPosition = _position;
        lastRotation = _rotation;
    }    
    public void MRLineSetPositionAndRotation(Vector3 _position, Quaternion _rotation)
    {
        if (currentSessionLine != null)
        {
            if (!currentSessionLine.activeInHierarchy)
            {
                currentSessionLine.SetActive(true);
            }
            currentSessionLine.transform.position = _position;
            lastPosition = _position;
            currentSessionLine.transform.rotation = _rotation;
            lastRotation = _rotation;
        }
    }

    public void MRLineSetPositionAndRotation(Vector3 _position)
    {
        if (currentSessionLine != null)
        {
            if (!currentSessionLine.activeInHierarchy)
            {
                currentSessionLine.SetActive(true);
            }

            currentSessionLine.transform.position = _position;
            lastPosition = _position;

        }
    }
    public void MRLineSetPositionAndRotation(Quaternion _rotation)
    {
        if (currentSessionLine != null)
        {
            if (!currentSessionLine.activeInHierarchy)
            {
                currentSessionLine.SetActive(true);
            }
            currentSessionLine.transform.rotation = _rotation;

        }
    }
    #endregion
}
