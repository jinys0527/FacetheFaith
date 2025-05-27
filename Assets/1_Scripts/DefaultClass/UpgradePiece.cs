using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradePiece : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (clickedObject == gameObject)
        {
            gameObject.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 1);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PieceVariant pieceVariant = PieceVariant.None;
        if (gameObject.name == "Pawn")
            pieceVariant = PieceVariant.Pawn;
        else if (gameObject.name == "Knight")
            pieceVariant = PieceVariant.Knight;
        else if (gameObject.name == "Rook")
            pieceVariant = PieceVariant.Rook;
        else if (gameObject.name == "Bishop")
            pieceVariant = PieceVariant.Bishop;
        else if (gameObject.name == "Queen")
            pieceVariant = PieceVariant.Queen;
        PlayerManager.instance.UpgradePiece(pieceVariant);
        SceneChageManager.Instance.ChangeGameState(GameState.Map);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }
}
