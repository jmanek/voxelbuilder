using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TabNext : MonoBehaviour {

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {

                InputField inputfield = next.GetComponent<InputField>();
                //if (inputfield != null)
                //{
                //    inputfield.OnPointerClick(new PointerEventData(EventSystem.current));
                //}
                EventSystem.current.SetSelectedGameObject(next.gameObject, new BaseEventData(EventSystem.current));
            }

        }
    }
}
