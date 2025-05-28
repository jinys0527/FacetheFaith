using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    
    [SerializeField] protected CardTable cardTable;
    public List<int> initDeckIndices = new List<int>();

    protected List<CardData> m_deckData = new List<CardData>();

    protected virtual void Awake()
    {
        cardTable = Resources.Load<CardTable>($"Data/CardTable");
        InitDeckData();
    }

    public virtual void InitDeckData()
    {
        m_deckData.Clear();
        
        foreach (int deckIndex in initDeckIndices)
        {
            CardData data = cardTable.dataTable.Find(x => x.index == deckIndex);
            if (data != null)
            {
                m_deckData.Add(data);
            }
            else
            {
                Debug.LogWarning($"CardData with index {deckIndex} not found in CardTable.");
            }
        }
    }

    public List<CardData> GetDeckData() => m_deckData; 
}
