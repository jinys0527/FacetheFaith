using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PieceControlManager : MonoBehaviour
{
    public static PieceControlManager instance;
    public static EffectManager effect;
    public static BossPatternManager bosspattern;

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

    public Piece piece;
    public bool isPlace = false;
    public bool canMove = false;

    public bool preventEffect = false;

    public bool blockPiece = false;

    public const int MAXPIECECOUNT = 6;

    // Update is called once per frame
    public void PieceControlUpdate()
    {
        if (BattleManager.instance.GetState() is eBattleState.GameOver or eBattleState.StageClear)
            return;

        UpdatePlaceAndMoveFlags();

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseDown();
        }
        if (Input.GetMouseButton(0))
        {
            HandleMouseDrag();
        }
        if (Input.GetMouseButtonUp(0))
        {
            HandleMouseUp();
        }
        if (BattleManager.instance != null && !BattleManager.instance.GetCanLayOut())
        {
            HandleNumberKeyInput();
        } 
    }

    private void HandleMouseDown() => SelectPiece();
    private void HandleMouseDrag() => DragPiece();
    private void HandleMouseUp()
    {
        if (isPlace)
        {
            PlacePiece();
        }
        else
        {
            MovePiece();
        }
    }

    private void UpdatePlaceAndMoveFlags()
    {
        if (BattleManager.instance != null)
        {
            isPlace = BattleManager.instance.GetCanLayOut();
            canMove = BattleManager.instance.GetIsPlayerTurn();
        }
        else
        {
            isPlace = true;
            canMove = false;
        }
    }

    private void HandleNumberKeyInput()
    {
        if (piece != null) return;
        if (BattlePieceManager.instance.pieces.Count == 0) return;

        for (int i = 0; i < BattlePieceManager.instance.pieces.Count && i < MAXPIECECOUNT; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                BattlePieceManager.instance.DisableEffectPieces();
                var pieceToSelect = GameObject.Find("StatusInfoUI").GetComponent<StatusInfoUI>().pieces[i].GetComponent<StatusPiece>().piece;
                BattlePieceManager.instance.EffectSelectedPiece(pieceToSelect);
                PieceControlManager.instance.preventEffect = false;
                break;
            }
        }
    }
    private void EndPieceAction()
    {
        piece = null;
        MovementManager.instance.InvalidateAll();
        EffectManager.instance.ClearPlaceEffects();
    }

    void SelectPiece()
    {
        if (blockPiece)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (piece != null)
        {
            var moves = BoardManager.instance.GetFrontTwoRows();
            EffectManager.instance.ShowPlaceEffects(moves);
            BattlePieceManager.instance.EffectSelectedPiece(piece);

            return;
        }

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Piece"))
            {
                SpriteRenderer spriteRenderer = hit.collider.transform.GetChild(0).GetComponent<SpriteRenderer>();
                if (spriteRenderer.color.a != 1)
                    continue;
                Piece selected = hit.collider.GetComponent<Piece>();

                // ���� �⹰�� �ٽ� Ŭ���� ��� �� ����
                if (selected == piece)
                    return;

                piece = selected;

                // �̵� ��� ��� �� ����Ʈ ǥ��

                if (isPlace)
                {
                    var moves = BoardManager.instance.GetFrontTwoRows();
                    EffectManager.instance.ShowPlaceEffects(moves);

                }
                else
                {
                    if (piece.canMove)
                    {
                        SoundManager.Instance.PlaySFX(e_SFXname.s_Piece_Move);
                        var moves = MovementManager.instance.GetMoves(piece);
                        EffectManager.instance.ShowPlaceEffects(moves);
                    }
                }
                BattlePieceManager.instance.EffectSelectedPiece(piece);
                return;
            }
        }

        // Ŭ���� ����� �⹰�� �ƴϸ� ���� ���� + ����Ʈ ����
        piece = null;
        EffectManager.instance.ClearPlaceEffects();
    }
    
    private (Node lastNode, bool blocked, int x, int y) GetLastNodeFromRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        int x = -1, y = -1;
        Node lastNode = null;
        bool blocked = false;

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.CompareTag("Node")) continue;

            Node node = hit.collider.GetComponent<Node>();

            // ���� ���� ����
            if (x != -1 && y != -1)
            {
                if (node.GridPos.y > y) continue;
                if ((node.GridPos.x <= 3 && x > node.GridPos.x) ||
                    (node.GridPos.x >= 3 && x < node.GridPos.x))
                    continue;
            }

            // ��ֹ� / �Ʊ� ����
            if (BoardManager.instance != null)
            {
                if (BoardManager.instance.IsBlocked(node.GridPos))
                {
                    blocked = true;
                    x = node.GridPos.x;
                    y = node.GridPos.y;
                    continue; 
                }
            }
            else if(MapManager.instance != null)
            { 
                if (MapManager.instance.IsBlocked(node.GridPos))
                {
                    blocked = true;
                    x = node.GridPos.x;
                    y = node.GridPos.y;
                    continue;
                }
            }

            // �� ĭ �߰� => �ĺ��� ���� (������ ���� piece ��ǥ�� �ǵ帮�� ����)
            blocked = false;
            x = node.GridPos.x;
            y = node.GridPos.y;
            lastNode = node;
        }

        return (lastNode, blocked, x, y);
    }

    private void UpdatePiecePosition(Piece piece, Node targetNode, bool addPiece = false)
    {
        SoundManager.Instance.PlaySFX(e_SFXname.s_Piece_Move);
        if (piece.node != null)
            piece.node.currentPiece = null;

        targetNode.currentPiece = piece.gameObject;
        piece.node = targetNode;

        piece.x = targetNode.GridPos.x;
        piece.y = targetNode.GridPos.y;

        if(addPiece)
        {
            BattlePieceManager.instance.AddPiece(piece);
        }

        piece.RePositioning();
    }

    void MovePiece()
    {
        if (!preventEffect)
        {
            BattlePieceManager.instance.DisableEffectPieces();
        }
        else
        {
            preventEffect = !preventEffect;
        }

        if (piece == null) return;
        Node originNode = piece.node; //  �̵� �� ��ġ ����

        BattlePieceManager.instance.DisableEffectPieces();

        var (lastNode, blocked, x, y) = GetLastNodeFromRay();

       
        // �̵� ���ɼ� ���� Ȯ�� + ��ǥ/��� ��ü 
        if (lastNode != null && !blocked && piece.canMove && canMove)
        {
            var moves = MovementManager.instance.GetMoves(piece);
            if (moves.Contains(lastNode))          // �չ� �̵�
            {
                UpdatePiecePosition(piece, lastNode, false);

                if (BattleManager.instance != null)
                {
                    piece.canMove = false;
                }

                if (MapManager.instance != null)
                {
                    MapManager.instance.RevealNextNodesOnly(lastNode);
                }

                piece.ReColor();
            }
            else
            {
                // �̵� �Ұ� -> ���� �ڸ��� �ǵ�����
                piece.RePositioning();
            }
        }
        else
        {
            // �̵� �Ұ� -> ���� �ڸ��� �ǵ�����
            if (piece != null)
                piece.RePositioning();
        }
        EndPieceAction();
    }


    void DragPiece()
    {
        if (piece == null)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Mathf.Abs(ray.direction.y) > 0.0001f)
        {
            float t = -ray.origin.y / ray.direction.y;
            if (t >= 0)
            {
                Vector3 groundPoint = ray.origin + ray.direction * t;
                print(groundPoint);
                piece.gameObject.transform.position = new Vector3(groundPoint.x, 0, groundPoint.z);
            }
            else
            {
                piece.gameObject.transform.position = new Vector3(0, 0, -100);
            }
        }
    }


    private void SwapPieces(Piece pieceA, Piece pieceB)
    {
        Node nodeA = pieceA.node;
        Node nodeB = pieceB.node;

        // Swap nodes' currentPiece references
        nodeA.currentPiece = pieceB.gameObject;
        nodeB.currentPiece = pieceA.gameObject;

        // Swap pieces' node references
        pieceA.node = nodeB;
        pieceB.node = nodeA;

        // Swap grid positions
        int tempX = pieceA.x;
        int tempY = pieceA.y;

        pieceA.x = pieceB.x;
        pieceA.y = pieceB.y;
        pieceB.x = tempX;
        pieceB.y = tempY;

        pieceA.RePositioning();
        pieceB.RePositioning();
    }

    void PlacePiece()
    {
        if (piece == null) return;

        BattlePieceManager.instance.DisableEffectPieces();

        var (lastNode, blocked, x, y) = GetLastNodeFromRay();

        // �̵� ���ɼ� ���� Ȯ�� + ��ǥ/��� ��ü
        if (lastNode != null && !blocked)
        {
            List<Node> moves = BoardManager.instance.GetFrontTwoRows();
            if (moves.Contains(lastNode) && (BattlePieceManager.instance.pieces.Count <= MAXPIECECOUNT - 1 || (BattlePieceManager.instance.pieces.Count == MAXPIECECOUNT && piece.node != null))) 
            {
                UpdatePiecePosition(piece, lastNode, true);
            }
            else
            {
                BattlePieceManager.instance.DestroyPiece(piece);
            }
        }
        else if (lastNode != null && blocked)
        {
            if (piece.node != lastNode)
            {
                List<Node> moves = BoardManager.instance.GetFrontTwoRows();
                if (moves.Contains(lastNode)) 
                {
                    if (piece.node == null)
                    {
                        BattlePieceManager.instance.DestroyPiece(lastNode.currentPiece.GetComponent<Piece>());
                        UpdatePiecePosition(piece, lastNode, true);
                    }
                    else
                    {
                        Piece currentPieceScript = lastNode.currentPiece.GetComponent<Piece>();
                        SwapPieces(piece, currentPieceScript);
                    }
                    SoundManager.Instance.PlaySFX(e_SFXname.s_Piece_Move);
                }
                else
                {
                    BattlePieceManager.instance.DestroyPiece(piece);
                }
            }
        }
        else
        {
            BattlePieceManager.instance.DestroyPiece(piece);
        }

        EndPieceAction();
    }
}
