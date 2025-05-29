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

        // ī������ Ȯ�� (�θ� Card ��ũ��Ʈ�� ���� ���� ����)
        Card cardComp = clickedObject.GetComponent<Card>();
        if (cardComp == null && clickedObject.transform.parent != null)
        {
            cardComp = clickedObject.transform.parent.GetComponent<Card>();
            clickedObject = cardComp?.gameObject; // Ŭ���� ��ü�� ī�� ��ü�� �缳��
        }

        if (cardComp == null)
            return;

        Image cardImage = clickedObject.GetComponent<Image>();
        if (selectedCards.Contains(clickedObject))
        {
            // ���� ���
            selectedCards.Remove(clickedObject);
            if (cardImage != null)
                cardImage.color = Color.white;
        }
        else
        {
            // ����
            selectedCards.Add(clickedObject);
            if (cardImage != null)
                cardImage.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        }

        // 3�� ���õǸ� ���� �� �� ��ȯ
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
        // �����ߴ� ī��� �� ����
        foreach (GameObject card in selectedCards)
        {
            var img = card.GetComponent<Image>();
            if (img != null)
                img.color = Color.white;
        }

        selectedCards.Clear();
    }
}
