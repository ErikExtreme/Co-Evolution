using UnityEngine;

public interface IGrabbableUI
{
    string PlacementTag { get; }
    bool isActive { get; set; }
}
