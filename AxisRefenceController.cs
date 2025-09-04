using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisRefenceController : MonoBehaviour
{

    public MoveObject mover;
    public Transform lookatPosition;
    public Transform fixedLookatPos;
    void Update()
    {
         
        if (lookatPosition.GetComponent<MoveObject>().isMoving)
        {
            mover.SetLookat(lookatPosition.position);
        }
        else
        {
            lookatPosition.position = fixedLookatPos.position;
        }
    }
}
