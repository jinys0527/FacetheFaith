using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class BattlePieceManager : MonoBehaviour
{
    public static BattlePieceManager instance;

    // 업그레이드 전, 후 각각 PieceVariant 별 개수를 딕셔너리로 관리
    private Dictionary<PieceVariant, int> normalCounts = new Dictionary<PieceVariant, int>();
    private Dictionary<PieceVariant, int> upgradedCounts = new Dictionary<PieceVariant, int>();

    public List<Piece> pieces = new List<Piece>();
    public InventoryPiece[] inventoryPieces;

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

        SceneManager.sceneLoaded += OnSceneLoaded;

        // 딕셔너리 초기화 (모든 PieceVariant에 0 세팅)
        foreach (PieceVariant variant in System.Enum.GetValues(typeof(PieceVariant)))
        {
            normalCounts[variant] = 0;
            upgradedCounts[variant] = 0;
        }

        if (inventoryPieces != null)
        {
            foreach (InventoryPiece inventoryPiece in inventoryPieces)
            {
                inventoryPiece.CountingPiece();
            }
        }
    }

    public int GetCount(PieceVariant pieceVariant, bool upgrade)
    {
        if (upgrade)
        { 
            return upgradedCounts.TryGetValue(pieceVariant, out int count) ? count : 0;
        }
        else
        {
            return normalCounts.TryGetValue(pieceVariant, out int count) ? count : 0;
        }
            
    }

    public void SetCount(PieceVariant pieceVariant, bool upgrade, int count)
    {
        if (upgrade)
        {
            upgradedCounts[pieceVariant] = count;
        }
        else
        {
            normalCounts[pieceVariant] = count;
        }

        if (inventoryPieces != null)
        {
            foreach (InventoryPiece inventoryPiece in inventoryPieces)
            {
                inventoryPiece.CountingPiece();
            }
        }
    }

    public void AddPiece(Piece piece)
    {
        if(!pieces.Contains(piece))
            pieces.Add(piece);
    }

    public void ResortPieceList()
    {
        for(int i = pieces.Count - 1; i >= 0; i++)
        {
            if (pieces[i] == null)
            {
                pieces.RemoveAt(i);
            }
        }
    }

    public void DestroyPiece(Piece piece)
    {
        pieces.Remove(piece);
        SetCount(piece.pieceVariant, piece.level, GetCount(piece.pieceVariant, piece.level) + 1);
        Destroy(piece.gameObject);
    }

    public void EffectSelectedPiece(Piece piece)
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i] != piece)
            {
                SpriteRenderer spriteRenderer = pieces[i].gameObject.GetComponentInChildren<SpriteRenderer>();
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
            }
        }
    }

    public void DisableEffectPieces()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            SpriteRenderer spriteRenderer = pieces[i].gameObject.GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        }
    }

    public void DisableStatusPieces()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].GetStatusEffect() != eStatusEffectType.None)
            {
                pieces[i].SetStatusEffect(eStatusEffectType.None);
            }
        }
    }

    public void ReColorPieces()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].ReColor();
        }
    }

    public void AblePieces()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].GetStatusEffect() != eStatusEffectType.Freeze)
                pieces[i].canMove = true;
        }
        ReColorPieces();
    }

    public void DisablePieces()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].canMove = false;
        }
        ReColorPieces();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PlayerManager.instance != null)
        {
            // PlayerManager.instance.pieceCounts, upgradedPieceCounts 가 딕셔너리라고 가정
            foreach (var playerPieceVariant in PlayerManager.instance.pieceCounts)
            {
                normalCounts[playerPieceVariant.Key] = playerPieceVariant.Value;
            }
            foreach (var playerPieceVariant in PlayerManager.instance.upgradedPieceCounts)
            {
                upgradedCounts[playerPieceVariant.Key] = playerPieceVariant.Value;
            }
        }
    }
}
