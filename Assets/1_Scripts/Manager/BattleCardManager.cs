using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//ī�� ���� �ҷ����� �� m_cardLoader

public class CardEffectProcessor
{

}

public class CardAnimationController
{

}

public class CardUIManager
{

}

public class DeckManager
{ 
}

public class BattleCardManager : CardManager
{
    #region
    [Header("Hand Layout Settings")]
    public float radius = 0;             // ��ġ�� ������
    public float maxAngle = 0;            // ��ü ī�尡 ������ �ִ� ���� (��)
    public float minAngle = 0; // ī�尡 ���� �� �ּ� ����
    public Vector3 handCenter;  // �߽� ��ġ (���� ��ǥ)
    public int idealCardCount = 0;

    [Header("Animation Settings")]
    public float lerpSpeed = 0;           // �ε巯�� �̵� �ӵ�


    public static BattleCardManager BattleCardManagerInstance;
    public GameObject cardPrefab;
    public GameObject cardCanvas;

    public GameObject Deck;
    public GameObject Hand;
    public GameObject Grave;
    public Sprite[] sprites;

    private GameObject currentCard;
    Coroutine drawCoroutine;

    public RectTransform ActiveHandRect;

    private int currentHoverIndex = 0;
    private int lastHoverIndex = 0;
    private int prevSiblingIndex = 0;

    [SerializeField] GraphicRaycaster raycaster;
    PointerEventData pointerData = new PointerEventData(EventSystem.current);
    List<RaycastResult> results = new List<RaycastResult>();

    [Header("Discard Animation Curve")]
    public AnimationCurve discardCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float elapsed = 0f;
    public float handUseDuration = 1f;
    public float discardDuration;

    public Action onDiscardComplete;
    public List<int> cardCategory = new List<int>();

    public bool isDrag = false;
    bool isPlayerTurn = false;
    bool isPossible = false;

    bool isShuffled = false;
    bool isExcluded = false;

    int m_excludeCount = 0;

    public float drawDelay;
    public const int DRAWCOUNT = 5;
    private int discardCount = 0;
    private int discardAfterDrawCount = 0;
    public int costReduction = 0;

    Vector2 animStartPos;

    private List<GameObject> m_deckObjects = new List<GameObject>();
    private List<GameObject> m_graveObjects = new List<GameObject>();
    private List<GameObject> m_handObjects = new List<GameObject>();
    private List<GameObject> m_excludeObjects = new List<GameObject>();

    [SerializeField] GameObject discardUI;

    GameObject hoverImage;
    GameObject clickedObject;

    int count = 0;
    #endregion

    private void OnDestroy()
    {
        Debug.Log("GameManager destroyed");
    }
    public List<GameObject> GetDeckObject()
    {
        return m_deckObjects;
    }

    public List<GameObject> GetGraveObject()
    {
        return m_graveObjects;
    }

    public List<GameObject> GetHandObject()
    {
        return m_handObjects;
    }

