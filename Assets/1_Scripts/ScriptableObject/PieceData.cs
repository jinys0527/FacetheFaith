using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPieceData", menuName = "Chess/PieceData")]
public class PieceData : ScriptableObject
{
    public PieceVariant pieceType;
    public float hp;
    public float atk;
    public int moveRadius;
    public Sprite pieceSprite;
    public float upgradeHp;
    public float upgradeAtk;
    public int upgradeMoveRadius;
    public Sprite upgradePieceSprite;
}
