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


    //[SerializeField] private Sprite glowSprite;
    //[SerializeField] private Sprite attackSprite;
    //[SerializeField] private Sprite freezeSprite;
    //[SerializeField] private Sprite weakeneSprite;
    //[SerializeField] private Sprite distortSprite;
    //[SerializeField] private Sprite shockSprite;
    //[SerializeField] private Sprite explodeSprite;
    //[SerializeField] private Sprite knockbackSprite;
    //[SerializeField] private Sprite enforceSprite;


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


    public void ShowPlaceEffects(List<Node> nodes) // ��� ����Ʈ �ѱ�� �ش� ���鿡 ����Ʈ ��� ��
    {
        //ClearEffects();  // ���� ����Ʈ ����

        foreach (var node in nodes)
        {
            GameObject effect = Instantiate(glowEffectPrefab);
            effect.transform.position = node.transform.position + new Vector3(0, 0.253f, 0);  // ��¦ ���
            activeEffects.Add(effect);
        }
    }

    public void ShowBossEffects(List<Node> nodes) // ��� ����Ʈ �ѱ�� �ش� ���鿡 ����Ʈ ��� ��
    {
        //ClearEffects();  // ���� ����Ʈ ����

        foreach (var node in nodes)
        {
            GameObject effect = null;
            switch (node.monsterAttackType)
            {
                case eMonsterAttackType.Default:
                    effect = Instantiate(attackEffectPrefab);
                    break;
                case eMonsterAttackType.Freeze:
                    effect = Instantiate(freezeEffectPrefab);
                    break;
                case eMonsterAttackType.Weak:
                    effect = Instantiate(weakenEffectPrefab);
                    break;
                case eMonsterAttackType.Distort:
                    effect = Instantiate(distortEffectPrefab);
                    break;
                case eMonsterAttackType.Shock:
                    effect = Instantiate(shockEffectPrefab);
                    break;
                case eMonsterAttackType.Explode:
                    effect = Instantiate(explodeEffectPrefab);
                    break;
                case eMonsterAttackType.Knockback:
                    effect = Instantiate(knockbackEffectPrefab);
                    break;
                case eMonsterAttackType.Enforce:
                    effect = Instantiate(enforceEffectPrefab);
                    break;
                default:
                    break;
            }
            if (effect == null)
                continue;
            effect.transform.position = node.transform.position + new Vector3(0, 0.251f, 0);  // ��¦ ���
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
