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
        // ������ �ִ� �ڽ� ������Ʈ ����
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

        // ���� ��ġ ��� (Grid�� 3ĭ�� �� ���� ��ġ�Ǵ� X ��ǥ��)
        float[] columnX = new float[] { 77f, 189.5f, 302f };

        // ������ �� �ε��� ���� ��ġ
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

        // 3���� Grid�� �˾Ƽ� ��ġ�ϹǷ� ���� �� ��
    }
}
