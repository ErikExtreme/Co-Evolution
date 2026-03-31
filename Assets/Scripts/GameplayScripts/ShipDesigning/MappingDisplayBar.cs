using UnityEngine;

public class MappingDisplayBar : MonoBehaviour
{
    RectTransform rectTransform;
    float fullPosition;
    float emptyPosition;

    float value;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        fullPosition = rectTransform.anchoredPosition.x;
        emptyPosition = fullPosition - GetComponentInParent<RectTransform>().rect.width;
    }

    void Update()
    {
        float fillPercent = value;
        rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(emptyPosition, fullPosition, fillPercent), rectTransform.anchoredPosition.y);
    }

    public void SetValue(float value)
    {
        this.value = value;
    }
}
