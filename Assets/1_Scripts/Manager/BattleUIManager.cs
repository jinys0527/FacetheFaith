using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : BaseUIManager
{
    [Header("Battle UI")]
    public GameObject deckUI;
    public GameObject graveUI;

    [Header("버튼")]
    [SerializeField] Button deckButton;
    [SerializeField] Button graveButton;
    [SerializeField] Button[] backButton;
    [SerializeField] Button[] gotoTitleButton;

    [Header("Monster HP")]
    private RectTransform hpRectTransform;
    private RectTransform hpDrainRectTransform;
    private Monster monster;

    [Header("Cost Display")]
    private TextMeshProUGUI currentCostText;
    private TextMeshProUGUI maxCostText;

    private GameObject deckContent, graveContent;
    private ScrollRect deckScrollRect, graveScrollRect;
    private StatusInfoUI statusInfoUI;
    private DamageInfoUI damageInfoUI;
    private GameObject placeUI, battleUIContainer;

    // HP 애니메이션 관련
    private const float MONSTER_HP_UI_RATIO = 989f;
    private bool isBossDamaged = false;
    private float prevWidth = 0, currentWidth = 989, speed = 0;

    [SerializeField] Transform deckHoverLayer;
    public override Transform DeckHoverLayer => deckHoverLayer;

    [SerializeField] Transform graveHoverLayer;
    public override Transform GraveHoverLayer => graveHoverLayer;

    private void Awake()
    {
        BaseUIManager.Instance = this;
    }

    public override void Initialize()
    {
        SetupButtonEvents();
        InitializeBattleUI();
        InitializeMonsterHP();
        InitializeCostDisplay();
        LoadDeck();
        CloseAllPopups();
    }

    public override void SetupButtonEvents()
    {
        deckButton?.onClick.AddListener(OnDeckClicked);
        graveButton?.onClick.AddListener(OnGraveClicked);
        foreach(var back in backButton)
        {
            back?.onClick.AddListener(CloseAllPopups);
        }
        foreach(var title in gotoTitleButton)
        {
            title?.onClick.AddListener(() => Press(GameState.Title));
        }
    }

    public override void CloseAllPopups()
    {
        deckUI?.SetActive(false);
        graveUI?.SetActive(false);
        isPopupOpen = false;
    }

    private void InitializeBattleUI()
    {
        placeUI = GameObject.FindWithTag("PlaceUI");
        battleUIContainer = GameObject.FindWithTag("BattleUI");

        var canvas = GameObject.Find("Canvas");
        deckUI = canvas.transform.Find("DeckUI").gameObject;
        graveUI = canvas.transform.Find("GraveUI").gameObject;

        deckScrollRect = deckUI.transform.Find("DeckScrollRect").GetComponent<ScrollRect>();
        graveScrollRect = graveUI.transform.Find("GraveScrollRect").GetComponent<ScrollRect>();

        deckContent = deckScrollRect.transform.Find("Viewport/DeckContent").gameObject;
        graveContent = graveScrollRect.transform.Find("Viewport/GraveContent").gameObject;

        if (deckScrollRect != null && graveScrollRect != null)
        {
            deckScrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            graveScrollRect.onValueChanged.AddListener(OnScrollValueChanged);

            deckHoverLayer = deckScrollRect.transform.Find("Viewport/HoverLayer");
            graveHoverLayer = graveScrollRect.transform.Find("Viewport/HoverLayer");
        }
    }

    private void InitializeMonsterHP()
    {
        monster = GameObject.Find("Monster")?.GetComponent<Monster>();
        hpRectTransform = GameObject.Find("Boss_Hp")?.GetComponent<RectTransform>();
        hpDrainRectTransform = GameObject.Find("Boss_DrainingHp")?.GetComponent<RectTransform>();
    }

    private void InitializeCostDisplay()
    {
        GameObject currentObj = GameObject.Find("current_cost");
        GameObject maxObj = GameObject.Find("max_cost");

        if (currentObj != null && maxObj != null)
        {
            currentCostText = currentObj.GetComponent<TextMeshProUGUI>();
            maxCostText = maxObj.GetComponent<TextMeshProUGUI>();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (PlayerManager.instance != null && currentCostText != null && maxCostText != null)
        {
            currentCostText.text = PlayerManager.instance.cost.ToString();
            maxCostText.text = PlayerManager.instance.maxCost.ToString();
        }
        UpdateCostDisplay();
        UpdateHPAnimation();
    }

    private void UpdateCostDisplay()
    {
        if (PlayerManager.instance != null && currentCostText != null && maxCostText != null)
        {
            currentCostText.text = PlayerManager.instance.cost.ToString();
            maxCostText.text = PlayerManager.instance.maxCost.ToString();
        }
    }

    private void UpdateHPAnimation()
    {
        // HP 드레인 애니메이션
        if (prevWidth > 0 && hpDrainRectTransform != null)
        {
            Vector2 sizeDelta = hpDrainRectTransform.sizeDelta;
            if (sizeDelta.x >= currentWidth)
                sizeDelta.x -= Time.deltaTime * speed;
            hpDrainRectTransform.sizeDelta = sizeDelta;
        }

        if (isBossDamaged)
        {
            SetWidth(hpDrainRectTransform, prevWidth);
            isBossDamaged = false;
        }
    }

    public override void DamageMonster()
    {
        if (monster == null || hpRectTransform == null) return;

        prevWidth = Mathf.Max(currentWidth, hpDrainRectTransform.sizeDelta.x);
        currentWidth = MONSTER_HP_UI_RATIO * monster.GetHpRatio();
        speed = prevWidth - currentWidth;
        isBossDamaged = true;

        SetWidth(hpRectTransform, currentWidth);

        var hpText = hpRectTransform.parent.GetChild(4).GetComponent<TMP_Text>();
        hpText.text = $"{Mathf.Max(0, monster.GetHp())}/{monster.GetMaxHp()}";
    }

    private void SetWidth(RectTransform rectTransform, float width)
    {
        Vector2 sizeDelta = rectTransform.sizeDelta;
        sizeDelta.x = width;
        rectTransform.sizeDelta = sizeDelta;
    }

    public override void HidePlaceUI()
    {
        if (placeUI != null)
        {
            for (int i = 0; i < placeUI.transform.childCount; i++)
                placeUI.transform.GetChild(i).gameObject.SetActive(false);
            ShowBattleUI();
        }
    }

    public void ShowBattleUI()
    {
        for (int i = 0; i < battleUIContainer.transform.childCount; i++)
            battleUIContainer.transform.GetChild(i).gameObject.SetActive(true);
    }

    public override void UpdateBattleUIs()
    {
        statusInfoUI ??= GameObject.FindWithTag("StatusInfoUI").GetComponent<StatusInfoUI>();
        statusInfoUI?.UpdateStatusInfo();

        damageInfoUI ??= GameObject.FindWithTag("DamageInfoUI").GetComponent<DamageInfoUI>();
        damageInfoUI?.UpdateDamageInfo();
    }

    private void LoadDeck()
    {
        foreach (GameObject card in BattleCardManager.BattleCardManagerInstance.GetDeckObject())
        {
            GameObject currentCard = Instantiate(card);
            currentCard.GetComponent<Card>().cardZone = CardZone.Deck;
            currentCard.transform.SetParent(deckContent.transform);
            currentCard.SetActive(true);
        }
    }

    public override void UpdateCards()
    {
        // 덱 업데이트
        ClearChildren(deckContent);
        foreach (GameObject card in BattleCardManager.BattleCardManagerInstance.GetDeckObject())
        {
            GameObject currentCard = Instantiate(card);
            currentCard.GetComponent<Card>().cardZone = CardZone.Deck;
            currentCard.transform.SetParent(deckContent.transform);
            currentCard.transform.SetSiblingIndex(currentCard.GetComponent<Card>().cardData.index);
            currentCard.SetActive(true);
        }

        // 무덤 업데이트
        ClearChildren(graveContent);
        foreach (GameObject card in BattleCardManager.BattleCardManagerInstance.GetGraveObject())
        {
            GameObject currentCard = Instantiate(card);
            currentCard.GetComponent<Card>().cardZone = CardZone.Grave;
            currentCard.transform.rotation = Quaternion.identity;
            currentCard.transform.SetParent(graveContent.transform);
            currentCard.transform.SetSiblingIndex(currentCard.GetComponent<Card>().cardData.index);
            currentCard.SetActive(true);
        }
    }

    private void ClearChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
            Destroy(child.gameObject);
    }

    private void OnScrollValueChanged(Vector2 scrollPos)
    {
        if (Card.GetCurrentHover() != null && Card.currentCard != null)
            Card.GetCurrentHover().transform.position = Card.currentCard.transform.position;
    }

    public void OnDeckClicked()
    { 
        deckUI.SetActive(true); 
        isPopupOpen = true;
    }

    public void OnGraveClicked()
    {
        graveUI.SetActive(true);
        isPopupOpen = true;
    }

    public override void OpenBattleUIs()
    {
        for (int i = 0; i < battleUIContainer.transform.childCount; i++)
        {
            if (battleUIContainer.transform.GetChild(i).gameObject.GetComponent<ImageClickHandler>() != null)
                battleUIContainer.transform.GetChild(i).gameObject.GetComponent<ImageClickHandler>().Open();
        }

    }

    public void CloseBattleUIs()
    {
        for (int i = 0; i < battleUIContainer.transform.childCount; i++)
        {
            battleUIContainer.transform.GetChild(i).gameObject.GetComponent<ImageClickHandler>().Close();
        }

    }
}
