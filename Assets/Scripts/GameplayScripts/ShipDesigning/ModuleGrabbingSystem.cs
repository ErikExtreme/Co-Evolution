
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

    Transform grabbed_Object_Transform;

    [SerializeField] ShipBlueprint shipBlueprint;
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
        if (grabbed_Object_Transform != null)
        {
            grabbed_Object_Transform.position = Mouse.current.position.ReadValue();


            if (click_Action.WasReleasedThisFrame())
            {
                Transform placementPoint = Raycast_To_Get_Transform("PlacementPoint");

                if (placementPoint != null)
                    Set_Module(placementPoint);

                grabbed_Object_Transform = null;
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);
            }
        }

        if (click_Action.WasPressedThisFrame() && grabbed_Object_Transform == null)
            grabbed_Object_Transform = Raycast_To_Get_Transform("UIModule");
    }
    private Transform Raycast_To_Get_Transform(string tagName)
    {
        pointer_Event_Data.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointer_Event_Data, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag(tagName))
                return result.gameObject.transform;
        }
        return null;
    }
    private void Set_Module(Transform locationTransform)
    {
        int siblingIndex = locationTransform.GetSiblingIndex();

        grabbed_Object_Transform.SetParent(locationTransform.parent);
        grabbed_Object_Transform.SetSiblingIndex(siblingIndex);


        TempModules moduleType = grabbed_Object_Transform.GetComponent<TempModule>().moduleType;
        int horiPos = siblingIndex % 5;//HARDCODED 5 FIX
        int vertPos = siblingIndex / 5;//HARDCODED 5 FIX
        shipBlueprint.SetModule(moduleType, horiPos, vertPos);


        Destroy(locationTransform.gameObject);
    }
}
