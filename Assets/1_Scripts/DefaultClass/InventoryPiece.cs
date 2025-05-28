using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class InventoryPiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PieceVariant pieceVariant;
    public bool upgrade;
    public GameObject piecePrefab;

    public GameObject numberObject;
    Image numberImage;

    Image myImage;

    bool selected = false;

    private Sprite[] sprites;

    void Start()
    {
        numberObject = transform.GetChild(0).gameObject;

        myImage = GetComponent<Image>();
        numberImage = numberObject.GetComponent<Image>();
        sprites = new Sprite[10];
        for (int i = 0; i < 10; i++)
        {
            sprites[i] = Resources.Load<Sprite>("Image/Chess/Number/" + i.ToString());
        }
        CountingPiece();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (clickedObject == gameObject)
        {
            if (BattlePieceManager.instance.GetCount(pieceVariant, upgrade) == 0)
                return;
            GameObject piece = Instantiate(piecePrefab, new Vector3(0, 0, 0), transform.rotation);
            piece.GetComponent<Piece>().SetVariant(pieceVariant);

            PieceControlManager.instance.piece = piece.GetComponent<Piece>();
            piece.GetComponent<Piece>().canMove = true;
            
            if (upgrade)
            {
                piece.GetComponent<Piece>().Upgrade();
            }

            myImage.color = new Color(0.7f, 0.7f, 0.7f, 1);
            selected = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (!selected)
            return;
        selected = false;
        myImage.color = new Color(1, 1, 1, 1);

        BattlePieceManager.instance.SetCount(pieceVariant, upgrade, BattlePieceManager.instance.GetCount(pieceVariant, upgrade) - 1);
    }

    public void CountingPiece()
    {
        int index = BattlePieceManager.instance.GetCount(pieceVariant, upgrade);
        numberImage.sprite = sprites[index];
        if (index == 0)
        {
            myImage.color = new Color(0f, 0f, 0f, 0.5f);
        }   
        else
        {
            myImage.color = new Color(1, 1, 1, 1);
        }
    }
}
