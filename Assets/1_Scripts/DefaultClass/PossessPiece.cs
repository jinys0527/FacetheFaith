using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PossessPiece : MonoBehaviour
{
    public static GameObject prefab; // 기물 UI 프리팹
    public static Transform grid; // GridLayoutGroup 붙은 오브젝트
    public static Sprite[] sprites = new Sprite[10]; // 0~9 숫자 이미지

    void OnEnable()
    {   
        if(prefab == null)
        {
            prefab = Resources.Load<GameObject>("Prefabs/P_JYS/UI/PossessPiece");
        }
        if(grid == null)
        {
            grid = transform.Find("Grid");
        }
        LoadNumberSprites();
    }

    void LoadNumberSprites()
    {
        for (int i = 0; i < 10; i++)
        {
            sprites[i] = Resources.Load<Sprite>("Image/Chess/Number/" + i.ToString());
        }
    }

    public static void UpdatePossessUI()
    {
        // 기존 자식 정리
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }

        AddPieceUI(PlayerManager.instance.pawnCount, ePieceType.Pawn);
        AddPieceUI(PlayerManager.instance.upgradedPawnCount, ePieceType.UpgradePawn);
        AddPieceUI(PlayerManager.instance.knightCount, ePieceType.Knight);
        AddPieceUI(PlayerManager.instance.upgradedKnightCount, ePieceType.UpgradeKnight);
        AddPieceUI(PlayerManager.instance.rookCount, ePieceType.Rook);
        AddPieceUI(PlayerManager.instance.upgradedRookCount, ePieceType.UpgradeRook);
        AddPieceUI(PlayerManager.instance.bishopCount, ePieceType.Bishop);
        AddPieceUI(PlayerManager.instance.upgradedBishopCount, ePieceType.UpgradeBishop);
        AddPieceUI(PlayerManager.instance.queenCount, ePieceType.Queen);
        AddPieceUI(PlayerManager.instance.upgradedQueenCount, ePieceType.UpgradeQueen);
    }

    static void AddPieceUI(int count, ePieceType type)
    {
        if (count <= 0) return;

        GameObject obj = Instantiate(prefab, grid);
        // 아이콘 세팅
        Image iconImage = obj.GetComponent<Image>();
        iconImage.sprite = GetPieceSprite(type);

        // 숫자 세팅
        Image numberImage = obj.transform.Find("Number").GetComponent<Image>();
        numberImage.sprite = sprites[Mathf.Clamp(count, 0, 9)];
    }

    static Sprite GetPieceSprite(ePieceType type)
    {
        return Resources.Load<Sprite>("Image/Chess/Piece/" + type.ToString());
    }
}