using UnityEngine;
using UnityEngine.UI;

public interface IGrabbableUI
{
    string PlacementTag { get; }
    bool isActive { get; set; }

    void DisplayStats(Text statsTextBox);
}
