using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    
    [SerializeField] protected CardTable cardTable;
    public List<int> initDeckIndices = new List<int>();

    protected List<CardData> m_deckData = new List<CardData>();
    protected List<CardData> m_handData = new List<CardData>();
    protected List<CardData> m_graveData = new List<CardData>();

    [SerializeField] protected int m_deckCount = 0;
    [SerializeField] protected int m_handCount = 0;
    [SerializeField] protected int m_graveCount = 0;

    protected virtual void Awake()
    {
        cardTable = Resources.Load<CardTable>($"Data/CardTable");
        InitDeckData();
    }

    public virtual void InitDeckData()
    {
        //!!!!
        //보상방 만들면 initDeckIndices에 얻은 카드 추가해야함.
        //!!!!

        m_deckData.Clear();
        m_deckCount = 0;
        
        foreach (int deckIndex in initDeckIndices)
        {
            CardData data = cardTable.dataTable.Find(x => x.index == deckIndex);
            if (data != null)
            {
                m_deckData.Add(data);
            }
        }
        print("InitDeck");
    }

    public virtual void DrawData(int num)
    {
        while (num > 0 && m_deckCount > 0)
        {
            if (m_deckData.Count == 0 && m_graveData.Count > 0)
            {
                ShuffleData();
            }

            if (m_deckData.Count == 0)
            {
                break;
            }

            m_handData.Add(m_deckData[0]);
            m_deckData.RemoveAt(0);

            num--;
        }
    }

    public virtual void ShuffleData()
    {
        m_deckData.AddRange(m_graveData);
        m_graveData.Clear();
        
        int n = m_deckData.Count;
        while (n > 1) 
        {
            n--; 
            int k = Random.Range(0, n + 1);
            CardData temp = m_deckData[k];
            m_deckData[k] = m_deckData[n];
            m_deckData[n] = temp;
        }
    }

    public List<CardData> GetDeckData() { return m_deckData; }
    public List<CardData> GetHandData() { return m_handData; }
    public List<CardData> GetGraveData() { return m_graveData; }

    public int GetDeckCount() { return m_deckCount; }
    public int GetHandCount() { return m_handCount; }
    public int GetGraveCount() { return m_graveCount; }
}
