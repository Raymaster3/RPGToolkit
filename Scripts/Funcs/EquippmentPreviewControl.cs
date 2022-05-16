using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquippmentPreviewControl : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    Vector2 initialPosition;
    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        EquipmentPreview.instance.Rotate(initialPosition.x - eventData.position.x);
        //Debug.Log(initialPosition.x - eventData.position.x);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
