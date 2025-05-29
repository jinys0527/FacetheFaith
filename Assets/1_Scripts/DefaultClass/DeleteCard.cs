using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeleteCard : MonoBehaviour, IPointerClickHandler
{
    GameObject hoverImage;
    GameObject clickedObject;

    public static List<GameObject> selectedCards = new List<GameObject>();

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (clickedObject == null)
            return;

        // 카드인지 확인 (부모에 Card 스크립트가 있을 수도 있음)
        Card cardComp = clickedObject.GetComponent<Card>();
        if (cardComp == null && clickedObject.transform.parent != null)
        {
            cardComp = clickedObject.transform.parent.GetComponent<Card>();
            clickedObject = cardComp?.gameObject; // 클릭된 객체를 카드 객체로 재설정
        }

        if (cardComp == null)
            return;

        Image cardImage = clickedObject.GetComponent<Image>();
        if (selectedCards.Contains(clickedObject))
        {
            // 선택 취소
            selectedCards.Remove(clickedObject);
            if (cardImage != null)
                cardImage.color = Color.white;
        }
        else
        {
            // 선택
            selectedCards.Add(clickedObject);
            if (cardImage != null)
                cardImage.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        }

        // 3장 선택되면 삭제 후 씬 전환
        if (selectedCards.Count == 3)
        {
            foreach (GameObject card in selectedCards)
            {
                var comp = card.GetComponent<Card>();
                BattleCardManager.instance.initDeckIndices.Remove(comp.cardData.index);
                Object.Destroy(card);
            }

            selectedCards.Clear();
            SceneChangeManager.Instance.ChangeGameState(GameState.Map);
        }
    }

    public static void DeleteSelectedCards()
    {
        foreach (GameObject card in selectedCards)
        {
            var cardComp = card.GetComponent<Card>();
            BattleCardManager.instance.initDeckIndices.Remove(cardComp.cardData.index);
            Object.Destroy(card);
        }

        selectedCards.Clear();
    }

    public static void CancelDelete()
    {
        // 선택했던 카드들 색 원복
        foreach (GameObject card in selectedCards)
        {
            var img = card.GetComponent<Image>();
            if (img != null)
                img.color = Color.white;
        }

        selectedCards.Clear();
    }
}
