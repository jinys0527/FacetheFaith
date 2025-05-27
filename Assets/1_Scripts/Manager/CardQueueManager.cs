using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardQueueManager : MonoBehaviour
{
    public static CardQueueManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            cardQueue = new Queue<GameObject>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    Queue<GameObject> cardQueue;
    bool isProcessing = false;

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

            yield return StartCoroutine(BattleCardManager.BattleCardManagerInstance.Co_ApplyCard(currentCard));
        }

        isProcessing = false;
    }
}
