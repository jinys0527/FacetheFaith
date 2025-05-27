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
        return obj is NodeAttackInfo other && this.position == other.position;
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
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

    public int GetIndex()
    {
        return index;
    }

    /* -------------- API -------------- */

    /// ���� �ε����� ��� ǥ��
    public void Play(int index)
    {
        EffectManager.instance.ClearBossEffects();
        patterns = BattleManager.BattleManagerInstance.monster.GetComponent<Monster>().monsterData.attackPattern;
        if (index < 0 || index >= patterns.Count) return;

        // 1) ��� ��� ����
        nodeList = new();

        int randomIndex = UnityEngine.Random.Range(0, patterns.Count);
        foreach (var cell in patterns[randomIndex].nodes)
        {
            Node n = board.GetNode(cell.position);     // ��ǥ  Node
            n.monsterAttackType = cell.attackType;
            if (n != null && n.gameObject.activeSelf) nodeList.Add(n);
        }

        // 2) ����Ʈ �Ŵ��� ȣ��
        EffectManager.instance.ShowBossEffects(nodeList);

        // 3) ���� �ð� �� �����
        //StopAllCoroutines();                  // �ߺ� ȣ�� ���
        //StartCoroutine(ClearAfterDelay());
    }

    public float WarningTime => warningTime;
    public int PatternCount => patterns.Count;

    /* -------------- ���� -------------- */
    IEnumerator ClearAfterDelay()
    {
        yield return new WaitForSeconds(warningTime);
        EffectManager.instance.ClearBossEffects();
    }
}
