using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TreasureRoomRewardGenerator;

public class TreasureRewardDisplayer : MonoBehaviour
{
    [Header("���� �̹��� ������Ʈ")]
    public Image[] cardImages;        // GiftCard_Image1 ~ 3
    public Image[] mainPieceImages;   // GiftChess_Image1 ~ 3
    public Image[] bonusPieceImages;  // GiftBonusChess_Image1 ~ 3

    [Header("�⹰ �̸� (Object_�̸�.png)")]
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
                Debug.Log($"��������Ʈ �ε� ����: {name}");
            }
            else
            {
                Debug.LogWarning($"��������Ʈ ����: Object_{name}");
            }
        }
    }

    public void DisplayReward(int index, RewardContent rewardContent, string cardGrade, string piece1, string piece2 = null)
    {
        Debug.Log($"[���� {index}] ī��: {cardGrade}, �⹰1: {piece1}, �⹰2: {piece2}");

        // ī�� ǥ�� (���� �� �ؽ�Ʈ)
        cardImages[index].sprite = Resources.Load<Sprite>("Image/Card/CARD_" + rewardContent.cardindex.ToString());
        var txt = cardImages[index].GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (txt != null) txt.text = cardGrade;

        // �⹰ 1
        if (testPieceSprites.TryGetValue(piece1, out Sprite sprite1))
        {
            mainPieceImages[index].sprite = sprite1;
            mainPieceImages[index].color = Color.white;

            Debug.Log($"[����� ��������Ʈ] {sprite1.name} �� mainPieceImages[{index}]");
        }
        else
        {
            Debug.LogWarning($"[�⹰ ��������Ʈ ����] {piece1}");
        }

        // �⹰ 2 (���ʽ�)
        if (!string.IsNullOrEmpty(piece2) && testPieceSprites.TryGetValue(piece2, out Sprite sprite2))
        {
            bonusPieceImages[index].sprite = sprite2;
            bonusPieceImages[index].color = Color.white;
        }
        else
        {
            bonusPieceImages[index].color = new Color(1, 1, 1, 0); // �� ���̰� ó��
        }

    }

    Color GetCardGradeColor(string grade)
    {
        return grade switch
        {
            "�Ϲ�" => Color.gray,
            "����" => Color.cyan,
            "����" => Color.yellow,
            _ => Color.white,
        };
    }
}
