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

        if (PlayerManager.instance.pawnCount > 0)
        { 
            sort[count] = UIpieces[0];
            count++;
        }
        if (PlayerManager.instance.upgradedPawnCount > 0)
        {
            sort[count] = UIpieces[1];
            count++;
        }
        if (PlayerManager.instance.knightCount > 0)
        {
            sort[count] = UIpieces[2];
            count++;
        }
        if (PlayerManager.instance.upgradedKnightCount > 0)
        {
            sort[count] = UIpieces[3];
            count++;
        }
        if (PlayerManager.instance.rookCount > 0)
        {
            sort[count] = UIpieces[4];
            count++;
        }
        if (PlayerManager.instance.upgradedRookCount > 0)
        {
            sort[count] = UIpieces[5];
            count++;
        }
        if (PlayerManager.instance.bishopCount > 0)
        {
            sort[count] = UIpieces[6];
            count++;
        }
        if (PlayerManager.instance.upgradedBishopCount > 0)
        {
            sort[count] = UIpieces[7];
            count++;
        }
        if (PlayerManager.instance.queenCount > 0)
        {
            sort[count] = UIpieces[8];
            count++;
        }
        if (PlayerManager.instance.upgradedQueenCount > 0)
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
    }
}
