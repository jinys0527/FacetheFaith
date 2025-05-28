using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectManager : MonoBehaviour
{


    [SerializeField] private GameObject glowEffectPrefab;
    [SerializeField] private GameObject attackEffectPrefab;
    [SerializeField] private GameObject freezeEffectPrefab;
    [SerializeField] private GameObject weakenEffectPrefab;
    [SerializeField] private GameObject distortEffectPrefab;
    [SerializeField] private GameObject shockEffectPrefab;
    [SerializeField] private GameObject explodeEffectPrefab;
    [SerializeField] private GameObject knockbackEffectPrefab;
    [SerializeField] private GameObject enforceEffectPrefab;


    private Dictionary<eMonsterAttackType, GameObject> bossEffectPrefabs;

    private List<GameObject> activeEffects = new();
    private List<GameObject> bossEffects = new();
    public static EffectManager instance;
    private void Awake() // ���Ѱܵ� ���ϼ��� �����Ǵ°���
                         // ���� �Ŵ����� 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    private void Start()
    {
        bossEffectPrefabs = new()
        {
            { eMonsterAttackType.Default, attackEffectPrefab },
            { eMonsterAttackType.Freeze, freezeEffectPrefab },
            { eMonsterAttackType.Weak, weakenEffectPrefab },
            { eMonsterAttackType.Distort, distortEffectPrefab },
            { eMonsterAttackType.Shock, shockEffectPrefab },
            { eMonsterAttackType.Explode, explodeEffectPrefab },
            { eMonsterAttackType.Knockback, knockbackEffectPrefab },
            { eMonsterAttackType.Enforce, enforceEffectPrefab }
        };
    }

    private void ShowEffects(List<Node> nodes, GameObject prefab, List<GameObject> targetList)
    {
        foreach (var node in nodes)
        {
            GameObject effect = Instantiate(prefab);
            effect.transform.position = node.transform.position + new Vector3(0, 0.253f, 0);  // ��¦ ���
            targetList.Add(effect);
        }
    }


    public void ShowPlaceEffects(List<Node> nodes) // ��� ����Ʈ �ѱ�� �ش� ���鿡 ����Ʈ ��� ��
    {
        ShowEffects(nodes, glowEffectPrefab, activeEffects);
    }

    public void ShowBossEffects(List<Node> nodes) // ��� ����Ʈ �ѱ�� �ش� ���鿡 ����Ʈ ��� ��
    {
        foreach (var node in nodes)
        {
            if (!bossEffectPrefabs.TryGetValue(node.monsterAttackType, out GameObject prefab))
                continue;

            GameObject effect = Instantiate(prefab);
            effect.transform.position = node.transform.position + new Vector3(0, 0.251f, 0);
            bossEffects.Add(effect);
        }
    }

    public void ClearPlaceEffects()
    {
        foreach (var effect in activeEffects)
        {
            Destroy(effect);
        }
        activeEffects.Clear();
    }

    public void ClearBossEffects()
    {
        foreach (var effect in bossEffects)
        {
            Destroy(effect);
        }
        bossEffects.Clear();
    }
}
