using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using System.Collections.Generic;

public class MarkerAlign : MonoBehaviour
{
    public static MarkerAlign Instance;
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RMMRDA.instance.OnTrackingObjectChanged += OnTrackingChanged;
    }
    private void OnDestroy()
    {
        RMMRDA.instance.OnTrackingObjectChanged -= OnTrackingChanged;
    }
    public Vector3 rootPosition;
    public Quaternion rootRotation;
    public bool trackingObject;
    public void ResetObject(GameObject MRObject)
    {
        rootPosition = MRObject.transform.position;
        rootRotation = MRObject.transform.rotation;
    }
    public void OnTrackingChanged(bool isTracking)
    {
        if (isTracking)
        {
            ResetObject(RMMRDA.instance.currentSessionLine);
        }
        trackingObject = isTracking;

    }
    public void UpdateMRObject(Plane currentPlane)
    {
        Debug.Log("trackingObject " + trackingObject);
        if (trackingObject)
        {
            float angleInDegree = Vector3.Angle(Vector3.ProjectOnPlane(currentPlane.normal, Vector3.up), currentPlane.normal) - 90;

            Debug.Log("raw : " + Vector3.Angle(Vector3.ProjectOnPlane(currentPlane.normal, Vector3.up), currentPlane.normal) + " Mathf.Abs(angleInDegree) : " + angleInDegree + " => " + Mathf.Abs(angleInDegree));
            if (Mathf.Abs(angleInDegree) < 45)
            {
                Quaternion planeRotation = Quaternion.LookRotation(currentPlane.normal);
                RMMRDA.instance.MRLineSetPositionAndRotation(/*Quaternion.Euler(planeRotation.eulerAngles.x, rootRotation.eulerAngles.y, planeRotation.eulerAngles.z)*/ planeRotation);
                float enter = 0.0f;
                Ray ray = new Ray(rootPosition, Vector3.ProjectOnPlane(currentPlane.normal, Vector3.up));
                Vector3 inPlanePoint = Vector3.zero;
                if (currentPlane.Raycast(ray, out enter))
                {
                    inPlanePoint = ray.GetPoint(enter);
                }
                else
                {
                    ray.direction = -ray.direction;
                    if (currentPlane.Raycast(ray, out enter))
                    {

                        inPlanePoint = ray.GetPoint(enter);
                    }
                }
                Vector3 directionPlane = Vector3.ProjectOnPlane(currentPlane.normal, Vector3.up);
                int direction = 0;
                if (isSameDirection(directionPlane.normalized, (inPlanePoint - rootPosition /*RMMRDA.instance.currentSessionLine.transform.position*/).normalized))
                {
                    direction = 1;
                }
                else
                {
                    direction = -1;
                }
                float yPosition = Mathf.Tan(direction * angleInDegree * Mathf.Deg2Rad) * Vector3.Distance(inPlanePoint, rootPosition /*anchorObject.transform.position*/) + rootPosition.y;
                RMMRDA.instance.MRLineSetPositionAndRotation(new Vector3(rootPosition.x, yPosition, rootPosition.z));
            }
            else
            {
                RMMRDA.instance.MRLineSetPositionAndRotation(new Vector3(rootPosition.x, currentPlane.ClosestPointOnPlane(rootPosition).y, rootPosition.z), rootRotation);
            }
        }
    }
    public bool isSameDirection(Vector3 a, Vector3 b)
    {
        return Vector3.Dot(a, b) < 0;
    } 
}

