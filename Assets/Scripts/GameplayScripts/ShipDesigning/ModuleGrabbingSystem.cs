
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
    InputAction openBlueprint_Action;

    GraphicRaycaster raycaster;
    PointerEventData pointer_Event_Data;
    EventSystem eventSystem;

    Transform grabbed_Object_Transform;
    string placementTag;

    [SerializeField] ShipBlueprint shipBlueprint;
    void Start()
    {
        click_Action = InputSystem.actions.FindAction("Click");
        openBlueprint_Action = InputSystem.actions.FindAction("OpenBlueprint");

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
                Transform placementPoint = Raycast_To_Get_Transform(placementTag);

                if (placementPoint != null)
                    Set_Module(grabbed_Object_Transform, placementPoint);

                grabbed_Object_Transform = null;
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);
            }
        }

        if (click_Action.WasPressedThisFrame() && grabbed_Object_Transform == null)
        {
            grabbed_Object_Transform = Raycast_To_Get_Transform("UIModule");

            if (grabbed_Object_Transform != null &&
                grabbed_Object_Transform.TryGetComponent<IGrabbableUI>(out var objectScript))
                placementTag = objectScript.PlacementTag;
        }

        if (openBlueprint_Action.WasPressedThisFrame())
            canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
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
    private void Set_Module(Transform grabbedObject, Transform locationTransform)
    {
        int siblingIndex = locationTransform.GetSiblingIndex();

        grabbedObject.SetParent(locationTransform.parent, false);
        grabbedObject.SetSiblingIndex(siblingIndex);


        if (grabbed_Object_Transform.TryGetComponent<UIWeapon>(out var uiWeapon))
        {
            int horiPos = siblingIndex % shipBlueprint.CurrentGridWidth;
            int vertPos = siblingIndex / shipBlueprint.CurrentGridWidth;

            shipBlueprint.SetWeapon(uiWeapon.weaponGenome, horiPos, vertPos);
        }
        if (grabbed_Object_Transform.TryGetComponent<UIShipModule>(out var uiShipModule))
        {
            int horiPos = siblingIndex % 5;//HARDCODED 5 FIX

            shipBlueprint.SetModule(uiShipModule.shipGenome, horiPos);
        }

        Destroy(locationTransform.gameObject);
    }
}
