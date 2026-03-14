using UnityEngine;

public class SelectUpgrade : MonoBehaviour
{
    public UpgradeSelectionManager upgradeSelectionManager;

    public void SelectThis(bool isWeapon)
    {
        if (isWeapon)
            upgradeSelectionManager.AddWeapon(transform.GetComponent<UIWeapon>());
        else
            upgradeSelectionManager.AddModule(transform.GetComponent<UIShipModule>());
    }
}
