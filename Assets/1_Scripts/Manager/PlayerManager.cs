using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    private void Awake()
    {
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
        }
        else
        {
            Destroy(gameObject); // 중복 제거
        }
    }

    public int gold = 0;
    public const int MAXCOST = 5;
    public int cost = MAXCOST;
    public int maxCost = MAXCOST;
    public int carryOverCost = 0;

    public int pawnCount = 0;
    public int knightCount = 0;
    public int bishopCount = 0;
    public int rookCount = 0;
    public int queenCount = 0;
    public int upgradedPawnCount = 0;
    public int upgradedKnightCount = 0;
    public int upgradedBishopCount = 0;
    public int upgradedRookCount = 0;
    public int upgradedQueenCount = 0;

    public void ResetPieces()
    {
        pawnCount = 0;
        knightCount = 0;
        bishopCount = 0;
        rookCount = 0;
        queenCount = 0;
        upgradedBishopCount = 0;
        upgradedKnightCount = 0;
        upgradedPawnCount = 0;
        upgradedQueenCount = 0;
        upgradedRookCount = 0;
    }

    public void AddPiece(PieceVariant pieceVariant, int count)
    {
        print("addpiece");
        print(pieceVariant);
        print(count);
        switch (pieceVariant)
        {
            case PieceVariant.None:
                break;
            case PieceVariant.Pawn:
                pawnCount += count;
                break;
            case PieceVariant.Knight:
                knightCount += count;
                break;
            case PieceVariant.Rook:
                rookCount += count;
                break;
            case PieceVariant.Bishop:
                bishopCount += count;
                break;
            case PieceVariant.Queen:
                queenCount += count;
                break;
        }
        NaturalUpgradePiece();
    }

    void NaturalUpgradePiece()
    {

        while (pawnCount >= 3)
        {
            print(pawnCount);
            pawnCount -= 3;
            upgradedPawnCount += 1;
        }
        while (knightCount >= 3)
        {
            print(knightCount);
            knightCount -= 3;
            upgradedKnightCount += 1;
        }
        while (rookCount >= 3)
        {
            print(rookCount);
            rookCount -= 3;
            upgradedRookCount += 1;
        }
        while (bishopCount >= 3)
        {
            print(bishopCount);
            bishopCount -= 3;
            upgradedBishopCount += 1;
        }
        while (queenCount >= 3)
        {
            print(queenCount);
            queenCount -= 3;
            upgradedQueenCount += 1;
        }

    }

    public void UpgradePiece(PieceVariant pieceVariant)
    {
        switch (pieceVariant)
        {
            case PieceVariant.None:
                break;
            case PieceVariant.Pawn:
                if (pawnCount > 0)
                {
                    pawnCount -= 1;
                    upgradedPawnCount += 1;
                }
                
                break;
            case PieceVariant.Knight:
                if (knightCount > 0)
                {
                    knightCount -= 1;
                    upgradedKnightCount += 1;
                }
                break;
            case PieceVariant.Rook:
                if (rookCount > 0)
                {
                    rookCount -= 1;
                    upgradedRookCount += 1;
                }
                break;
            case PieceVariant.Bishop:
                if (bishopCount > 0)
                {
                    bishopCount -= 1;
                    upgradedBishopCount += 1;
                }
                break;
            case PieceVariant.Queen:
                if (queenCount > 0)
                {
                    queenCount -= 1;
                    upgradedQueenCount += 1;
                }
                break;
        }
    }

    //아이템 매니저 만들면 추가 

    //카드 만약 dontdestroy면 추가 안해도 됨
}
