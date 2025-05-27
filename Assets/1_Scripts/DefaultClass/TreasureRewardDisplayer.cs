using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TreasureRoomRewardGenerator;

public class TreasureRewardDisplayer : MonoBehaviour
{
    [Header("보상 이미지 오브젝트")]
    public Image[] cardImages;        // GiftCard_Image1 ~ 3
    public Image[] mainPieceImages;   // GiftChess_Image1 ~ 3
    public Image[] bonusPieceImages;  // GiftBonusChess_Image1 ~ 3

    [Header("기물 이름 (Object_이름.png)")]
    public List<string> pieceNames;

    private Dictionary<string, Sprite> testPieceSprites;

    private void Awake()
    {
        LoadTestPieceSprites();
    }

    void LoadTestPieceSprites()
    {
        testPieceSprites = new Dictionary<string, Sprite>();
        foreach (string name in pieceNames)
        {
            Sprite sprite = Resources.Load<Sprite>($"Image/Chess/Object_{name}");
            if (sprite != null)
            {
                testPieceSprites[name] = sprite;
                Debug.Log($"스프라이트 로드 성공: {name}");
            }
            else
            {
                Debug.LogWarning($"스프라이트 없음: Object_{name}");
            }
        }
    }

    public void DisplayReward(int index, RewardContent rewardContent, string cardGrade, string piece1, string piece2 = null)
    {
        Debug.Log($"[보상 {index}] 카드: {cardGrade}, 기물1: {piece1}, 기물2: {piece2}");

        // 카드 표시 (색상 및 텍스트)
        cardImages[index].sprite = Resources.Load<Sprite>("Image/Card/CARD_" + rewardContent.cardindex.ToString());
        var txt = cardImages[index].GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (txt != null) txt.text = cardGrade;

        // 기물 1
        if (testPieceSprites.TryGetValue(piece1, out Sprite sprite1))
        {
            mainPieceImages[index].sprite = sprite1;
            mainPieceImages[index].color = Color.white;

            Debug.Log($"[적용된 스프라이트] {sprite1.name} → mainPieceImages[{index}]");
        }
        else
        {
            Debug.LogWarning($"[기물 스프라이트 없음] {piece1}");
        }

        // 기물 2 (보너스)
        if (!string.IsNullOrEmpty(piece2) && testPieceSprites.TryGetValue(piece2, out Sprite sprite2))
        {
            bonusPieceImages[index].sprite = sprite2;
            bonusPieceImages[index].color = Color.white;
        }
        else
        {
            bonusPieceImages[index].color = new Color(1, 1, 1, 0); // 안 보이게 처리
        }

    }

    Color GetCardGradeColor(string grade)
    {
        return grade switch
        {
            "일반" => Color.gray,
            "레어" => Color.cyan,
            "전설" => Color.yellow,
            _ => Color.white,
        };
    }
}
