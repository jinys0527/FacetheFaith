using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUIManager : MonoBehaviour
{
    public static BaseUIManager Instance { get; protected set; }

    public GameObject cardPrefab;

    public bool isPopupOpen = false;

    public virtual Transform MapDeckHoverLayer => null;
    public virtual Transform DeckHoverLayer => null;
    public virtual Transform GraveHoverLayer => null;

    public virtual void UpdateCards() { }
    public virtual void UpdateBattleUIs() { }
    public virtual void ShowStageInfo(StageNodeType type) { }

    public virtual void OpenBattleUIs() { }
    public virtual void HidePlaceUI() { }
    public virtual void DamageMonster() { }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Start()
    {
        cardPrefab = Resources.Load<GameObject>("Prefabs/P_JYS/card");
        Initialize();
    }

    public abstract void Initialize();
    public abstract void SetupButtonEvents();
    public abstract void CloseAllPopups();

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllPopups();
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Press(GameState gameState)
    {
        if (GameManager.instance.currentState == GameState.Title)
        {
            // 기존 초기화 코드 그대로 
            PlayerManager.instance.ResetPieces();
            PlayerManager.instance.pieceCounts[PieceVariant.Pawn] = 1;
            PlayerManager.instance.pieceCounts[PieceVariant.Knight] = 1;
            PlayerManager.instance.pieceCounts[PieceVariant.Bishop] = 1;

            var list = BattleCardManager.instance.initDeckIndices;
            list.Clear();
            list.AddRange(new[] { 0, 0, 1, 1, 3, 3, 8, 10, 11, 12 });

            SoundManager.Instance.StartFadeOut(SoundManager.Instance.bgmSource, 2.0f);
        }

        isPopupOpen = false;
        /* GameState 를 직접 바꾸는 대신 TransitionManager 에게 넘긴다 */
        SceneChangeManager.Instance.ChangeGameState(gameState);
    }
}
