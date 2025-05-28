using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapUIManager : BaseUIManager
{
    [Header("Map Popups")]
    public GameObject optionPopup;
    public GameObject chessPopup;
    public GameObject mapDeckUI;
    GameObject canvas;

    [Header("버튼")]
    [SerializeField] Button optionButton;
    [SerializeField] Button chessButton;
    [SerializeField] Button mapDeckButton;
    [SerializeField] Button[] backButton;
    [SerializeField] Button giveUpButton;

    [Header("Stage Info")]
    [SerializeField] private Image stageIcon;
    [SerializeField] private TMP_Text stageText;

    [Header("Stage Icons")]
    private Sprite iconBattle, iconElite, iconBoss, iconRest, iconTreasure, iconUnknown;

    private GameObject mapDeckContent;
    private ScrollRect mapScrollRect;

    [SerializeField] Transform mapDeckHoverLayer;
    public override Transform MapDeckHoverLayer => mapDeckHoverLayer;

    public override void Initialize()
    {
        SetupButtonEvents();
        LoadStageIcons();
        InitializePopups();
        InitializeDeckUI();
        LoadDeck();
        CloseAllPopups();
    }

    public override void SetupButtonEvents()
    {
        optionButton?.onClick.AddListener(OnOptionButtonClicked);
        chessButton?.onClick.AddListener(OnChessButtonClicked);
        mapDeckButton?.onClick.AddListener(OnMapDeckButtonClicked);
        foreach (var back in backButton)
        {
            back?.onClick.AddListener(CloseAllPopups);
        }
        
        giveUpButton?.onClick.AddListener(OnGiveUpButtonClicked);
    }

    public override void CloseAllPopups()
    {
        optionPopup?.SetActive(false);
        mapDeckUI?.SetActive(false);
        chessPopup?.SetActive(false);
        isPopupOpen = false;
    }

    private void LoadStageIcons()
    {
        iconBattle = Resources.Load<Sprite>("Image/UI/MONSTER_ICON");
        iconElite = Resources.Load<Sprite>("Image/UI/ELITE_BOSS_ICON");
        iconBoss = Resources.Load<Sprite>("Image/UI/FINAL_BOSS_ICON");
        iconRest = Resources.Load<Sprite>("Image/UI/HOME_ICON");
        iconTreasure = Resources.Load<Sprite>("Image/UI/TRESURE_ICON");
        iconUnknown = Resources.Load<Sprite>("Image/UI/UNKNOWN_ICON");
    }

    private void InitializePopups()
    {
        canvas = GameObject.Find("Canvas_Overlay");
        optionPopup = GameObject.Find("Option_canvas");
        chessPopup = canvas.transform.Find("PopUp_Chess_Canvas").gameObject;
    }

    private void InitializeDeckUI()
    {
        mapDeckUI = canvas.transform.Find("MapDeckUI_Canvas").gameObject;
        mapScrollRect = mapDeckUI.transform.Find("MapDeckScrollRect").GetComponent<ScrollRect>();
        mapDeckContent = mapScrollRect.transform.Find("Viewport/MapDeckContent").gameObject;

        if (mapScrollRect != null)
        {
            mapScrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            mapDeckHoverLayer = mapScrollRect.transform.Find("Viewport/HoverLayer");
        }
    }

    public override void ShowStageInfo(StageNodeType type)
    {
        stageIcon.sprite = GetStageIcon(type);
        stageText.text = GetStageText(type);
        stageIcon.enabled = true;
        stageText.enabled = true;
    }

    public void HideStageInfo()
    {
        stageIcon.enabled = false;
        stageText.enabled = false;
    }

    private Sprite GetStageIcon(StageNodeType type) => type switch
    {
        StageNodeType.NormalBattle => iconBattle,
        StageNodeType.EliteBattle => iconElite,
        StageNodeType.BossBattle => iconBoss,
        StageNodeType.Rest => iconRest,
        StageNodeType.Treasure => iconTreasure,
        _ => iconUnknown
    };

    private string GetStageText(StageNodeType type) => type switch
    {
        StageNodeType.NormalBattle or StageNodeType.EliteBattle or StageNodeType.BossBattle
            => "당신의 두려움을 마주하세요!",
        StageNodeType.Rest => "잠깐 쉬고 가는 것도 괜찮을지도..?",
        StageNodeType.Treasure => "어떤 보상이 기다릴까요?",
        StageNodeType.Unknown => "무엇이 나올지 아무도 모릅니다!",
        _ => "???"
    };

    private void ClearMapDeckContent()
    {
        foreach (Transform child in mapDeckContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void LoadDeck()
    {
        ClearMapDeckContent();

        foreach (CardData data in BattleCardManager.instance.GetDeckData())
        {
            GameObject currentCard = Instantiate(cardPrefab);
            currentCard.GetComponent<Card>().SetData(data);
            currentCard.transform.SetParent(mapDeckContent.transform);
            currentCard.GetComponent<Card>().cardZone = CardZone.MapDeck;

            string path = data.image;
            currentCard.GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
            currentCard.SetActive(true);
        }
    }

    private void OnScrollValueChanged(Vector2 scrollPos)
    {
        if (Card.GetCurrentHover() != null && Card.currentCard != null)
        {
            Card.GetCurrentHover().transform.position = Card.currentCard.transform.position;
        }
    }

    public void OnMapDeckButtonClicked() => mapDeckUI.SetActive(true);
    public void OnOptionButtonClicked() => TogglePopup(optionPopup);
    public void OnChessButtonClicked() => TogglePopup(chessPopup);

    private void TogglePopup(GameObject popup)
    {
        CloseAllPopups();
        popup?.SetActive(true);
        if (popup == chessPopup)
        {
            PossessPiece.UpdatePossessUI();
        }
        isPopupOpen = true;
    }

    public void OnGiveUpButtonClicked()
    {
        MapManager.instance.gameover_gameobj?.SetActive(true);
    }
}
