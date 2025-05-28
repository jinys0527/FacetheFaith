using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class PieceSpriteDB
{
    public static Sprite pawnSprite;
    public static Sprite knightSprite;
    public static Sprite rookSprite;
    public static Sprite bishopSprite;
    public static Sprite queenSprite;

    public static void Load()
    {
        pawnSprite = Resources.Load<Sprite>("Image/Chess/Piece/Pawn");
        knightSprite = Resources.Load<Sprite>("Image/Chess/Piece/Knight");
        rookSprite = Resources.Load<Sprite>("Image/Chess/Piece/Rook");
        bishopSprite = Resources.Load<Sprite>("Image/Chess/Piece/Bishop");
        queenSprite = Resources.Load<Sprite>("Image/Chess/Piece/Queen");
    }
}

public class UpgradePiece : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    GameObject clickedObject;

    private void OnEnable()
    {
        PieceSpriteDB.Load();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (clickedObject == gameObject)
        {
            gameObject.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 1);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Image img = clickedObject.GetComponent<Image>();
        if (img == null || img.sprite == null) return;

        Sprite clickedSprite = img.sprite;
        PieceVariant pieceVariant = PieceVariant.None;

        if (clickedSprite == PieceSpriteDB.pawnSprite)
            pieceVariant = PieceVariant.Pawn;
        else if (clickedSprite == PieceSpriteDB.knightSprite)
            pieceVariant = PieceVariant.Knight;
        else if (clickedSprite == PieceSpriteDB.rookSprite)
            pieceVariant = PieceVariant.Rook;
        else if (clickedSprite == PieceSpriteDB.bishopSprite)
            pieceVariant = PieceVariant.Bishop;
        else if (clickedSprite == PieceSpriteDB.queenSprite)
            pieceVariant = PieceVariant.Queen;
        else
            Debug.LogWarning("알 수 없는 스프라이트: " + clickedSprite.name);

        if (pieceVariant != PieceVariant.None)
        {
            PlayerManager.instance.UpgradePiece(pieceVariant);
            SceneChangeManager.Instance.ChangeGameState(GameState.Map);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }
}
