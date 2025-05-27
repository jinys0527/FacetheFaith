using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestUIManager : BaseUIManager
{
    [Header("Rest UI")]
    public GameObject upgradePopup;
    public GameObject mapDeckUI;

    [Header("Button")]
    [SerializeField] Button upgradeButton;
    [SerializeField] Button deleteButton;
    [SerializeField] Button[] backButton;

    private GameObject mapDeckContent;
    private ScrollRect mapScrollRect;

    public override void Initialize()
    {
        upgradePopup = GameObject.Find("Upgrade_canavas");

        var canvas = GameObject.Find("Canvas");
        mapDeckUI = canvas.transform.Find("MapDeckUI_Canvas").gameObject;
        mapScrollRect = mapDeckUI.transform.Find("MapDeckScrollRect").GetComponent<ScrollRect>();
        mapDeckContent = mapScrollRect.transform.Find("Viewport/MapDeckContent").gameObject;

        LoadDeck();
        CloseAllPopups();
    }

    public override void SetupButtonEvents()
    {
        upgradeButton?.onClick.AddListener(OnUpgradeBackButtonClicked);
        deleteButton?.onClick.AddListener(OnMapDeckClicked);
        foreach(var back in backButton)
        {
            back?.onClick.AddListener(CloseAllPopups);
        }
    }

    public override void CloseAllPopups()
    {
        upgradePopup?.SetActive(false);
        mapDeckUI?.SetActive(false);
        isPopupOpen = false;
    }

    private void LoadDeck()
    {
        foreach (CardData data in BattleCardManager.BattleCardManagerInstance.GetDeckData())
        {
            var cardPrefab = Resources.Load<GameObject>("CardPrefab");
            GameObject currentCard = Instantiate(cardPrefab);
            currentCard.GetComponent<Card>().SetData(data);
            currentCard.transform.SetParent(mapDeckContent.transform);
            currentCard.GetComponent<Card>().cardZone = CardZone.MapDeck;

            currentCard.GetComponent<Image>().sprite = Resources.Load<Sprite>(data.image);
            currentCard.SetActive(true);
        }
    }

    public void OnUpgradeBackButtonClicked()
    {
        upgradePopup?.SetActive(false);
        isPopupOpen = true;
    }

    public void OnMapDeckClicked()
    {
        mapDeckUI.SetActive(true);
        isPopupOpen = true;
    }
}
