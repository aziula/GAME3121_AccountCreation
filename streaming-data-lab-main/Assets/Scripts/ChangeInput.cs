using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.VirtualTexturing;

public class ChangeInput : MonoBehaviour
{
    EventSystem _system;
    public Selectable firstInput;
    public Button submitButton;

    // Use this for initiatlization
    void Start()
    {
        _system = EventSystem.current;
        firstInput.Select();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            Selectable previous = _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            if (previous != null)
            {
                previous.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                next.Select();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                submitButton.onClick.Invoke();
                Debug.Log("Button pressed!");
            }
        }
    }


}
