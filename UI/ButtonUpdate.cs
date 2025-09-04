using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonUpdate : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool update;
    [SerializeField] UnityEvent onClick;
    [SerializeField]
    [Tooltip("How long must pointer be down on this object to trigger a long press")]
    private float holdTime = 1f;

    public void OnPointerDown(PointerEventData eventData)
    {
        Invoke("OnLongPress", holdTime);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        update = false;
        CancelInvoke("OnLongPress");
    }
    private void OnLongPress()
    {
        update = true;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (update)
        {
            onClick?.Invoke();
        }
    }
}