    protected override void Awake()
    {
        Debug.Log("Awake");
        #region singleton
        if (BattleCardManagerInstance == null)
        {
            BattleCardManagerInstance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
        else if (BattleCardManagerInstance != this)
        {
            Debug.Log("�� �����");
            Destroy(gameObject);
            return;
        }
        #endregion
        radius = 4000f;
        maxAngle = 15f;
        minAngle = 0.5f;

        handCenter = new Vector3(0, -4640f, 0);
        idealCardCount = 5;
        //M_InitDeck(10); //OnEnable Ȥ�� Awake ���� �� �ʱ�ȭ

        m_handData.Clear();

    }

    private void Start()
    {
        if (GameManager.instance != null)
            if (GameManager.instance.currentState != GameState.Stage_Battle )
            {
                enabled = false;
            }
    }

    private void OnEnable()
    {
        Debug.Log("Ȱ��ȭ ��");
        if (GameManager.instance != null)
            if (GameManager.instance.currentState != GameState.Stage_Battle )
            {
                enabled = false;
            }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log($"�� �ε� �Ϸ�: {scene.name}, �ε� ���: {mode}");
        Debug.Log("���� �̵�");
        if (GameManager.instance != null)
        {
            if (GameManager.instance.currentState != GameState.Stage_Battle )
            {
                cardCanvas = GameObject.Find("Canvas_Overlay");
                base.Awake();
                enabled = false;
                return;
            }
        }


        // ��: �̱��� �ٽ� Ȱ��ȭ
        ActiveHandRect = GameObject.FindWithTag("ActiveHand").GetComponent<RectTransform>();

        pointerData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointerData, results);

        //Debug.Log("ã�� ����");
        cardCanvas = GameObject.Find("Canvas_Overlay");
        raycaster = cardCanvas.GetComponent<GraphicRaycaster>();
        Deck = GameObject.Find("Deck");
        Hand = GameObject.Find("Hand");
        Grave = GameObject.Find("Grave");
        discardUI = cardCanvas.transform.Find("DiscardUI").gameObject;

        base.Awake();

        Shuffle();
        InitDeckVisuals(); //OnEnable Ȥ�� Awake ���� �� �ʱ�ȭ OnEnable �� awake, start�� ����ѵ� ������Ʈ�� Ȱ��ȭ �ɶ����� �����, �̹� Ȱ��ȭ���� �����
    }

    public void StartDiscard(int targetCount, Action callback)
    {
        discardUI.SetActive(true);
        onDiscardComplete = callback;
    }

    public void CardUpdate()
    {
        if (!isDrag)
        {
            ViewMaximum(m_handObjects);
            ArrangeHand();
            ScaleToOrigin();
        }
        UpdateCount();
    }

