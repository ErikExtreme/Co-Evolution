using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class BarDisplay : MonoBehaviour
{
    [SerializeField] ShipHealth shipHealthScript;
    [SerializeField] string displayedStatName;

    RectTransform rectTransform;
    float fullPosition;
    float emptyPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        fullPosition = rectTransform.anchoredPosition.x;
        emptyPosition = fullPosition - rectTransform.rect.width * rectTransform.localScale.x;
    }
    private void Update()
    {
        float fillPercent = 1;
        if (displayedStatName == "shipHealth")
            fillPercent = (float)shipHealthScript.Health / shipHealthScript.HullHP;
        if (displayedStatName == "shipShield")
            fillPercent = (float)shipHealthScript.Health / shipHealthScript.ShieldCapacity;
        if (displayedStatName == "shipPower")
            fillPercent = (float)shipHealthScript.Power / shipHealthScript.PowerCapacity;

        rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(emptyPosition, fullPosition, fillPercent), rectTransform.anchoredPosition.y);
        
    }
}
