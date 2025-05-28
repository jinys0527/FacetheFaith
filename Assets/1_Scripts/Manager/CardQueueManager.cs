using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardQueueManager : MonoBehaviour
{
    public static CardQueueManager instance;

    Queue<GameObject> cardQueue = new();
    bool isProcessing = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnqueueCard(GameObject card)
    {
        cardQueue.Enqueue(card);
        if(!isProcessing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (cardQueue.Count > 0)
        {
            GameObject currentCard = cardQueue.Dequeue();

            yield return StartCoroutine(BattleCardManager.instance.Co_ApplyCard(currentCard));
        }

        isProcessing = false;
    }
}
