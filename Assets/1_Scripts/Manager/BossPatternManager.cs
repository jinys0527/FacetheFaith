using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

// 한 개 노드마다 공격 타입
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

/// 한 개 패턴
[System.Serializable]
public class BossPattern
{
    public string name = "Pattern";
    public List<NodeAttackInfo> nodes = new();   // 7×7 좌표
}

public class BossPatternManager : MonoBehaviour
{
    public static BossPatternManager instance;
    private void Awake() // 씬넘겨도 유일성이 보존되는거지
                         // 보드 매니저는 
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
    [SerializeField] BoardManager board;      // 체스판
    [SerializeField, Min(0.1f)] float warningTime = 1.5f;

    public List<BossPattern> patterns = new();
    public List<Node> nodeList;

    public int index;

    public int GetIndex()
    {
        return index;
    }

    /* -------------- API -------------- */

    /// 패턴 인덱스로 경고 표시
    public void Play(int index)
    {
        EffectManager.instance.ClearBossEffects();
        patterns = BattleManager.BattleManagerInstance.monster.GetComponent<Monster>().monsterData.attackPattern;
        if (index < 0 || index >= patterns.Count) return;

        // 1) 노드 목록 생성
        nodeList = new();

        int randomIndex = UnityEngine.Random.Range(0, patterns.Count);
        foreach (var cell in patterns[randomIndex].nodes)
        {
            Node n = board.GetNode(cell.position);     // 좌표  Node
            n.monsterAttackType = cell.attackType;
            if (n != null && n.gameObject.activeSelf) nodeList.Add(n);
        }

        // 2) 이펙트 매니저 호출
        EffectManager.instance.ShowBossEffects(nodeList);

        // 3) 지정 시간 뒤 지우기
        //StopAllCoroutines();                  // 중복 호출 대비
        //StartCoroutine(ClearAfterDelay());
    }

    public float WarningTime => warningTime;
    public int PatternCount => patterns.Count;

    /* -------------- 내부 -------------- */
    IEnumerator ClearAfterDelay()
    {
        yield return new WaitForSeconds(warningTime);
        EffectManager.instance.ClearBossEffects();
    }
}
