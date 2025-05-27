using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TMPHoverTextChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI targetText;
    public string hoverText = "마우스 올림";
    public string defaultText = "기본 텍스트";

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetText != null)
            targetText.text = hoverText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetText != null)
            targetText.text = defaultText;
    }
}
