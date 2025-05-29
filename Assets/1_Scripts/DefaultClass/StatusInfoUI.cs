using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusInfoUI : MonoBehaviour
{
    public GameObject prefab;
    public List<GameObject> pieces = new List<GameObject>();
    public GridLayoutGroup grid;
    public int columns = 2;

    public int count = 0;

    void Start()
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
            pieces[i].transform.SetParent(grid.transform);
            pieces[i].transform.SetSiblingIndex(i);
            UnityEngine.UI.Image image = pieces[i].GetComponent<UnityEngine.UI.Image>();
            pieces[i].GetComponent<StatusPiece>().piece = piece;
            image.sprite = piece.level ? piece.data.upgradePieceSprite : piece.data.pieceSprite;
        }

        UpdateStatusInfo();

        StartCoroutine(DelayedCenter());
    }
    public void UpdateStatusInfo()
    {
        foreach (GameObject piece in pieces)
        {
            piece.GetComponent<StatusPiece>().StatusUpdate();
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
        float[] columnX = new float[] { 47.5f, 137.5f };

        // ������ �� �ε��� ���� ��ġ
        int startIndex = fullRows * columns;

        if (lastRowCount == 1)
        {
            float firstX = (columnX[0] + columnX[1]) / 2f;  // 133.25

            Transform item1 = grid.transform.GetChild(startIndex);
            RectTransform rt1 = item1.GetComponent<RectTransform>();
            rt1.anchoredPosition = new Vector2(firstX, rt1.anchoredPosition.y);
        }
    }
}
