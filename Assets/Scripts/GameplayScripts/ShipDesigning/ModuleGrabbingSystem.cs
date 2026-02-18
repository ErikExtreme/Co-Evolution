
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ModuleGrabbingSystem : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform inventoryLayoutGroup;
    [SerializeField] RectTransform weaponsLayoutGroup;
    [SerializeField] RectTransform modulesLayoutGroup;
    InputAction click_Action;
    InputAction openBlueprint_Action;

    GraphicRaycaster raycaster;
    PointerEventData pointer_Event_Data;
    EventSystem eventSystem;

    Transform grabbed_Object_Transform;
    string placementTag;

    [SerializeField] ShipBlueprint shipBlueprint;
    [SerializeField] GameObject emptySlotPrefab;
    void Start()
    {
        click_Action = InputSystem.actions.FindAction("Click");
        openBlueprint_Action = InputSystem.actions.FindAction("OpenBlueprint");

        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
        pointer_Event_Data = new PointerEventData(eventSystem);
    }

    void Update()
    {
        if (grabbed_Object_Transform != null)
        {
            grabbed_Object_Transform.position = Mouse.current.position.ReadValue();


            if (click_Action.WasReleasedThisFrame())
            {
                if (grabbed_Object_Transform.parent != modulesLayoutGroup && grabbed_Object_Transform.parent != weaponsLayoutGroup)
                    grabbed_Object_Transform.SetParent(inventoryLayoutGroup);

                Transform placementPoint = Raycast_To_Get_Transform(placementTag);

                if (placementPoint != null)
                    Set_Module(grabbed_Object_Transform, placementPoint);

                grabbed_Object_Transform = null;

                LayoutRebuilder.MarkLayoutForRebuild(inventoryLayoutGroup);
                LayoutRebuilder.MarkLayoutForRebuild(weaponsLayoutGroup);
                LayoutRebuilder.MarkLayoutForRebuild(modulesLayoutGroup);
            }
        }

        if (click_Action.WasPressedThisFrame() && grabbed_Object_Transform == null)
        {
            grabbed_Object_Transform = Raycast_To_Get_Transform("UIModule");

            if (grabbed_Object_Transform == null)
                return;

            if (grabbed_Object_Transform.parent == inventoryLayoutGroup)
                grabbed_Object_Transform.SetParent(canvas.transform);

            if (grabbed_Object_Transform.TryGetComponent<IGrabbableUI>(out var objectScript))
            {
                placementTag = objectScript.PlacementTag;

                if (objectScript.isActive)
                    RemoveModule(grabbed_Object_Transform);
            }

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

        grabbed_Object_Transform.TryGetComponent<IGrabbableUI>(out var objectScript);

        grabbedObject.SetParent(locationTransform.parent);
        grabbedObject.transform.localScale = locationTransform.localScale;
        grabbedObject.SetSiblingIndex(siblingIndex);
        Destroy(locationTransform.gameObject);

        if (grabbed_Object_Transform.TryGetComponent<UIWeapon>(out var uiWeapon))
        {
            int horiPos = siblingIndex % shipBlueprint.MaxWeaponGridSize;
            int vertPos = siblingIndex / shipBlueprint.MaxWeaponGridSize;

            shipBlueprint.SetWeapon(uiWeapon.weaponGenome, uiWeapon.weaponStatsTracker, horiPos, vertPos);

            uiWeapon.isActive = true;
        }
        if (grabbed_Object_Transform.TryGetComponent<UIShipModule>(out var uiShipModule))
        {
            int horiPos = siblingIndex % shipBlueprint.ModuleListSize;

            shipBlueprint.SetModule(uiShipModule.shipGenome, uiShipModule.moduleStatsTracker, horiPos);

            uiShipModule.isActive = true;
        }
    }
    private void RemoveModule(Transform grabbedObject)
    {
        int siblingIndex = grabbedObject.GetSiblingIndex();

        GameObject slotInstance = Instantiate(emptySlotPrefab, grabbedObject.parent);
        slotInstance.transform.SetSiblingIndex(siblingIndex);
        grabbedObject.SetParent(canvas.transform);
        grabbedObject.transform.localScale = Vector2.one;

        if (grabbed_Object_Transform.TryGetComponent<UIWeapon>(out var uiWeapon))
        {
            int horiPos = siblingIndex % shipBlueprint.MaxWeaponGridSize;
            int vertPos = siblingIndex / shipBlueprint.MaxWeaponGridSize;

            shipBlueprint.RemoveWeapon(horiPos, vertPos);

            uiWeapon.isActive = false;
        }
        if (grabbed_Object_Transform.TryGetComponent<UIShipModule>(out var uiShipModule))
        {
            int horiPos = siblingIndex % shipBlueprint.ModuleListSize;

            shipBlueprint.RemoveModule(horiPos);

            uiShipModule.isActive = false;

            slotInstance.tag = "ModuleSlot";
        }
    }
}
