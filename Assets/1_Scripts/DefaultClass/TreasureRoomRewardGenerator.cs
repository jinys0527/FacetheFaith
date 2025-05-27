using System;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoomRewardGenerator : MonoBehaviour
{
    [Serializable]
    public class RewardRate
    {
        public string name;
        public float normalRate;
        public float rareRate;
        public float legendaryRate;
        public float bonusPieceRate;
    }

    [Serializable]
    public class PieceRate
    {
        public string name;
        public float rate;
    }

    [Serializable]
    public class RewardContent
    {
        public RewardContent(string grade, string piece1, string piece2)
        {
            if(grade == "일반")
            {
                cardindex = UnityEngine.Random.Range(0, 14);
            }
            else if (grade == "레어")
            {
                cardindex = 14 + UnityEngine.Random.Range(0, 9);
            }
            else if (grade == "전설")
            {
                cardindex = 23 + UnityEngine.Random.Range(0, 3);
            }

            if(piece1 == "Pawn")
            {
                pieceVariant = PieceVariant.Pawn;
            }
            else if (piece1 == "Knight")
            {
                pieceVariant = PieceVariant.Knight;
            }
            else if (piece1 == "Rook")
            {
                pieceVariant = PieceVariant.Rook;
            }
            else if (piece1 == "Bishop")
            {
                pieceVariant = PieceVariant.Bishop;
            }
            else if (piece1 == "Queen")
            {
                pieceVariant = PieceVariant.Queen;
            }
            else
            {
                pieceVariant = PieceVariant.None;
            }

            if (piece2 != null)
            {
                if (piece2 == "Pawn")
                {
                    secondPieceVariant = PieceVariant.Pawn;
                }
                else if (piece2 == "Knight")
                {
                    secondPieceVariant = PieceVariant.Knight;
                }
                else if (piece2 == "Rook")
                {
                    secondPieceVariant = PieceVariant.Rook;
                }
                else if (piece2 == "Bishop")
                {
                    secondPieceVariant = PieceVariant.Bishop;
                }
                else if (piece2 == "Queen")
                {
                    secondPieceVariant = PieceVariant.Queen;
                }
            }
            else
            {
                secondPieceVariant = PieceVariant.None;
            }
        }
        public int cardindex;
        public PieceVariant pieceVariant;
        public PieceVariant secondPieceVariant;
    }

    public List<RewardRate> rewardRates; // A/B/C 유형별 카드 확률
    public List<PieceRate> pieceRates;   // 기물 확률
    public static List<RewardContent> rewardContents = new List<RewardContent>();

    [SerializeField] private TreasureRewardDisplayer rewardDisplayer;

    private void Start()
    {
        string[] types = { "A", "B", "C" };

        for (int i = 0; i < 3; i++)
        {
            var (grade, piece1, piece2) = GenerateReward(types[i]);
            RewardContent tempReward = new (grade, piece1, piece2);

            rewardContents.Add(tempReward);
            rewardDisplayer.DisplayReward(i, tempReward, grade, piece1, piece2);
        }
    }

    public (string cardGrade, string piece1, string piece2) GenerateReward(string type)
    {
        var reward = rewardRates.Find(r => r.name == type);
        if (reward == null)
        {
            Debug.LogError("잘못된 보물방 타입 입력: " + type);
            return ("없음", null, null);
        }

        string cardGrade = GetRandomCardGrade(reward.normalRate, reward.rareRate, reward.legendaryRate);
        string piece1 = GetRandomPiece();
        string piece2 = null;

        if (UnityEngine.Random.value < reward.bonusPieceRate / 100f)
            piece2 = GetRandomPiece();

        return (cardGrade, piece1, piece2);
    }

    string GetRandomCardGrade(float normal, float rare, float legendary)
    {
        float rand = UnityEngine.Random.Range(0f, 100f);
        if (rand < normal) return "일반";
        if (rand < normal + rare) return "레어";
        return "전설";
    }

    string GetRandomPiece()
    {
        float rand = UnityEngine.Random.Range(0f, 100f);
        float cumulative = 0f;

        foreach (var piece in pieceRates)
        {
            cumulative += piece.rate;
            if (rand < cumulative)
                return piece.name;
        }

        return "알 수 없음";
    }
}
