
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ModuleGrabbingSystem : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform layoutGroup;
    InputAction click_Action;

    GraphicRaycaster raycaster;
    PointerEventData pointer_Event_Data;
    EventSystem eventSystem;

    GameObject grabbed_Object;
    void Start()
    {
        click_Action = InputSystem.actions.FindAction("Click");

        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
        pointer_Event_Data = new PointerEventData(eventSystem);
    }
    // Update is called once per frame
    void Update()
    {
        if (!click_Action.IsPressed())
        {
            grabbed_Object = null;
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);
            return;
        }
        
        if (grabbed_Object != null)
        {
            grabbed_Object.transform.position = Mouse.current.position.ReadValue();
            return;
        }


        pointer_Event_Data.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointer_Event_Data, results);

        if (results.Count <= 0)
            return;

        grabbed_Object = results[0].gameObject;

        if (!grabbed_Object.CompareTag("UIModule"))
            grabbed_Object = null;
    }
}
