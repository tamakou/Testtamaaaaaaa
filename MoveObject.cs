using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float speed = 20;
    public bool hasMovedByUser = false;
    public bool isMoving = false;
    public bool freezeX, freezeY, freezeZ;
    public void SetPosition(Vector3 pos)
    {
        Vector3 targetPos = new Vector3(freezeX ? transform.position.x : pos.x, freezeY ? transform.position.y : pos.y, freezeZ ? transform.position.z : pos.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
        if (!hasMovedByUser) hasMovedByUser = true;
    }
    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }
    public void SetLookat(Vector3 trans)
    {
        transform.LookAt(trans);
    }
}
