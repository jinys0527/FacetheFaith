using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public enum CardZone
{
    MapDeck,
    Deck,
    Grave
}

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CardData cardData;
    public CardZone cardZone;

    public float scaleFactor = 1.25f; // 마우스 오버 시 확대할 비율
    private Vector3 originalScale;

    public GameObject cardPrefab;

    public static GameObject currentCard;
    private static GameObject currentHover;

    static public GameObject GetCurrentHover()
    {
        return currentHover;
    }

    private void Start()
    {
        originalScale = GetComponent<RectTransform>().localScale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 오브젝트 위로 올라왔을 때
        currentCard = gameObject;

        Transform hoverLayer = null;

        switch(cardZone)
        {
            case CardZone.MapDeck:
                if(GameManager.instance.CurrentUIManager.MapDeckHoverLayer != null)
                {
                    hoverLayer = GameManager.instance.CurrentUIManager.MapDeckHoverLayer.transform;
                }
                break;
            case CardZone.Deck:
                if(GameManager.instance.CurrentUIManager.DeckHoverLayer != null)
                {
                    hoverLayer = GameManager.instance.CurrentUIManager.DeckHoverLayer.transform;
                }
                break;
            case CardZone.Grave:
                if (GameManager.instance.CurrentUIManager.GraveHoverLayer != null)
                {
                    hoverLayer = GameManager.instance.CurrentUIManager.GraveHoverLayer.transform;
                }
                break;
            default:
                break;
        }

        if (hoverLayer != null)
        {
            currentHover = Instantiate(cardPrefab, hoverLayer);
            currentHover.GetComponent<CanvasGroup>().blocksRaycasts = false;
            currentHover.GetComponent<RectTransform>().sizeDelta = new Vector2(270, 360);

            currentHover.GetComponent<RectTransform>().localScale = originalScale * scaleFactor;

            currentHover.transform.position = transform.position;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(currentHover);
        currentHover = null;
    }

    public void SetData(CardData data)
    {
        cardData = data;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(GameManager.instance.currentState == GameState.Stage_Battle)
        {
            BattleCardManager.instance.OnBeginDragEvent(gameObject);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.instance.currentState == GameState.Stage_Battle)
        {
            BattleCardManager.instance.OnDragEvent(gameObject);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.instance.currentState == GameState.Stage_Battle)
        {
            BattleCardManager.instance.OnEndDragEvent(gameObject);
        }
    }
}