using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public enum PieceVariant { None = -1, Pawn, Knight, Rook, Bishop, Queen, King }

public enum eStatusEffectType
{
    None,       
    Freeze,     //빙결 이동 불가
    Weak,       //약화 기본 공격 * 0.5
    Shock,      //감전 공격 불가
    Distort,    //왜곡 카드 효과 적용안됨
    Knockback   //넉백 최하단으로 밀림
};

public enum ePieceType
{
    Pawn = 0,
    UpgradePawn,
    Knight,
    UpgradeKnight,
    Rook,
    UpgradeRook,
    Bishop,
    UpgradeBishop,
    Queen,
    UpgradeQueen,
    Count
}

public class Piece : MonoBehaviour
{
    public PieceData data;
    public PieceVariant pieceVariant;

    public int x;
    public int y;

    public Node node = null;
    [SerializeField] private eStatusEffectType statusEffectType;
    public GameObject[] attackEffect;

    public bool level = false; // 0에서부터 아님 1에서 부터?
    private int moveRadius = 10; // 아직은 제한이 없을려나

    [Header("Stat")]
    [SerializeField] float hp;
    [SerializeField] float atk;

    float shield = 0f;
    bool isAlive = true;

    public bool canMove = true;
    bool canAttack = true;
    bool isDistort = false;
    public int attackCount = 1;
    public float currentDamage;
    public bool isDamageImmune = false;
    public bool isStatusImmune = false;

    private SpriteRenderer spriteRenderer;

    private static Dictionary<PieceVariant, PieceData> cachedData = new();

    // 카드 효과를 순서대로 저장하는 구조
    public class DamageModifier
    {
        public float sum = 0f;      // 공격력에 더하는 값
        public float product = 1f;  // 공격력에 곱하는 값
    }

    // PlayerAttack에서 사용할 리스트 (카드 적용 순서대로)
    private List<DamageModifier> damageModifiers = new List<DamageModifier>();
    private List<DamageModifier> upcomingDamageModifiers = new List<DamageModifier>();

    public List<DamageModifier> GetDamageModifiers()
    {
        return damageModifiers;
    }

    // 카드가 적용될 때마다 아래처럼 damageModifiers에 효과를 추가해둠
    public void AddDamageModifier(float sum, float product)
    {
        damageModifiers.Add(new DamageModifier { sum = sum, product = product });
    }

    public void ClearDamageModifiers()
    {
        damageModifiers.Clear();
    }

    public float GetHp()
    {
        return hp;
    }

    public float GetAtk()
    {
        return atk;
    }

    public float GetShield()
    {
        return shield;
    }

    public void SetHp(float hp)
    {
        this.hp = hp;
    }

    public void SetAtk(float atk)
    {
        this.atk = atk;
    }

    public void SetShield(float shield)
    {
        this.shield = shield;
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }

    public void SetDead()
    {
        SoundManager.Instance.PlaySFX(e_SFXname.s_Piece_Broken);
        isAlive = false;
        node.currentPiece = null;
        gameObject.SetActive(false);
    }

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void ClearAdditionalEffect()
    {
        attackCount = 1;
        isDamageImmune = false;
        isStatusImmune = false;
    }

    public bool GetIsDistort()
    {
        return isDistort;
    }

    public bool GetCanAttack()
    {
        return canAttack;
    }

    public void SetStatusEffect(eStatusEffectType statusEffect)
    {
        this.statusEffectType = statusEffect;
        canMove = true;
        canAttack = true;
        isDistort = false;
        switch (statusEffect)
        {
            case eStatusEffectType.Freeze:
                canMove = false;
                break;
            case eStatusEffectType.Weak:
                currentDamage *= 0.5f;
                break;
            case eStatusEffectType.Shock:
                canAttack = false;
                break;
            case eStatusEffectType.Distort:
                isDistort = true;
                break;
            case eStatusEffectType.Knockback:
                if (y == 0)
                    break;
                node.currentPiece = null;

                bool isBlocked = false;

                for(int i = y-1; i >= 0; i--)
                {
                    Vector2Int pos = new Vector2Int(x, i);
                    if(BoardManager.instance.GetNode(pos).currentPiece != null)
                    {
                        pos = new Vector2Int(x, i+1);
                        node = BoardManager.instance.GetNode(pos);
                        node.currentPiece = this.gameObject;
                        y = i + 1;
                        isBlocked = true;
                        break;
                    }
                }

                if (isBlocked)
                    break;
                node.currentPiece = this.gameObject;
                y = 0;
                break;
        }
    }

    public eStatusEffectType GetStatusEffect()
    {
        return statusEffectType;
    }

    public void Upgrade()
    {
        level = true;
        hp = data.upgradeHp;
        atk = data.upgradeAtk;
        moveRadius = data.upgradeMoveRadius;
        spriteRenderer.sprite = data.upgradePieceSprite;
    }

    public int GetMoveRadius()
    {
        return moveRadius;
    }

    public void RePositioning()     // 노드 중앙으로 재배치
    {
        if (BoardManager.instance != null)
        {
            SoundManager.Instance.PlaySFX(e_SFXname.s_Piece_Move);
            float offsetX = (BoardManager.instance.width - 1) * 0.5f;
            transform.position = new Vector3((x - offsetX) * BoardManager.instance.GetNodeSize(), 0, y - 5);
        }
        else 
        {
            if (node != null)
            {
                transform.position = node.transform.position + Vector3.up * 0.5f;
            }
        }
    }

    public void ReColor()
    {
        if (canMove)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, spriteRenderer.color.a);
        }
        else
        { 
            spriteRenderer.color = new Color(0.7f, 0.7f, 0.7f, spriteRenderer.color.a); 
        }
    }

    public void SetVariant(PieceVariant variant)
    {
        this.pieceVariant = variant;

        if (!cachedData.ContainsKey(variant))
        {
            var loadedData = Resources.Load<PieceData>($"Data/{variant}Data");
            if (loadedData == null)
            {
                Debug.LogError($"PieceData for {variant} not found!");
                return;
            }
            cachedData[variant] = loadedData;
        }

        data = cachedData[variant];
        InitializeFromData();
    }

    private void InitializeFromData()
    {
        hp = data.hp;
        atk = data.atk;
        moveRadius = data.moveRadius;
        currentDamage = atk;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        spriteRenderer.sprite = data.pieceSprite;
    }

}

