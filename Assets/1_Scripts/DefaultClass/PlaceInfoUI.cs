using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceInfoUI : MonoBehaviour
{
    public GameObject[] UIpieces = new GameObject[10];
    public GameObject[] sort = new GameObject[10];

    void Start()
    {
        int count = 0;

        if (PlayerManager.instance.pieceCounts[PieceVariant.Pawn] > 0)
        { 
            sort[count] = UIpieces[0];
            count++;
        }
        if (PlayerManager.instance.upgradedPieceCounts[PieceVariant.Pawn] > 0)
        {
            sort[count] = UIpieces[1];
            count++;
        }
        if (PlayerManager.instance.pieceCounts[PieceVariant.Knight] > 0)
        {
            sort[count] = UIpieces[2];
            count++;
        }
        if (PlayerManager.instance.upgradedPieceCounts[PieceVariant.Knight] > 0)
        {
            sort[count] = UIpieces[3];
            count++;
        }
        if (PlayerManager.instance.pieceCounts[PieceVariant.Rook] > 0)
        {
            sort[count] = UIpieces[4];
            count++;
        }
        if (PlayerManager.instance.upgradedPieceCounts[PieceVariant.Rook] > 0)
        {
            sort[count] = UIpieces[5];
            count++;
        }
        if (PlayerManager.instance.pieceCounts[PieceVariant.Bishop] > 0)
        {
            sort[count] = UIpieces[6];
            count++;
        }
        if (PlayerManager.instance.upgradedPieceCounts[PieceVariant.Bishop] > 0)
        {
            sort[count] = UIpieces[7];
            count++;
        }
        if (PlayerManager.instance.pieceCounts[PieceVariant.Queen] > 0)
        {
            sort[count] = UIpieces[8];
            count++;
        }
        if (PlayerManager.instance.upgradedPieceCounts[PieceVariant.Queen] > 0)
        {
            sort[count] = UIpieces[9];
            count++;
        }

        UpdateUIPieces(count);

        GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
        gridLayoutGroup.padding.top = -((count-1) / 2 - 4) * 80;
    }

    void UpdateUIPieces(int count)
    {
        for (int i = 0; i < 10; i++)
        {
            UIpieces[i].SetActive(false);
        }
        for (int i = 0; i < count; i++)
        {
            sort[i].SetActive(true);
        }

        if (GameManager.instance.currentState == GameState.Stage_Battle)
        {
            BattlePieceManager.instance.inventoryPieces = GameObject.Find("Canvas_Overlay").GetComponentsInChildren<InventoryPiece>();
        }
    }
}
