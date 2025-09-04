using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(ARPlaneManager))]
public class AutoPlacementOfObjectsInPlane : MonoBehaviour
{
    void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
        planeManager.planesChanged += PlaneChanged;
    }


    #region properties

    bool isPlacement
    {
        get { return isEnable && RMMRDA.instance !=null && RMMRDA.instance.currentSessionLine != null;}
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


    void UpdateMRLinePosition()
    {
        if (!isPlacement) return;
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
            if (currentTracked.transform.position.y < Camera.main.transform.position.y && Vector3.Angle(currentTracked.normal, Vector3.up) < 45)
            {
                if (lastTracked == null)
                {
                    RMMRDA.instance.MRLineSetPositionAndRotation(currentTracked.transform.position);
                    lastTracked = currentTracked;
                }
                else
                {
                    if (currentTracked != lastTracked)
                    {
                        RMMRDA.instance.MRLineSetPositionAndRotation(currentTracked.transform.position);
                        lastTracked = currentTracked;
                    }
                }

            }

        }
    }
    private void PlaneChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added !=null)
        {
            foreach (var item in args.added)
            {
                trackedPlanes.Add(item);
                item.gameObject.layer = 30;
            }
        }
        if (args.removed != null)
        {
            foreach (var item in args.removed)
            {
                trackedPlanes.Remove(item);
            }
        }

        UpdateMRLinePosition();

    }
}