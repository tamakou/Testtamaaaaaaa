using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using System.Collections.Generic;

public class PlaneDetectionAlign : MonoBehaviour
{
    void Awake()
    {
        //planeManager = GetComponent<ARPlaneManager>();
        planeManager.planesChanged += PlaneChanged;
    }

    private void Start()
    {
        RMMRDA.instance.OnTrackingObjectChanged += OnTrackingChanged;
    }
    private void OnDestroy()
    {
        RMMRDA.instance.OnTrackingObjectChanged -= OnTrackingChanged;
    }

    #region properties

    bool isPlacement
    {
        get { return isEnable && RMMRDA.instance != null && RMMRDA.instance.currentSessionLine != null; }
    }

    #endregion


    #region private varibles

    bool isEnable = false;
    List<ARPlane> trackedPlanes = new List<ARPlane>();
    ARPlane currentTracked, lastTracked;
    [SerializeField]
    private ARPlaneManager planeManager;
    #endregion


    #region public method
    public void EnablePlacement()
    {
        isEnable = true;
        UpdateMRLinePosition();
    }
    public void DisablePlacement()
    {
        isEnable = false;
    }
    #endregion




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
    public Material trackedPlane;
    void UpdateMRLinePosition()
    {
        //if (!isPlacement) return;
        //Debug.Log("trackedPlanes.Count :" + trackedPlanes.Count);
        if (trackedPlanes.Count > 0)
        {
            float largestSize = trackedPlanes[0].size.magnitude;
            currentTracked = trackedPlanes[0];


            foreach (var i in trackedPlanes)
            {
                if (i.size.magnitude > largestSize)
                {
                    largestSize = i.size.magnitude;
                    currentTracked = i;

                }
            }
            currentTracked.GetComponent<Renderer>().material = trackedPlane;
            UpdateMRObject(currentTracked);
        }
    }
    public void UpdateMRObject(ARPlane currentARPlane)
    {
        Debug.Log("trackingObject " + trackingObject);
        if (trackingObject)
        {
            Plane currentPlane = currentARPlane.infinitePlane;
            
            float angleInDegree = Vector3.Angle(Vector3.ProjectOnPlane(currentPlane.normal, Vector3.up), currentPlane.normal) - 90;

            Debug.Log("raw : "+ Vector3.Angle(Vector3.ProjectOnPlane(currentPlane.normal, Vector3.up), currentPlane.normal) + " Mathf.Abs(angleInDegree) : " + angleInDegree + " => " + Mathf.Abs(angleInDegree));
            if (Mathf.Abs(angleInDegree) < 45)
            {
                Quaternion planeRotation = Quaternion.LookRotation(currentPlane.normal);



                RMMRDA.instance.MRLineSetPositionAndRotation(Quaternion.Euler(planeRotation.eulerAngles.x, rootRotation.eulerAngles.y, planeRotation.eulerAngles.z));
                float enter = 0.0f;
                Ray ray = new Ray(/*RMMRDA.instance.currentSessionLine.transform.position*/ rootPosition, Vector3.ProjectOnPlane(currentPlane.normal, Vector3.up));
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
                RMMRDA.instance.MRLineSetPositionAndRotation( new Vector3(rootPosition.x,currentARPlane.transform.position.y, rootPosition.z ), rootRotation);
            }
        }
        //else
        //{
        //    RMMRDA.instance.MRLineSetPositionAndRotation(rootPosition,rootRotation);
        //}
    }
    public bool isSameDirection(Vector3 a, Vector3 b)
    {
        return Vector3.Dot(a, b) < 0;
    }
    private void PlaneChanged(ARPlanesChangedEventArgs args)
    {
        //Debug.Log("PlaneChanged" );
        if (args.added != null)
        {
            foreach (var item in args.added)
            {

                if (Vector3.Angle(item.normal, Vector3.up) < 45)
                {
                    trackedPlanes.Add(item);
                    item.gameObject.layer = 30;
                }
            }
        }
        if (args.removed != null)
        {
            foreach (var item in args.removed)
            {
                if (trackedPlanes.Contains(item))
                {
                    trackedPlanes.Remove(item);
                }
            }
        }
        //Debug.Log("PlaneChanged " + trackedPlanes.Count);
        UpdateMRLinePosition();

    }
}
