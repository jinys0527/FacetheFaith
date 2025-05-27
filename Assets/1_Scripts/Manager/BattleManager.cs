using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using static Piece;


public enum eBattleState
{
    LayOut = 0,
    ShowPattern,
    Draw,
    PlayerTurn,
    PlayerEnd,
    MonsterAttack,
    PlayerAttack,
    StageClear,
    GameOver
};

public class BattleManager : MonoBehaviour
{
    public static BattleManager BattleManagerInstance;

    [SerializeField] bool CanLayOut = false;
    [SerializeField] bool isPlayerTurn = false;
    public GameObject monster;
    Monster monsterScript;
    [SerializeField] eBattleState currentState;

    [SerializeField] GameObject RewardPanel;


    public float totalDamage = 0f;
    public float currentDamage = 0f;
    int stageNum = 1;
    int type;
    bool isChangingState = false;

    public GameObject gameover_gameobj;
    public GameObject gameclear_gameobj;

    public Sprite bossBackground;

    private void Awake()
    {
        RewardPanel = GameObject.Find("RewardPanel");
        gameover_gameobj = GameObject.Find("GameOverPopup_canvas");
        gameclear_gameobj = GameObject.Find("GameClearPopup_canvas");

        gameover_gameobj.SetActive(false);
        gameclear_gameobj.SetActive(false);
        RewardPanel.SetActive(false);

        if (BattleManagerInstance == null)
        {
            BattleManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        monster = GameObject.Find("Monster");
        monsterScript = monster.GetComponent<Monster>();
        if (MapManager.instance != null)
        {
            stageNum = MapManager.instance.GetStageNum();
        }
        else
        {  // 디버깅용
            stageNum = 3;
        }

        switch (stageNum)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                type = Random.Range(0, 2);
                if (type == 0)
                {
                    monsterScript.SetType(eMonsterType.Betrayal, stageNum);
                }
                else
                {
                    monsterScript.SetType(eMonsterType.Mockery, stageNum);
                }
                break;
            case 5:
                monsterScript.SetType(eMonsterType.Elite, stageNum);
                break;
            case 6:
            case 8:
            case 9:
                type = Random.Range(0, 2);
                if (type == 0)
                {
                    monsterScript.SetType(eMonsterType.Plague, stageNum);
                }
                else
                {
                    monsterScript.SetType(eMonsterType.Greed, stageNum);
                }
                break;
            case 10:
                monsterScript.SetType(eMonsterType.Boss, stageNum);
                GameObject.Find("Canvas_World").transform.GetChild(1).gameObject.GetComponent<Image>().sprite = bossBackground;
                break;
        }
        SetState(eBattleState.LayOut);
        SoundManager.Instance.PlaySFX(e_SFXname.s_Enemy_Appears);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetState(++currentState);
        }
    }
    public bool GetCanLayOut()
    {
        return CanLayOut;
    }

    public bool GetIsPlayerTurn()
    {
        return isPlayerTurn;
    }

    public void SetState(eBattleState state)
    {
        currentState = state;
        StateUpdate();
    }

    public eBattleState GetState()
    {
        return currentState;
    }

    public void NextState()
    {
        if(!BaseUIManager.Instance.isPopupOpen)
        {
            SoundManager.Instance.PlaySFX(e_SFXname.s_Turn);
            if (isChangingState) return;


            if (currentState == eBattleState.LayOut)
            {
                if (BattlePieceManager.instance.pieces.Count < 1) return;
                BaseUIManager.Instance.HidePlaceUI();
                BattlePieceManager.instance.DisablePieces();
                BaseUIManager.Instance.OpenBattleUIs();
            }

            StartCoroutine(NextStateCoroutine());
        }
    }

    IEnumerator NextStateCoroutine()
    {
        isChangingState = true;
        SetState(++currentState);
        yield return null;
        isChangingState = false;
    }

    void StateUpdate()
    {
        switch (currentState)
        {
            case eBattleState.LayOut:   //배치 완료 UI
                CanLayOut = true;
                PlayerManager.instance.cost = PlayerManager.MAXCOST;
                PlayerManager.instance.maxCost = PlayerManager.MAXCOST;
                break;
            case eBattleState.ShowPattern:
                BattleCardManager.BattleCardManagerInstance.cardCategory.Clear();
                BattleCardManager.BattleCardManagerInstance.costReduction = 0;
                PlayerManager.instance.cost = PlayerManager.instance.maxCost + PlayerManager.instance.carryOverCost;
                if (PlayerManager.instance.carryOverCost != 0)
                {
                    PlayerManager.instance.carryOverCost = 0;
                }
                foreach (Piece piece in BattlePieceManager.instance.pieces)
                {
                    piece.SetStatusEffect(eStatusEffectType.None);      // 턴별로 기물 상태이상 풀리므로 풀어주기
                    piece.ClearAdditionalEffect();
                    piece.ClearDamageModifiers();
                    piece.currentDamage = piece.GetAtk();
                }
                BossPatternManager.instance.Play(BossPatternManager.instance.GetIndex());
                CanLayOut = false;
                SetState(eBattleState.Draw);
                break;
            case eBattleState.Draw:
                BattleCardManager.BattleCardManagerInstance.StartCo_Draw(BattleCardManager.DRAWCOUNT);
                SetState(eBattleState.PlayerTurn);
                break;
            case eBattleState.PlayerTurn: //이때 플레이어 턴 UI  활성화
                TurnPopupManager.Instance.ShowPlayerTurn();
                BattlePieceManager.instance.AblePieces();
                isPlayerTurn = true;
                break;
            case eBattleState.PlayerEnd:  //턴 종료 UI
                isPlayerTurn = false;
                BattlePieceManager.instance.DisablePieces();
                SetState(eBattleState.MonsterAttack);
                break;
            case eBattleState.MonsterAttack: // 이때 몬스터 턴 UI 활성화
                StartCoroutine(HandleEnemyTurn()); // 몬스터 행동이 패턴이 공격 >> 패턴 보여주기 순서로 진행되도록 코루틴으로 진행함
                SoundManager.Instance.PlaySFX(e_SFXname.s_Enemy_Attack);
                break;
            case eBattleState.PlayerAttack:
                int alivePieceCount = 0;
                foreach (Piece piece in BattlePieceManager.instance.pieces)
                {
                    if (piece.GetIsAlive())
                    {
                        alivePieceCount++;
                    }
                }

                if (alivePieceCount > 0 && monsterScript.GetIsAlive())
                {
                    PlayerAttack();
                    
                }
                else if (alivePieceCount == 0)
                {
                    SetState(eBattleState.GameOver);
                }

                break;
            case eBattleState.StageClear:
                print("StageClear");
                SoundManager.Instance.PlaySFX(e_SFXname.s_Clear);
                if (MapManager.instance == null) stageNum = 1; // 디버깅용
                else stageNum = MapManager.instance.GetStageNum();

                BattleCardManager.BattleCardManagerInstance.FinishBattle();

                if (stageNum == 10)    //보스 스테이지인 경우
                {
                    gameclear_gameobj.SetActive(true);

                    StopAllCoroutines();
                }
                else
                { //여기수정해야함

                    RewardPanel.SetActive(true);
                    StopAllCoroutines();
                }

                //맵으로
                break;
            case eBattleState.GameOver:
                Debug.Log("GameOver");
                SoundManager.Instance.PlaySFX(e_SFXname.s_Death);

                // Canvas findcanvas = gameobj.GetComponent<Canvas>();

                gameover_gameobj.SetActive(true);

                //SceneChageManager.Instance.ChangeGameState(GameState.Title);
                //게임 오버 처리
                break;
        }
    }

    private IEnumerator HandleEnemyTurn()
    {
        // 1. 팝업
        yield return TurnPopupManager.Instance.FadeSequence(TurnPopupManager.Instance.enemyTurn_image);

        // 2. 몬스터 행동
        BattlePieceManager.instance.DisableStatusPieces();
        monsterScript.Attack(BossPatternManager.instance.nodeList);

        // 여기에 패턴 보여주는거 추가하면댐

        // 3. 다음 상태 전환
        SetState(eBattleState.PlayerAttack);
    }

    public void PlayerAttack()
    {
        StartCoroutine(Co_PlayerAttack());
    }

    IEnumerator Co_PlayerAttack()
    {
        foreach (Piece piece in BattlePieceManager.instance.pieces)
        {
            if (!piece.GetCanAttack())
                continue;
            float baseAtk = piece.GetAtk();

            for (int i = 0; i < piece.attackCount; i++)
            {
                float modifiedAtk = baseAtk;

                foreach (var mod in piece.GetDamageModifiers())
                {
                    modifiedAtk = modifiedAtk * mod.product + mod.sum;
                }
                currentDamage = modifiedAtk;
                monsterScript.SetHp(monsterScript.GetHp() - currentDamage);
                BaseUIManager.Instance.DamageMonster();
                totalDamage += currentDamage;
                Animator animator = piece.gameObject.GetComponent<Animator>();
                if (animator != null)
                    animator.SetTrigger("Attack");

                switch(piece.pieceVariant)
                {
                    case PieceVariant.Pawn:
                        SoundManager.Instance.PlaySFX(e_SFXname.s_Piece_Effect_Pawn);
                        break;
                    case PieceVariant.Knight:
                        SoundManager.Instance.PlaySFX(e_SFXname.s_Piece_Effect_Knight);
                        break;
                    case PieceVariant.Rook:
                        SoundManager.Instance.PlaySFX(e_SFXname.s_Piece_Effect_Rook);
                        break;
                    case PieceVariant.Bishop:
                        SoundManager.Instance.PlaySFX(e_SFXname.s_Piece_Effect_Bishop);
                        break;
                    case PieceVariant.Queen:
                        SoundManager.Instance.PlaySFX(e_SFXname.s_Piece_Effect_Queen);
                        break;  
                }

                yield return null; // 상태 전환 대기

                GameObject go = GameObject.FindGameObjectWithTag("Monster");
                GameObject effect;

                switch (piece.pieceVariant)
                {
                    case PieceVariant.Pawn:
                        effect = Instantiate(piece.attackEffect[(int)piece.pieceVariant]);
                        effect.transform.position = effect.transform.position + Vector3.up * 0.5f;
                        break;
                    case PieceVariant.Knight:
                        effect = Instantiate(piece.attackEffect[(int)piece.pieceVariant]);
                        effect.transform.position = effect.transform.position + Vector3.up * 0.5f;
                        break;
                    default:
                        effect = Instantiate(piece.attackEffect[(int)piece.pieceVariant], go.transform);
                        effect.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + 9f, go.transform.position.z);
                        break;
                }


                Vector3 direction = go.transform.position - piece.transform.position;
                if (direction != Vector3.zero)
                {
                    effect.transform.rotation = Quaternion.LookRotation(direction);
                }

                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                float waitTime = stateInfo.length;
                yield return new WaitForSeconds(waitTime);
            }
        }

        if (monsterScript.GetHp() <= 0f)
        {
            monsterScript.SetHp(0f);
            monsterScript.SetDead();
            SetState(eBattleState.StageClear);
            SoundManager.Instance.PlaySFX(e_SFXname.s_Enemy_Die);
        }
        else
        {
            BattleCardManager.BattleCardManagerInstance.Discard();
            SetState(eBattleState.ShowPattern);
        }
    }
}
