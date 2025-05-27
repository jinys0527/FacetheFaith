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
    public bool CanMove = false;

    public bool preventEffect = false;

    public bool blockPiece = false;

    // Update is called once per frame
    public void PieceControlUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (BattleManager.BattleManagerInstance != null)
            {
                isPlace = BattleManager.BattleManagerInstance.GetCanLayOut();
                CanMove = BattleManager.BattleManagerInstance.GetIsPlayerTurn();
            }
            else
            {
                isPlace = true;
                CanMove = false;
            }
            if(BattleManager.BattleManagerInstance.GetState() != eBattleState.GameOver && BattleManager.BattleManagerInstance.GetState() != eBattleState.StageClear)
                SelectPiece();
        }
        if (Input.GetMouseButton(0))
        {
            DragPiece();
        }
        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log(isPlace);
            if (isPlace)
                PlacePiece();
            else
                MovePiece();
        }
        if (BattleManager.BattleManagerInstance != null)
            if (!BattleManager.BattleManagerInstance.GetCanLayOut())
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    if (BattlePieceManager.instance.pieces.Count >= 1)
                        if (piece == null)
                    {
                        BattlePieceManager.instance.DisableEffectPieces();
                        BattlePieceManager.instance.EffectSelectedPiece(GameObject.Find("StatusInfoUI").GetComponent<StatusInfoUI>().pieces[0].GetComponent<StatusPiece>().piece);
                        PieceControlManager.instance.preventEffect = false;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    if (BattlePieceManager.instance.pieces.Count >= 2)
                        if (piece == null)
                    {
                        BattlePieceManager.instance.DisableEffectPieces();
                        BattlePieceManager.instance.EffectSelectedPiece(GameObject.Find("StatusInfoUI").GetComponent<StatusInfoUI>().pieces[1].GetComponent<StatusPiece>().piece);
                        PieceControlManager.instance.preventEffect = false;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    if (BattlePieceManager.instance.pieces.Count >= 3)
                        if (piece == null)
                    {
                        BattlePieceManager.instance.DisableEffectPieces();
                        BattlePieceManager.instance.EffectSelectedPiece(GameObject.Find("StatusInfoUI").GetComponent<StatusInfoUI>().pieces[2].GetComponent<StatusPiece>().piece);
                        PieceControlManager.instance.preventEffect = false;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    if (BattlePieceManager.instance.pieces.Count >= 4)
                        if (piece == null)
                    {
                        BattlePieceManager.instance.DisableEffectPieces();
                        BattlePieceManager.instance.EffectSelectedPiece(GameObject.Find("StatusInfoUI").GetComponent<StatusInfoUI>().pieces[3].GetComponent<StatusPiece>().piece);
                        PieceControlManager.instance.preventEffect = false;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    if (BattlePieceManager.instance.pieces.Count >= 5)
                        if (piece == null)
                    {
                        BattlePieceManager.instance.DisableEffectPieces();
                        BattlePieceManager.instance.EffectSelectedPiece(GameObject.Find("StatusInfoUI").GetComponent<StatusInfoUI>().pieces[4].GetComponent<StatusPiece>().piece);
                        PieceControlManager.instance.preventEffect = false;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    if(BattlePieceManager.instance.pieces.Count>=6)
                    if (piece == null)
                    {
                        BattlePieceManager.instance.DisableEffectPieces();
                        BattlePieceManager.instance.EffectSelectedPiece(GameObject.Find("StatusInfoUI").GetComponent<StatusInfoUI>().pieces[5].GetComponent<StatusPiece>().piece);
                        PieceControlManager.instance.preventEffect = false;
                    }
                }
            }
    }

    void SelectPiece()
    {
        if (blockPiece)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (piece != null)
        {

            var moves = BoardManager.instance.GetFrontTwoRaws();
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
                    var moves = BoardManager.instance.GetFrontTwoRaws(); //
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

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        int x = -1, y = -1;
        Node lastNode = null;
        bool blocked = false;

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.CompareTag("Node")) continue;

            Node node = hit.collider.GetComponent<Node>();

            /* ���� ���� ���� --------------------------------------------------- */
            if (x != -1 && y != -1)
            {
                if (node.GridPos.y > y) continue;
                if ((node.GridPos.x <= 3 && x > node.GridPos.x) ||
                    (node.GridPos.x >= 3 && x < node.GridPos.x))
                    continue;
            }

            /* ��ֹ� / �Ʊ� ���� ---------------------------------------------- */
            if (BoardManager.instance != null) // ����Ŵ��������� �Ʒ�
            {
                if (BoardManager.instance.IsBlocked(node.GridPos))
                {
                    blocked = true;
                    x = node.GridPos.x;
                    y = node.GridPos.y;
                    continue; // �������� �� ���� �� ��
                }
            }
            else { // ������ �ʸŴ��� �����״ϱ� ����
                if (MapManager.instance.IsBlocked(node.GridPos))
                {
                    blocked = true;
                    x = node.GridPos.x;
                    y = node.GridPos.y;
                    continue; // �������� �� ���� �� ��
                }
            }


                /* �� ĭ �߰� => �ĺ��� ���� (������ ���� piece ��ǥ�� �ǵ帮�� ����) */
                blocked = false;
            x = node.GridPos.x;
            y = node.GridPos.y;
            lastNode = node;
        }

        /* ---------------------------------------------------------------------- */
        /* �̵� ���ɼ� ���� Ȯ�� + ��ǥ/��� ��ü                                */
        /* ---------------------------------------------------------------------- */
        if (lastNode != null && !blocked && piece.canMove && CanMove)
        {
            var moves = MovementManager.instance.GetMoves(piece);
            if (moves.Contains(lastNode))          // �չ� �̵�
            {
                SoundManager.Instance.PlaySFX(e_SFXname.s_Piece_Move);
                /* ��� ��ũ ��ü */
                if (piece.node != null)
                    piece.node.currentPiece = null;

                lastNode.currentPiece = piece.gameObject;
                piece.node = lastNode;

                /* �̶� ��ǥ�� Ȯ�������� ���� */
                piece.x = lastNode.GridPos.x;
                piece.y = lastNode.GridPos.y;

                if (BattleManager.BattleManagerInstance != null)
                {
                    piece.canMove = false;
                }

                if (MapManager.instance != null)
                {
                    MapManager.instance.RevealNextNodesOnly(lastNode);
                    //MapManager.instance.RevealPathToTarget(originNode, lastNode);
                }

                piece.ReColor();
            }
        }


        /* ���� �Ǵ� ���� */
        piece.RePositioning();
        piece = null;

        MovementManager.instance.InvalidateAll();
        EffectManager.instance.ClearPlaceEffects();
        Debug.Log($"last = ({x},{y})");
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
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
                piece.gameObject.transform.position = new Vector3(groundPoint.x, 0, groundPoint.z);
            }
            else
                piece.gameObject.transform.position = new Vector3(0, 0, -100);
        }
    }

    void PlacePiece()
    {
        if (piece == null) return;
        //isPlace = false;
        BattlePieceManager.instance.DisableEffectPieces();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        int x = -1, y = -1;
        Node lastNode = null;
        bool blocked = false;

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.CompareTag("Node")) continue;

            Node node = hit.collider.GetComponent<Node>();

            /* ���� ���� ���� --------------------------------------------------- */
            if (x != -1 && y != -1)
            {
                if (node.GridPos.y > y) continue;
                if ((node.GridPos.x <= 3 && x > node.GridPos.x) ||
                    (node.GridPos.x >= 3 && x < node.GridPos.x))
                    continue;
            }

            /* ��ֹ� / �Ʊ� ���� ---------------------------------------------- */
            if (BoardManager.instance.IsBlocked(node.GridPos))
            {
                blocked = true;
            }
            else
                blocked = false;
            /* �� ĭ �߰� => �ĺ��� ���� (������ ���� piece ��ǥ�� �ǵ帮�� ����) */
            x = node.GridPos.x;
            y = node.GridPos.y;
            lastNode = node;
        }

        /* ---------------------------------------------------------------------- */
        /* �̵� ���ɼ� ���� Ȯ�� + ��ǥ/��� ��ü                                */
        /* ---------------------------------------------------------------------- */
        if (lastNode != null && !blocked)
        {
            List<Node> moves = BoardManager.instance.GetFrontTwoRaws();
            if (moves.Contains(lastNode) && (BattlePieceManager.instance.pieces.Count <= 5 || (BattlePieceManager.instance.pieces.Count == 6 && piece.node != null)))          // �չ� �̵�
            {
                /* ��� ��ũ ��ü */
                if (piece.node != null)
                    piece.node.currentPiece = null;

                lastNode.currentPiece = piece.gameObject;
                piece.node = lastNode;

                /* �̶� ��ǥ�� ���� */
                piece.x = lastNode.GridPos.x;
                piece.y = lastNode.GridPos.y;
                BattlePieceManager.instance.AddPiece(piece);
                SoundManager.Instance.PlaySFX(e_SFXname.s_Piece_Move);
            }
            else
            {
                BattlePieceManager.instance.DestroyPiece(piece);
            }
        }
        else if (lastNode != null && blocked)
        {
            Debug.Log("�ߵ�");
            if (piece.node != lastNode)
            {
                List<Node> moves = BoardManager.instance.GetFrontTwoRaws();
                if (moves.Contains(lastNode))          // �չ� �̵�
                {
                    if (piece.node == null)
                    {
                        Piece currentPieceScript = lastNode.currentPiece.GetComponent<Piece>();
                        //PieceManager.instance.SetCount(currentPieceScript.pieceVariant, currentPieceScript.level, PieceManager.instance.GetCount(currentPieceScript.pieceVariant, currentPieceScript.level) + 1);
                        //Destroy(currentPieceScript.gameObject);
                        BattlePieceManager.instance.DestroyPiece(currentPieceScript);

                        if (piece.node != null)
                            piece.node.currentPiece = null;

                        lastNode.currentPiece = piece.gameObject;
                        piece.node = lastNode;

                        /* �̶� ��ǥ�� Ȯ�������� ���� */
                        piece.x = lastNode.GridPos.x;
                        piece.y = lastNode.GridPos.y;
                        BattlePieceManager.instance.AddPiece(piece);
                    }
                    else
                    {
                        Piece currentPieceScript = lastNode.currentPiece.GetComponent<Piece>();
                        var temp = piece.node.currentPiece;

                        piece.node.currentPiece = currentPieceScript.gameObject;
                        currentPieceScript.node = piece.node;

                        currentPieceScript.x = piece.x;
                        currentPieceScript.y = piece.y;
                        currentPieceScript.RePositioning();

                        piece.node = lastNode;
                        lastNode.currentPiece = piece.gameObject;

                        /* �̶� ��ǥ�� Ȯ�������� ���� */


                        piece.x = lastNode.GridPos.x;
                        piece.y = lastNode.GridPos.y;
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
        /* ���� �Ǵ� ���� */
        piece.RePositioning();
        piece = null;
        MovementManager.instance.InvalidateAll();
        EffectManager.instance.ClearPlaceEffects();
        Debug.Log($"last = ({x},{y})");
        Debug.DrawRay(ray.origin, ray.direction, Color.red);

    }
}
