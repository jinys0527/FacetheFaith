using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PieceControlManager : MonoBehaviour
{
    public static PieceControlManager instance;
    public static EffectManager effect;
    public static BossPatternManager bosspattern;

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

                // 같은 기물을 다시 클릭한 경우 → 무시
                if (selected == piece)
                    return;

                piece = selected;

                // 이동 노드 계산 후 이펙트 표시

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

        // 클릭한 대상이 기물이 아니면 선택 해제 + 이펙트 제거
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
        Node originNode = piece.node; //  이동 전 위치 저장

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

            /* 기존 필터 로직 --------------------------------------------------- */
            if (x != -1 && y != -1)
            {
                if (node.GridPos.y > y) continue;
                if ((node.GridPos.x <= 3 && x > node.GridPos.x) ||
                    (node.GridPos.x >= 3 && x < node.GridPos.x))
                    continue;
            }

            /* 장애물 / 아군 차단 ---------------------------------------------- */
            if (BoardManager.instance != null) // 보드매니저있으면 아래
            {
                if (BoardManager.instance.IsBlocked(node.GridPos))
                {
                    blocked = true;
                    x = node.GridPos.x;
                    y = node.GridPos.y;
                    continue; // 막혔으면 더 진행 안 함
                }
            }
            else { // 없으면 맵매니저 있을테니까 여기
                if (MapManager.instance.IsBlocked(node.GridPos))
                {
                    blocked = true;
                    x = node.GridPos.x;
                    y = node.GridPos.y;
                    continue; // 막혔으면 더 진행 안 함
                }
            }


                /* 빈 칸 발견 => 후보로 저장 (하지만 아직 piece 좌표는 건드리지 않음) */
                blocked = false;
            x = node.GridPos.x;
            y = node.GridPos.y;
            lastNode = node;
        }

        /* ---------------------------------------------------------------------- */
        /* 이동 가능성 최종 확인 + 좌표/노드 교체                                */
        /* ---------------------------------------------------------------------- */
        if (lastNode != null && !blocked && piece.canMove && CanMove)
        {
            var moves = MovementManager.instance.GetMoves(piece);
            if (moves.Contains(lastNode))          // 합법 이동
            {
                SoundManager.Instance.PlaySFX(e_SFXname.s_Piece_Move);
                /* 노드 링크 교체 */
                if (piece.node != null)
                    piece.node.currentPiece = null;

                lastNode.currentPiece = piece.gameObject;
                piece.node = lastNode;

                /* 이때 좌표를 확정적으로 갱신 */
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


        /* 스냅 또는 원복 */
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

            /* 기존 필터 로직 --------------------------------------------------- */
            if (x != -1 && y != -1)
            {
                if (node.GridPos.y > y) continue;
                if ((node.GridPos.x <= 3 && x > node.GridPos.x) ||
                    (node.GridPos.x >= 3 && x < node.GridPos.x))
                    continue;
            }

            /* 장애물 / 아군 차단 ---------------------------------------------- */
            if (BoardManager.instance.IsBlocked(node.GridPos))
            {
                blocked = true;
            }
            else
                blocked = false;
            /* 빈 칸 발견 => 후보로 저장 (하지만 아직 piece 좌표는 건드리지 않음) */
            x = node.GridPos.x;
            y = node.GridPos.y;
            lastNode = node;
        }

        /* ---------------------------------------------------------------------- */
        /* 이동 가능성 최종 확인 + 좌표/노드 교체                                */
        /* ---------------------------------------------------------------------- */
        if (lastNode != null && !blocked)
        {
            List<Node> moves = BoardManager.instance.GetFrontTwoRaws();
            if (moves.Contains(lastNode) && (BattlePieceManager.instance.pieces.Count <= 5 || (BattlePieceManager.instance.pieces.Count == 6 && piece.node != null)))          // 합법 이동
            {
                /* 노드 링크 교체 */
                if (piece.node != null)
                    piece.node.currentPiece = null;

                lastNode.currentPiece = piece.gameObject;
                piece.node = lastNode;

                /* 이때 좌표를 갱신 */
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
            Debug.Log("잘됨");
            if (piece.node != lastNode)
            {
                List<Node> moves = BoardManager.instance.GetFrontTwoRaws();
                if (moves.Contains(lastNode))          // 합법 이동
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

                        /* 이때 좌표를 확정적으로 갱신 */
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

                        /* 이때 좌표를 확정적으로 갱신 */


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
        /* 스냅 또는 원복 */
        piece.RePositioning();
        piece = null;
        MovementManager.instance.InvalidateAll();
        EffectManager.instance.ClearPlaceEffects();
        Debug.Log($"last = ({x},{y})");
        Debug.DrawRay(ray.origin, ray.direction, Color.red);

    }
}
