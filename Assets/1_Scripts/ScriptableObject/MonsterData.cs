using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterData", menuName = "Monster/MonsterData")]
public class MonsterData : ScriptableObject
{
    public eMonsterType monsterType;
    public GameObject monsterPrefab;

    public List<BossPattern> attackPattern = new();  // 7x7 ÆÐÅÏ¿ë
}
