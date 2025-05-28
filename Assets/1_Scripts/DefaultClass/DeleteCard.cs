using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeleteCard : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    GameObject hoverImage;
    GameObject clickedObject;

    public static List<GameObject> selectedCards = new List<GameObject>();

    public void OnPointerDown(PointerEventData eventData)
    {
        clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (clickedObject.GetComponent<Card>() != null)
        {
            if (transform.GetChild(1).childCount != 0)
            {
                hoverImage = transform.GetChild(1).GetChild(0).gameObject;
                hoverImage.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 1);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("클릭됨");
        if (clickedObject != null)
        { 
            if (clickedObject.GetComponent<Card>() != null)
            {
                selectedCards.Add(clickedObject);

                if (transform.GetChild(1).childCount != 0)
                {
                    hoverImage = transform.GetChild(1).GetChild(0).gameObject;
                    hoverImage.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 1);
                }

                if (selectedCards.Count == 3)
                {
                    foreach (GameObject card in selectedCards)
                    {
                        var cardComp = card.GetComponent<Card>();
                        BattleCardManager.instance.initDeckIndices.Remove(cardComp.cardData.index);
                        Destroy(card);
                    }

                    selectedCards.Clear();
                    SceneChangeManager.Instance.ChangeGameState(GameState.Map);
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(transform.GetChild(1).name);
        if (transform.GetChild(1).childCount != 0)
        {
            Debug.Log(transform.GetChild(1).GetChild(0).name);
            hoverImage = transform.GetChild(1).GetChild(0).gameObject;
            hoverImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
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
