using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class UpgradePieceUI : MonoBehaviour
{
    public static GameObject prefab; // �⹰ UI ������
    public static Transform grid; // GridLayoutGroup ���� ������Ʈ
    public static Sprite[] sprites = new Sprite[10]; // 0~9 ���� �̹���

    static int columns = 3;

    void Start()
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>("Prefabs/P_JYS/UI/UpgradePiece");
        }
        if (grid == null)
        {
            grid = transform.Find("Grid");
        }
        LoadNumberSprites();
        UpdateUpgradeUI();
        StartCoroutine(DelayedCenter());
    }

    void LoadNumberSprites()
    {
        for (int i = 0; i < 10; i++)
        {
            sprites[i] = Resources.Load<Sprite>("Image/Chess/Number/" + i.ToString());
        }
    }

    public static void UpdateUpgradeUI()
    {
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }

        AddPieceUI(PlayerManager.instance.pieceCounts[PieceVariant.Pawn], ePieceType.Pawn);
        AddPieceUI(PlayerManager.instance.pieceCounts[PieceVariant.Knight], ePieceType.Knight);
        AddPieceUI(PlayerManager.instance.pieceCounts[PieceVariant.Rook], ePieceType.Rook);
        AddPieceUI(PlayerManager.instance.pieceCounts[PieceVariant.Bishop], ePieceType.Bishop);
        AddPieceUI(PlayerManager.instance.pieceCounts[PieceVariant.Queen], ePieceType.Queen);
    }

    static void AddPieceUI(int count, ePieceType type)
    {
        if (count <= 0) return;

        GameObject obj = Instantiate(prefab, grid);
        // ������ ����
        Image iconImage = obj.GetComponent<Image>();
        iconImage.sprite = GetPieceSprite(type);

        // ���� ����
        Image numberImage = obj.transform.Find("Number").GetComponent<Image>();
        numberImage.sprite = sprites[Mathf.Clamp(count, 0, 9)];
    }

    static Sprite GetPieceSprite(ePieceType type)
    {
        return Resources.Load<Sprite>("Image/Chess/Piece/" + type.ToString());
    }

    IEnumerator DelayedCenter()
    {
        // Wait until layout is done
        yield return new WaitForEndOfFrame();
        CenterLastRow();
    }

    public static void CenterLastRow()
    {
        int childCount = grid.transform.childCount;
        if (childCount == 0) return;

        int fullRows = childCount / columns;
        int lastRowCount = childCount % columns;

        if (lastRowCount == 0) return;

        // ���� ��ġ ��� (Grid�� 3ĭ�� �� ���� ��ġ�Ǵ� X ��ǥ��)
        float[] columnX = new float[] { 460f, 960f, 1460f };

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
            float firstX = (columnX[0] + columnX[1]) / 2f;  
            float secondX = (columnX[1] + columnX[2]) / 2f; 

            Transform item1 = grid.transform.GetChild(startIndex);
            RectTransform rt1 = item1.GetComponent<RectTransform>();
            rt1.anchoredPosition = new Vector2(firstX, rt1.anchoredPosition.y);

            Transform item2 = grid.transform.GetChild(startIndex + 1);
            RectTransform rt2 = item2.GetComponent<RectTransform>();
            rt2.anchoredPosition = new Vector2(secondX, rt2.anchoredPosition.y);
        }
    }
}