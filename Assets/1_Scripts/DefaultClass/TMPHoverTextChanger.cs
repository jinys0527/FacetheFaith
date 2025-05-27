using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TMPHoverTextChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI targetText;
    public string hoverText = "���콺 �ø�";
    public string defaultText = "�⺻ �ؽ�Ʈ";

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
