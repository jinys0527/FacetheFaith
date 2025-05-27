using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

public class PreventClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        PieceControlManager.instance.blockPiece = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PieceControlManager.instance.blockPiece = false;
    }
}
