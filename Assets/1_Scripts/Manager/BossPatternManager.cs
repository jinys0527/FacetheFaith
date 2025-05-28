using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

// �� �� ��帶�� ���� Ÿ��
[System.Serializable]
public struct NodeAttackInfo
{
    public Vector2Int position;
    public eMonsterAttackType attackType;

    public override bool Equals(object obj)
    {
        if (obj is not NodeAttackInfo other) return false;
        return position.Equals(other.position) && attackType == other.attackType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(position, attackType);
    }
}

/// �� �� ����
[System.Serializable]
public class BossPattern
{
    public string name = "Pattern";
    public List<NodeAttackInfo> nodes = new();   // 7��7 ��ǥ
}

public class BossPatternManager : MonoBehaviour
{
    public static BossPatternManager instance;
    private void Awake() // ���Ѱܵ� ���ϼ��� �����Ǵ°���
                         // ���� �Ŵ����� 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("Reference")]
    [SerializeField] BoardManager board;      // ü����
    [SerializeField, Min(0.1f)] float warningTime = 1.5f;

    public List<BossPattern> patterns = new();
    public List<Node> nodeList;

    public int index;

    /* -------------- API -------------- */

    /// ���� �ε����� ��� ǥ��
    public void Play()
    {
        EffectManager.instance.ClearBossEffects();
        patterns = BattleManager.instance.monster.GetComponent<Monster>().monsterData.attackPattern;

        // 1) ��� ��� ����
        nodeList = new();

        index = UnityEngine.Random.Range(0, patterns.Count);
        foreach (var cell in patterns[index].nodes)
        {
            Node n = board.GetNode(cell.position);
            if (n != null && n.gameObject.activeSelf)
            {
                n.monsterAttackType = cell.attackType;
                nodeList.Add(n);
            }
        }

        // 2) ����Ʈ �Ŵ��� ȣ��
        EffectManager.instance.ShowBossEffects(nodeList);

    }

    public float WarningTime => warningTime;
    public int PatternCount => patterns.Count;
}
