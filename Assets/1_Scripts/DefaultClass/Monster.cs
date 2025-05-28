using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;


public enum eMonsterAttackType
{
    Default,    //해당 기물의 체력 -1
    Freeze,     //빙결 다음 턴 해당 기물 이동 불가
    Weak,       //약화 다음 턴 해당 기물의 기본 공격력 * 0.5
    Distort,    //왜곡 다음 턴 해당 기물 카드효과 적용 불가
    Shock,      //감전 다음 턴 해당 기물 자동공격 불가
    Explode,    //폭발 해당 기물 피격 시 해당 기물 주변 1칸까지 체력 -1
    Knockback,  //넉백 해당 기물이 체스판 최하단으로 밀림
    Enforce     //강화 쉴드무시 / 해당 기물의 체력 -1
}

public enum eMonsterType
{
    Betrayal,
    Mockery,
    Plague,
    Greed,
    Elite,
    Boss
}

public class Monster : MonoBehaviour
{
    public MonsterData monsterData;
    [SerializeField] GameObject monsterPrefab;
    [Header("Stat")]
    [SerializeField] float hp;
    float maxHp;
    eMonsterAttackType monsterAttackType;
    [SerializeField] eMonsterType monsterType;
    [SerializeField] GameObject canvas;
    private bool isAlive = true;

    private static Dictionary<eMonsterType, MonsterData> cachedData = new();

    public void SetType(eMonsterType type, int stageNum)
    {
        monsterType = type;

        if (!cachedData.ContainsKey(type))
        {
            var loadedData = Resources.Load<MonsterData>($"Data/{monsterType}Data");

            if (loadedData == null)
            {
                Debug.LogError($"PieceData for {monsterType} not found!");
                return;
            }
            cachedData[monsterType] = loadedData;
        }

        monsterData = cachedData[type];

        switch (stageNum) // 보스랑 엘리트 추가해야됨
        {
            case 1:
                hp = 1500f;
                break;
            case 2:
                hp = 3000f;
                break;
            case 3:
                hp = 5000f;
                break;
            case 4:
                hp = 8000f;
                break;
            case 5:                // 엘리트
                hp = 15000f;
                break;
            case 6:
                hp = 12000f;
                break;
            case 8:
                hp = 13000f;
                break;
            case 9:
                hp = 20000f;
                break;
            case 10:               // 보스
                hp = 30000f;
                break;
        }
        maxHp = hp;

        if (stageNum > 0 && stageNum < 5) // 초반 스테이지 몬스터 위치
        {
            if (type == eMonsterType.Mockery)   // Mockery
            {
                GameObject monster = Instantiate(monsterData.monsterPrefab); // 5by5일때 보스 스폰 위치 세팅
                monster.transform.position = new Vector3(-0.24f, -6.09f, 16.41f);
                monster.transform.rotation = Quaternion.Euler(18.26f, 0, 0);
            }
            else                                // Betrayal
            {
                GameObject monster = Instantiate(monsterData.monsterPrefab); // 5by5일때 보스 스폰 위치 세팅
                monster.transform.position = new Vector3(0.4f, -12.39f, 9.09f);
                monster.transform.rotation = Quaternion.Euler(15, 0, 0);
            }

        }
        else if (stageNum == 5)                 // Elite
        {
            Debug.Log("monsterData : " + monsterData);
            Debug.Log("monsterData.monsterPrefab : " + monsterData.monsterPrefab);
            GameObject monster = Instantiate(monsterData.monsterPrefab); // 7by7일때 보스 스폰 위치 세팅
            monster.transform.position = new Vector3(0.4f, -10.43f, 5.61f);
            monster.transform.rotation = Quaternion.Euler(15, 0, 0);
        }
        else if (5 < stageNum && stageNum <10)  //중반 스테이지 몬스터 위치
        {
            if (type == eMonsterType.Plague)    // Plague
            {
                GameObject monster = Instantiate(monsterData.monsterPrefab); // 7by7일때 보스 스폰 위치 세팅
                monster.transform.position = new Vector3(-0.86f, 1.12f, 18.04f);
                monster.transform.rotation = Quaternion.Euler(15, 0, 0);
            }
            else                                // Greed
            {
                GameObject monster = Instantiate(monsterData.monsterPrefab); // 7by7일때 보스 스폰 위치 세팅
                monster.transform.position = new Vector3(-0.98f, -0.91f, 14.31f); // 베랫나루 돼지
                monster.transform.rotation = Quaternion.Euler(33.18f, 0, 0);
            }

        }
        else                                    // Boss
        {
            GameObject monster = Instantiate(monsterData.monsterPrefab); // 7by7일때 보스 스폰 위치 세팅
            monster.transform.position = new Vector3(-1.19f, -13.35f, 12.35f);
            monster.transform.rotation = Quaternion.Euler(15, 0, 0);
        }
    }

