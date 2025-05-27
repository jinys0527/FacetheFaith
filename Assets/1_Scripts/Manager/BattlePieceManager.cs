using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class BattlePieceManager : MonoBehaviour
{
    public static BattlePieceManager instance;
    private void Awake() // 씬넘겨도 유일성이 보존되는거지
                         // 보드 매니저는 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public int pawnCount = 0;
    public int knightCount = 0;
    public int bishopCount = 0;
    public int rookCount = 0;
    public int queenCount = 0;
    public int upgradedPawnCount = 0;
    public int upgradedKnightCount = 0;
    public int upgradedBishopCount = 0;
    public int upgradedRookCount = 0;
    public int upgradedQueenCount = 0;

    public List<Piece> pieces = new List<Piece>();

    public int GetCount(PieceVariant pieceVariant, bool upgrade)
    {
        if (upgrade)
        {
            if (pieceVariant == PieceVariant.Pawn)
                return upgradedPawnCount;
            if (pieceVariant == PieceVariant.Knight)
                return upgradedKnightCount;
            if (pieceVariant == PieceVariant.Bishop)
                return upgradedBishopCount;
            if (pieceVariant == PieceVariant.Rook)
                return upgradedRookCount;
            if (pieceVariant == PieceVariant.Queen)
                return upgradedQueenCount;
        }
        else
        {
            if (pieceVariant == PieceVariant.Pawn)
                return pawnCount;
            if (pieceVariant == PieceVariant.Knight)
                return knightCount;
            if (pieceVariant == PieceVariant.Bishop)
                return bishopCount;
            if (pieceVariant == PieceVariant.Rook)
                return rookCount;
            if (pieceVariant == PieceVariant.Queen)
                return queenCount;
        }
        return 0;
    }

    public void SetCount(PieceVariant pieceVariant, bool upgrade, int count)
    {
        if (upgrade)
        {
            if (pieceVariant == PieceVariant.Pawn)
                upgradedPawnCount = count;
            if (pieceVariant == PieceVariant.Knight)
                upgradedKnightCount = count;
            if (pieceVariant == PieceVariant.Bishop)
                upgradedBishopCount = count;
            if (pieceVariant == PieceVariant.Rook)
                upgradedRookCount = count;
            if (pieceVariant == PieceVariant.Queen)
                upgradedQueenCount = count;
        }
        else
        {
            if (pieceVariant == PieceVariant.Pawn)
                pawnCount = count;
            if (pieceVariant == PieceVariant.Knight)
                knightCount = count;
            if (pieceVariant == PieceVariant.Bishop)
                bishopCount = count;
            if (pieceVariant == PieceVariant.Rook)
                rookCount = count;
            if (pieceVariant == PieceVariant.Queen)
                queenCount = count;
        }

        var inventoryPieces = GameObject.Find("Canvas_Overlay").GetComponentsInChildren<InventoryPiece>();
        foreach (InventoryPiece inventoryPiece in inventoryPieces)
        {
            inventoryPiece.CountingPiece();
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
        //ResortPieceList();
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
        Debug.Log("가능");
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].GetStatusEffect() != eStatusEffectType.Freeze)
                pieces[i].canMove = true;
        }
        ReColorPieces();
    }

    public void DisablePieces()
    {
        Debug.Log("불가능");
        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].canMove = false;
        }
        ReColorPieces();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log($"플레이어 에게서 기물 가져옴");

        if (PlayerManager.instance != null)
        {
            pawnCount = PlayerManager.instance.pawnCount;
            knightCount = PlayerManager.instance.knightCount;
            bishopCount = PlayerManager.instance.bishopCount;
            rookCount = PlayerManager.instance.rookCount;
            queenCount = PlayerManager.instance.queenCount;
            upgradedPawnCount = PlayerManager.instance.upgradedPawnCount;
            upgradedKnightCount = PlayerManager.instance.upgradedKnightCount;
            upgradedBishopCount = PlayerManager.instance.upgradedBishopCount;
            upgradedRookCount = PlayerManager.instance.upgradedRookCount;
            upgradedQueenCount = PlayerManager.instance.upgradedQueenCount;
        }


    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
