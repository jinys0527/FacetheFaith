using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Linq; // 꼭 필요함

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public int gold = 0;
    public const int MAXCOST = 5;
    public int cost = MAXCOST;
    public int maxCost = MAXCOST;
    public int carryOverCost = 0;

    // 말 개수 및 업그레이드된 말 개수를 Dictionary로 관리
    public Dictionary<PieceVariant, int> pieceCounts = new();
    public Dictionary<PieceVariant, int> upgradedPieceCounts = new();

    private List<PieceVariant> usableVariants = new List<PieceVariant>
    {
        PieceVariant.Pawn,
        PieceVariant.Knight,
        PieceVariant.Rook,
        PieceVariant.Bishop,
        PieceVariant.Queen
    };

    private void Awake()
    {
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
            InitializePieceDictionaries();
        }
        else
        {
            Destroy(gameObject); // 중복 제거
        }
    }

    // Dictionary 초기화
    private void InitializePieceDictionaries()
    {
        foreach (PieceVariant variant in usableVariants)
        {
            pieceCounts[variant] = 0;
            upgradedPieceCounts[variant] = 0;
        }
    }

    public void ResetPieces()
    {
        foreach (PieceVariant variant in pieceCounts.Keys.ToList())
        {
            pieceCounts[variant] = 0;
        }

        foreach (PieceVariant variant in upgradedPieceCounts.Keys.ToList())
        {
            upgradedPieceCounts[variant] = 0;
        }
    }

    public void AddPiece(PieceVariant pieceVariant, int count)
    {
        if (pieceVariant == PieceVariant.None) return;

        pieceCounts[pieceVariant] += count;
        Debug.Log($"[AddPiece] {pieceVariant} += {count} → now {pieceCounts[pieceVariant]}");
        NaturalUpgradePieces();
    }

    private void NaturalUpgradePieces()
    { 
        // 먼저 키 목록을 복사 (스냅샷 방식)
        List<PieceVariant> keys = new List<PieceVariant>(pieceCounts.Keys);

        foreach (PieceVariant variant in keys)
        {
            while (pieceCounts[variant] >= 3)
            {
                pieceCounts[variant] -= 3;

                if (!upgradedPieceCounts.ContainsKey(variant))
                    upgradedPieceCounts[variant] = 0;

                upgradedPieceCounts[variant] += 1;
                Debug.Log($"[Auto Upgrade] {variant} upgraded! Total Upgraded: {upgradedPieceCounts[variant]}");
            }
        }
    }

    public void UpgradePiece(PieceVariant pieceVariant)
    {
        if (pieceVariant == PieceVariant.None) return;

        if (pieceCounts[pieceVariant] > 0)
        {
            pieceCounts[pieceVariant] -= 1;
            upgradedPieceCounts[pieceVariant] += 1;
            Debug.Log($"[Manual Upgrade] {pieceVariant} upgraded! Total Upgraded: {upgradedPieceCounts[pieceVariant]}");
        }
        else
        {
            Debug.LogWarning($"[Manual Upgrade] Not enough {pieceVariant} to upgrade.");
        }
    }

    // 향후 아이템 매니저 추가 가능

    //카드 만약 dontdestroy면 추가 안해도 됨
}