    private void ClearCardObjects()
    {
        foreach (GameObject obj in m_deckObjects)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in m_handObjects)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in m_graveObjects)
        {
            Destroy(obj);
        }

        m_deckObjects.Clear();
        m_handObjects.Clear();
        m_graveObjects.Clear();
    }

    #region UI
    public void InitDeckVisuals()
    {
        ClearCardObjects();

        foreach (CardData data in m_deckData)
        {
            GameObject gameObject = Instantiate(cardPrefab, cardCanvas.transform);
            gameObject.GetComponent<RectTransform>().anchoredPosition = Deck.GetComponent<RectTransform>().anchoredPosition;

            Card card = gameObject.GetComponent<Card>();

            string path = data.image;
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(path);

            card.SetData(data);

            m_deckCount++;
            m_deckObjects.Add(gameObject);
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region Animation
    public void ArrangeHand()
    {
        int count = m_handObjects.Count;
        if (count == 0) return;

        float dynamicAngle = (count <= 1) ? 0f : Mathf.Lerp(minAngle, maxAngle, Mathf.Clamp01((float)(count - 1) / (idealCardCount - 1)));

        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 0.5f : (float)i / (count - 1); // ī�� 1���� ���� �߾�
            float angle = Mathf.Lerp(-dynamicAngle / 2f, dynamicAngle / 2f, t);
            float rad = angle * Mathf.Deg2Rad;

            Vector3 targetPos = handCenter + new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0) * radius;

            float targetRot = angle;

            RectTransform rect = m_handObjects[i].GetComponent<RectTransform>();

            // �ε巯�� �̵� & ȸ��
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, Time.deltaTime * lerpSpeed);
            Quaternion rot = Quaternion.Euler(0, 0, -targetRot);
            rect.localRotation = Quaternion.Slerp(rect.localRotation, rot, Time.deltaTime * lerpSpeed);

            if (i == currentHoverIndex)
            {
                continue;
            }

            // UI ���� Z���� (������)
            m_handObjects[i].transform.SetSiblingIndex(i + 9);
        }
    }

    public void ViewMaximum(List<GameObject> cards) // ���� �˾� ������ �� ���� ������ (GameOver,Clear�˾�)
    {
        if (!BaseUIManager.Instance.isPopupOpen)
        {
            // 1.������ �̺�Ʈ ����
            pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            // 2. ��� ����Ʈ �ʱ�ȭ
            results.Clear();

            // 3. ����ĳ��Ʈ ����
            if (raycaster != null)
            {
                raycaster.Raycast(pointerData, results);
                bool found = false;

                // 4. ���� ó��
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.CompareTag("Card"))
                    {
                        int i = 0;
                        foreach (GameObject card in m_handObjects)
                        {
                            if (card == result.gameObject)
                            {
                                currentHoverIndex = i;
                                found = true;
                                break;
                            }
                            i++;
                        }
                        if (found)
                        {
                            break;
                        }
                    }
                }

                UpdateHandCenter(found);

                if (!found)
                {
                    currentHoverIndex = -1;
                }

                ApplyCardHoverEffect();
            }
        }
    }

    private void UpdateHandCenter(bool isHovering)
    {
        handCenter = isHovering ? new Vector3(0, -4350f, 0) : new Vector3(0, -4590f, 0);
    }

    private void ApplyCardHoverEffect()
    {
        if (currentHoverIndex != lastHoverIndex)
        {
            // ���� ī�� ������� ����
            if (lastHoverIndex != -1 && lastHoverIndex < m_handObjects.Count)
            {
                var lastCard = m_handObjects[lastHoverIndex];
                lastCard.transform.localScale = Vector3.one;
                lastCard.GetComponent<RectTransform>().anchoredPosition *= new Vector2(1, 1.5f);
                lastCard.transform.SetSiblingIndex(prevSiblingIndex);
            }

            // ���� ī�� Ȯ�� �� ��ġ ����
            if (currentHoverIndex != -1 && currentHoverIndex < m_handObjects.Count)
            {
                var currentCard = m_handObjects[currentHoverIndex];
                currentCard.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                currentCard.GetComponent<RectTransform>().anchoredPosition *= new Vector2(1, 0.66f);
                prevSiblingIndex = currentCard.transform.GetSiblingIndex();
                currentCard.transform.SetAsLastSibling();
            }

            lastHoverIndex = currentHoverIndex;
        }
    }

    IEnumerator DiscardAnim(GameObject card)
    {
        elapsed = 0;

        RectTransform rect = card.GetComponent<RectTransform>();
        Vector2 startPos = rect.anchoredPosition;
        Vector2 targetPos = Grave.GetComponent<RectTransform>().anchoredPosition;


        while (elapsed < discardDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / discardDuration);
            float curveT = discardCurve.Evaluate(t);
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, curveT);

            yield return null;
        }

        rect.anchoredPosition = targetPos;
    }

    IEnumerator HandUseAnim(GameObject card)
    {
        elapsed = 0;

        RectTransform rect = card.GetComponent<RectTransform>();
        Vector2 startPos = animStartPos;
        Vector2 targetPos = Grave.GetComponent<RectTransform>().anchoredPosition;


        while (elapsed < handUseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / handUseDuration);
            // ������ ��ġ (X, Y ��� ���� ����)
            Vector2 linearPos = Vector2.Lerp(startPos, targetPos, t);

            // � ȿ���� �ֱ� ���� ���� (������ó�� ���� ��¦ Ƣ���ٰ� ������)
            float height = 100f; // �������� ����. �� Ű��� �� Ʀ
            float curveOffset = 4f * height * t * (1 - t); // t(1-t) ������ ������

            // ���� ��ġ�� � ������ �߰� (Y�࿡�� ����)
            linearPos.y += curveOffset;

            rect.anchoredPosition = linearPos;

            yield return null;
        }

        rect.anchoredPosition = targetPos;
    }

    IEnumerator CoDiscardWaitAnim(GameObject card)
    {
        yield return StartCoroutine(DiscardAnim(card));
        card.SetActive(false);
        yield return null;
        BaseUIManager.Instance.UpdateCards();
    }

    public void StartDiscardWaitAnim(GameObject card)
    {
        StartCoroutine(CoDiscardWaitAnim(card));
    }

    IEnumerator CoHandUseWaitAnim(GameObject card)
    {
        yield return StartCoroutine(HandUseAnim(card));
        card.SetActive(false);
        yield return null;
        BaseUIManager.Instance.UpdateCards();
    }

    public void StartHandUseWaitAnim(GameObject card)
    {
        StartCoroutine(CoHandUseWaitAnim(card));
    }
    #endregion

    public void Draw(int num)
    {
        if (GameManager.instance.currentState != GameState.Stage_Battle)
            return;

        base.DrawData(num);

        if (m_deckObjects.Count == 0 && m_graveObjects.Count > 0)
        {
            Shuffle();
            isShuffled = true;
            return;
        }

        SoundManager.Instance.PlaySFX(e_SFXname.s_Card_Draw);

        while (num > 0 && m_deckObjects.Count > 0)
        {
            GameObject cardObject = m_deckObjects[0];
            m_deckObjects.RemoveAt(0);

            if (!cardObject.activeSelf)
            {
                cardObject.SetActive(true);
            }

            RectTransform rect = cardObject.GetComponent<RectTransform>();
            RectTransform hand = Hand.GetComponent<RectTransform>();
            rect.anchoredPosition = hand.anchoredPosition;

            m_handObjects.Add(cardObject);

            m_deckCount--;
            m_handCount++;
            num--;
        }
    }

    public void StartCo_Draw(int drawCount)
    {
        if (drawCoroutine == null)
        {
            drawCoroutine = StartCoroutine(Co_Draw(drawCount));
        }
    }

    IEnumerator Co_Draw(int drawCount)
    {
        for (int i = 0; i < drawCount; i++)
        {
            Draw(1);
            yield return new WaitForSeconds(drawDelay);
        }
        if (isShuffled)
        {
            Draw(1);
            isShuffled = false;
        }
        drawCoroutine = null;
        BaseUIManager.Instance.UpdateCards();
    }

    public void ScaleToOrigin()
    {
        foreach (var card in m_graveObjects)
        {
            card.GetComponent<RectTransform>().anchoredPosition = new Vector2(Deck.GetComponent<RectTransform>().anchoredPosition.x, Deck.GetComponent<RectTransform>().anchoredPosition.y - 500);
            card.SetActive(true);
            card.transform.localScale = Vector3.one;
        }

        foreach (var card in m_deckObjects)
        {
            card.GetComponent<RectTransform>().anchoredPosition = new Vector2(Deck.GetComponent<RectTransform>().anchoredPosition.x, Deck.GetComponent<RectTransform>().anchoredPosition.y - 500);
            card.SetActive(true);
            card.transform.localScale = Vector3.one;
        }

        foreach (var card in m_excludeObjects)
        {
            card.GetComponent<RectTransform>().anchoredPosition = new Vector2(Deck.GetComponent<RectTransform>().anchoredPosition.x, Deck.GetComponent<RectTransform>().anchoredPosition.y - 500);
            card.SetActive(true);
            card.transform.localScale = Vector3.one;
        }
    }

    public void Shuffle()
    {
        ShuffleData();

        for (int i = 0; i < m_graveObjects.Count; i++)
        {
            GameObject card = m_graveObjects[i];
            m_deckObjects.Add(card);
            RectTransform rect = card.GetComponent<RectTransform>();
            RectTransform deck = Deck.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(deck.anchoredPosition.x, deck.anchoredPosition.y - 500);

            card.SetActive(true);
            card.transform.localScale = Vector3.one;
        }

        m_deckCount += m_graveCount;
        m_graveCount = 0;

        m_graveObjects.Clear();

        // Fisher-Yates shuffle
        int n = m_deckObjects.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            (m_deckObjects[n], m_deckObjects[k]) = (m_deckObjects[k], m_deckObjects[n]);
        }
    }

    public void Discard()
    {
        foreach (GameObject card in m_handObjects)
        {
            m_handCount--;
            m_graveCount++;
            m_graveObjects.Add(card);
            StartDiscardWaitAnim(card);
        }
        m_handObjects.Clear();
    }

    public void HandUse(GameObject card)
    {
        if (PlayerManager.instance.cost >= card.GetComponent<Card>().cardData.cost || discardCount > 0)
        {
            CardQueueManager.instance.EnqueueCard(card);
        }
    }

    public IEnumerator Co_ApplyCard(GameObject card)
    {
        CardData cardData = card.GetComponent<Card>().cardData;

        if (discardCount == 0)
        {
            if (cardData.piece != "")
            {
                string[] applyPieces = cardData.piece.Split(',');

                for (int i = 0; i < applyPieces.Length; i++)
                {
                    PieceVariant variant = (PieceVariant)int.Parse(applyPieces[i]);
                    yield return StartCoroutine(Co_ApplyToPiece(variant, card));
                }
            }
            else
            {
                ApplyEffect(card);
            }

            int currentCost = cardData.cost;

            if (cardCategory.Count > 0)
            {
                foreach (int category in cardCategory)
                {
                    if (cardData.category == category)
                    {
                        currentCost = cardData.cost - costReduction;
                    }
                }
            }

            if (currentCost < 0)
            {
                currentCost = 0;
            }

            PlayerManager.instance.cost -= currentCost;
        }
        else
        {
            discardCount--;
            if (discardCount == 0)
            {
                discardUI.SetActive(false);
                onDiscardComplete?.Invoke();
            }
        }

        //yield return StartCoroutine(PlayCardUseWithOptionalBuff(card));

        SoundManager.Instance.PlaySFX(e_SFXname.s_Card_Use);
        if (card.GetComponent<Card>().cardData.category == 0)
        {
            SoundManager.Instance.PlaySFX(e_SFXname.s_Card_Buff);
        }

        yield return new WaitForSeconds(0.1f);

        if (isPossible && !isExcluded)
        {
            StartHandUseWaitAnim(card);
            m_handObjects.Remove(card);
            m_graveObjects.Add(card);
            m_graveCount++;
            m_handCount--;
        }

        if (isExcluded)
        {
            m_handObjects.Remove(card);
            m_excludeObjects.Add(card);
            m_handCount--;
            m_excludeCount++;
            isExcluded = false;
        }
    }

    public IEnumerator Co_ApplyToPiece(PieceVariant pieceVariant, GameObject card)
    {
        foreach (Piece piece in BattlePieceManager.instance.pieces)
        {
            if (piece.pieceVariant == pieceVariant && !piece.GetIsDistort())
            {
                ApplyEffect(piece, card);

                yield return null;
                BaseUIManager.Instance.UpdateBattleUIs();
            }
        }
    }

    #region Effect
    public void ApplyEffect(GameObject card)
    {
        isPossible = true;
        string effect = card.GetComponent<Card>().cardData.effect;

        if (effect.Contains("����������"))
        {
            var numberStr = Regex.Matches(effect, @"(\d+)��");
            discardCount = int.Parse(numberStr[0].Groups[1].Value);
            discardAfterDrawCount = int.Parse(numberStr[1].Groups[1].Value);

            if (discardCount < m_handObjects.Count)
            {
                StartDiscard(discardCount, () =>
                {
                    if (effect.Contains("��ο�"))
                    {
                        StartCo_Draw(discardAfterDrawCount);
                    }
                });
            }
            else
            {
                isPossible = false;
                discardCount = 0;
                discardAfterDrawCount = 0;
                PlayerManager.instance.cost += card.GetComponent<Card>().cardData.cost;
            }
        }
        if (effect.Contains("ȸ��"))
        {
            if (effect.Contains("�ڽ�Ʈ"))
            {
                string numberStr = Regex.Match(effect, @"(\d+)ȸ��").Groups[1].Value;
                PlayerManager.instance.cost += int.Parse(numberStr);
                if (PlayerManager.instance.cost > PlayerManager.instance.maxCost)
                {
                    PlayerManager.instance.cost = PlayerManager.instance.maxCost;
                }
            }
        }
        if (!effect.Contains("������") && effect.Contains("��ο�"))
        {
            string numberStr = Regex.Match(effect, @"(\d+)��").Groups[1].Value;
            StartCo_Draw(int.Parse(numberStr));
        }
        if (effect.Contains("�ڽ�Ʈ-"))
        {
            if (effect.Contains("����"))
            {
                cardCategory.Add(0);
            }
            if (effect.Contains("���"))
            {
                cardCategory.Add(1);
            }
            if (effect.Contains("��ƿ"))
            {
                cardCategory.Add(2);
            }
            string numberStr = Regex.Match(effect, @"-(\d+)").Groups[1].Value;
            costReduction = int.Parse(numberStr);
        }
        if (effect.Contains("�ѷ�"))
        {
            if (effect.Contains("����"))
            {
                string numberStr = Regex.Match(effect, @"(\d+)(��|)������").Groups[1].Value;
                int additionalCost = int.Parse(numberStr) - PlayerManager.instance.maxCost;
                if (additionalCost > 0)
                {
                    PlayerManager.instance.cost += additionalCost;
                    PlayerManager.instance.maxCost += additionalCost;
                }
                isExcluded = true;
            }
        }
        if (effect.Contains("�̿�"))
        {
            PlayerManager.instance.carryOverCost = PlayerManager.instance.cost;
            BattleManager.BattleManagerInstance.SetState(eBattleState.MonsterAttack);
        }
    }

    public void ApplyEffect(Piece piece, GameObject card)
    {
        isPossible = true;
        string effect = card.GetComponent<Card>().cardData.effect;
        PlayerManager.instance.carryOverCost = 0;
        float product = 1f;
        float sum = 0f;

        if (effect.Contains("���ݷ�x"))
        {
            if (effect.Contains("��ġ�ȱ⹰�Ǽ�"))
            {
                int count = 0;
                foreach (Piece currentPiece in BattlePieceManager.instance.pieces)
                {
                    if (currentPiece.GetIsAlive())
                    {
                        count++;
                    }
                }
                product = count;
                piece.AddDamageModifier(0f, product);
            }
            else
            {
                string numberStr = Regex.Match(effect, @"x(\d+)").Groups[1].Value;
                product = int.Parse(numberStr);
                piece.AddDamageModifier(0f, product);
            }
        }
        if (effect.Contains("���ݷ�+"))
        {
            string numberStr = Regex.Match(effect, @"\+(\d+)").Groups[1].Value;
            sum = int.Parse(numberStr);
            piece.AddDamageModifier(sum, 1f);
        }
        if (effect.Contains("����Ƚ��"))
        {
            string numberStr = Regex.Match(effect, @"x(\d+)").Groups[1].Value;
            piece.attackCount *= int.Parse(numberStr);
        }
        if (effect.Contains("�鿪"))
        {
            if (effect.Contains("������"))
            {
                piece.isDamageImmune = true;
            }
            if (effect.Contains("�����̻�"))
            {
                piece.isStatusImmune = true;
            }
        }
        if (effect.Contains("ȸ��"))
        {
            if (effect.Contains("�����̻�"))
            {
                piece.SetStatusEffect(eStatusEffectType.None);
            }
        }

        piece.currentDamage = piece.currentDamage * product + sum;
    }
    #endregion

    void UpdateCount()
    {
        TextMeshProUGUI deckText = Deck.GetComponentInChildren<TextMeshProUGUI>();
        if (deckText != null)
        {
            deckText.text = m_deckCount.ToString();
        }

        TextMeshProUGUI graveText = Grave.GetComponentInChildren<TextMeshProUGUI>();
        if (graveText != null)
        {
            graveText.text = m_graveCount.ToString();
        }
    }


    

    public void FinishBattle()
    {
        int handCount = m_handObjects.Count;
        int graveCount = m_graveObjects.Count;

        for (int i = 0; i < handCount; i++)
        {
            currentCard = m_handObjects[0];
            m_deckObjects.Add(currentCard);
            m_handObjects.RemoveAt(0);
        }
        for (int i = 0; i < graveCount; i++)
        {
            currentCard = m_graveObjects[0];
            m_deckObjects.Add(currentCard);
            m_graveObjects.RemoveAt(0);
        }
        for (int i = 0; i < m_excludeObjects.Count; i++)
        {
            currentCard = m_excludeObjects[0];
            m_deckObjects.Add(currentCard);
            m_excludeObjects.RemoveAt(0);
        }

        m_deckCount = m_deckObjects.Count;
        m_handCount = 0;
        m_graveCount = 0;

        m_handData.Clear();
        m_graveData.Clear();
    }

    public void OnBeginDragEvent(GameObject card)
    {
        if (!(card.transform.parent.gameObject.name == "DeckContent") && !(card.transform.parent.gameObject.name == "GraveContent"))
        {
            isDrag = true;
            isPlayerTurn = BattleManager.BattleManagerInstance.GetIsPlayerTurn();
            card.GetComponent<CanvasGroup>().blocksRaycasts = false; // �巡�� �߿� �ٸ� UI�� Raycast�� ���� �� �ֵ��� ��Ȱ��ȭ
        }
    }

    public void OnDragEvent(GameObject card)
    {
        if (!(card.transform.parent.gameObject.name == "DeckContent") && !(card.transform.parent.gameObject.name == "GraveContent"))
        {
            // �巡�� �� ��ġ ����
            Vector2 mousePos = Input.mousePosition;
            mousePos.x -= 960f;
            mousePos.y -= 540f;
            card.GetComponent<RectTransform>().anchoredPosition = mousePos;
        }
    }

    public void OnEndDragEvent(GameObject card)
    {
        if (!(card.transform.parent.gameObject.name == "DeckContent") && !(card.transform.parent.gameObject.name == "GraveContent"))
        {
            isDrag = false;
            if (lastHoverIndex != -1 && lastHoverIndex < m_handObjects.Count)
            {
                m_handObjects[lastHoverIndex].transform.localScale = Vector3.one;
                m_handObjects[lastHoverIndex].GetComponent<RectTransform>().anchoredPosition *= new Vector2(1, 1.5f);
                m_handObjects[lastHoverIndex].transform.SetSiblingIndex(prevSiblingIndex);
            }
            card.GetComponent<CanvasGroup>().blocksRaycasts = true;
            RectTransform rectTransform = ActiveHandRect.GetComponent<RectTransform>();
            Vector2 mousePos = Input.mousePosition;
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePos) && BattleManager.BattleManagerInstance.GetIsPlayerTurn())
            {
                Vector2 localStartPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    Grave.transform.parent as RectTransform,
                    Input.mousePosition,
                    null,
                    out localStartPos
                );
                animStartPos = localStartPos;
                HandUse(card);
            }
        }
    }

    

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.instance.currentState != GameState.Stage_Battle)
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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.instance.currentState != GameState.Stage_Battle)
        {
            Debug.Log("Ŭ����");
            if (clickedObject != null)
            {
                if (clickedObject.GetComponent<Card>() != null)
                {
                    count++;
                    BattleCardManager.BattleCardManagerInstance.initDeckIndices.Remove(clickedObject.GetComponent<Card>().cardData.index);

                    Destroy(clickedObject);
                    if (transform.GetChild(1).childCount != 0)
                    {
                        hoverImage = transform.GetChild(1).GetChild(0).gameObject;
                    }
                    Destroy(hoverImage);

                    if (count > 2)
                    {
                        SceneChageManager.Instance.ChangeGameState(GameState.Map);
                    }
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.instance.currentState != GameState.Stage_Battle)
        {
            Debug.Log(transform.GetChild(1).name);
            if (transform.GetChild(1).childCount != 0)
            {
                Debug.Log(transform.GetChild(1).GetChild(0).name);
                hoverImage = transform.GetChild(1).GetChild(0).gameObject;
                hoverImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }
    }

}