    public float GetHp()
    {
        return hp;
    }

    public float GetMaxHp()
    { return maxHp; }

    public float GetHpRatio()
    {
        return (hp / maxHp);
    }

    public void SetHp(float hp)
    {
        this.hp = hp;
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }

    public void SetDead()
    {
        isAlive = false;
    }

    public void Attack(List<Node> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.currentPiece != null)
            {
                Piece piece = node.currentPiece.GetComponent<Piece>();
             
                monsterAttackType = node.monsterAttackType;
                switch (monsterAttackType)
                {
                    case eMonsterAttackType.Default:
                        if (piece.GetShield() > 0 && !piece.isDamageImmune)
                        {
                            piece.SetShield(piece.GetShield() - 1f);
                        }
                        else if(!piece.isDamageImmune)
                        {
                            piece.SetHp(piece.GetHp() - 1f);
                        }
                        break;
                    case eMonsterAttackType.Freeze:
                        if(!piece.isStatusImmune)
                        {
                            piece.SetStatusEffect(eStatusEffectType.Freeze);
                        }
                        break;
                    case eMonsterAttackType.Weak:
                        if(!piece.isStatusImmune)
                        {
                            piece.SetStatusEffect(eStatusEffectType.Weak);
                        }
                        break;
                    case eMonsterAttackType.Distort:
                        if (!piece.isStatusImmune)
                        {
                            piece.SetStatusEffect(eStatusEffectType.Distort);
                        }
                        break;
                    case eMonsterAttackType.Shock:
                        if (!piece.isStatusImmune)
                        {
                            piece.SetStatusEffect(eStatusEffectType.Shock);
                        }
                        break;
                    case eMonsterAttackType.Explode:
                        ExplodeAttack(node.GridPos);
                        break;
                    case eMonsterAttackType.Knockback:
                        if (!piece.isStatusImmune)
                        {
                            piece.SetStatusEffect(eStatusEffectType.Knockback);
                            piece.RePositioning();
                        }
                        break;
                    case eMonsterAttackType.Enforce:
                        if (!piece.isDamageImmune)
                        {
                            piece.SetHp(piece.GetHp() - 1f);
                        }
                        break;
                }
                
                if(piece.GetHp() <= 0)
                {
                    piece.SetHp(0f);
                    piece.SetDead();
                }
                GameManager.instance.CurrentUIManager.UpdateBattleUIs();
            }
        }
    }

    void ExplodeAttack(Vector2Int pos)
    {
        Vector2Int upperPos = new Vector2Int(pos.x, pos.y - 1);
        Vector2Int lowerPos = new Vector2Int(pos.x, pos.y + 1);
        Vector2Int leftPos = new Vector2Int(pos.x - 1, pos.y);
        Vector2Int rightPos = new Vector2Int(pos.x + 1, pos.y);

        Node upperNode = BoardManager.instance.GetNode(upperPos);
        Node lowerNode = BoardManager.instance.GetNode(lowerPos);
        Node leftNode = BoardManager.instance.GetNode(leftPos);
        Node rightNode = BoardManager.instance.GetNode(rightPos);

        if (upperNode.currentPiece != null)
        {
            Piece upperPiece = upperNode.currentPiece.GetComponent<Piece>();
            if (upperPiece != null && !upperPiece.isDamageImmune)
            {
                upperPiece.SetHp(upperPiece.GetHp() - 1);
            }
        }
        if (lowerNode.currentPiece != null)
        {
            Piece lowerPiece = lowerNode.currentPiece.GetComponent<Piece>();
            if (lowerPiece != null && !lowerPiece.isDamageImmune)
            {
                lowerPiece.SetHp(lowerPiece.GetHp() - 1);
            }
        }
        if (rightNode.currentPiece != null)
        {
            Piece leftPiece = leftNode.currentPiece.GetComponent<Piece>();
            if (leftPiece != null && !leftPiece.isDamageImmune)
            {
                leftPiece.SetHp(leftPiece.GetHp() - 1);
            }
        }
        if((lowerNode.currentPiece != null))
        {
            Piece rightPiece = rightNode.currentPiece.GetComponent<Piece>();
            if (rightPiece != null && !rightPiece.isDamageImmune)
            {
                rightPiece.SetHp(rightPiece.GetHp() - 1);
            }
        }
    }
}
