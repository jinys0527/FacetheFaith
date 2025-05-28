using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Linq; // �� �ʿ���

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public int gold = 0;
    public const int MAXCOST = 5;
    public int cost = MAXCOST;
    public int maxCost = MAXCOST;
    public int carryOverCost = 0;

    // �� ���� �� ���׷��̵�� �� ������ Dictionary�� ����
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
        // �̱��� �ʱ�ȭ
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� ����
            InitializePieceDictionaries();
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
    }

    // Dictionary �ʱ�ȭ
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
        Debug.Log($"[AddPiece] {pieceVariant} += {count} �� now {pieceCounts[pieceVariant]}");
        NaturalUpgradePieces();
    }

    private void NaturalUpgradePieces()
    { 
        // ���� Ű ����� ���� (������ ���)
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

    // ���� ������ �Ŵ��� �߰� ����

    //ī�� ���� dontdestroy�� �߰� ���ص� ��
}
