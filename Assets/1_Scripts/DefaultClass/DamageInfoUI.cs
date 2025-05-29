using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DamageInfoUI : MonoBehaviour
{
    public GameObject prefab;
    public List<GameObject> pieces = new List<GameObject>();
    public GridLayoutGroup grid;
    public int columns = 3;

    public void Start()
    {
        // 기존에 있던 자식 오브젝트 정리
        foreach (Transform child in grid.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < BattlePieceManager.instance.pieces.Count; i++)
        {
            Piece piece = BattlePieceManager.instance.pieces[i].GetComponent<Piece>();
            if (!piece.GetIsAlive()) continue;

            pieces.Add(Instantiate(prefab, grid.transform));
            pieces[i].transform.SetSiblingIndex(i);
            UnityEngine.UI.Image image = pieces[i].GetComponent<UnityEngine.UI.Image>();
            image.sprite = piece.level ? piece.data.upgradePieceSprite : piece.data.pieceSprite;
            pieces[i].GetComponentInChildren<TMP_Text>().text = piece.currentDamage.ToString();
        }

        StartCoroutine(DelayedCenter());
    }
    public void UpdateDamageInfo()
    {
        for (int i = 0; i < BattlePieceManager.instance.pieces.Count; i++)
        {
            Piece piece = BattlePieceManager.instance.pieces[i].GetComponent<Piece>();
            if (!piece.GetIsAlive()) continue;  
            if(piece.isDistort)
            {
                pieces[i].GetComponentInChildren<TMP_Text>().text = piece.GetAtk().ToString();
            }
            else
            {
                pieces[i].GetComponentInChildren<TMP_Text>().text = piece.currentDamage.ToString();
            }
        }
    }

    IEnumerator DelayedCenter()
    {
        // Wait until layout is done
        yield return new WaitForEndOfFrame();
        CenterLastRow();
    }

    public void CenterLastRow()
    {
        int childCount = grid.transform.childCount;
        if (childCount == 0) return;

        int fullRows = childCount / columns;
        int lastRowCount = childCount % columns;

        if (lastRowCount == 0) return;

        // 기준 위치 목록 (Grid가 3칸일 때 실제 배치되는 X 좌표들)
        float[] columnX = new float[] { 77f, 189.5f, 302f };

        // 마지막 줄 인덱스 시작 위치
        int startIndex = fullRows * columns;

        if (lastRowCount == 1)
        {
            Transform item = grid.transform.GetChild(startIndex);
            RectTransform rt = item.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(columnX[1], rt.anchoredPosition.y);
        }
        else if (lastRowCount == 2)
        {
            float firstX = (columnX[0] + columnX[1]) / 2f;  // 133.25
            float secondX = (columnX[1] + columnX[2]) / 2f; // 245.75

            Transform item1 = grid.transform.GetChild(startIndex);
            RectTransform rt1 = item1.GetComponent<RectTransform>();
            rt1.anchoredPosition = new Vector2(firstX, rt1.anchoredPosition.y);

            Transform item2 = grid.transform.GetChild(startIndex + 1);
            RectTransform rt2 = item2.GetComponent<RectTransform>();
            rt2.anchoredPosition = new Vector2(secondX, rt2.anchoredPosition.y);
        }

        // 3개면 Grid가 알아서 배치하므로 수정 안 함
    }
}
