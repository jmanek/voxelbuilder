using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIMask : MonoBehaviour, IPointerEnterHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("EVENT");
    }

}
