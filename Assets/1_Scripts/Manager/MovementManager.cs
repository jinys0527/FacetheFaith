using System.Collections.Generic;
using UnityEngine;

/// 보드 위 모든 기물의 이동 가능 영역을 계산·캐싱한다.
public class MovementManager : MonoBehaviour
{
    public static MovementManager instance;

    //   Piece 이동 가능 노드 목록  (한 턴마다 갱신)
    private readonly Dictionary<Piece, List<Node>> cache = new(); 
    // 딕셔너리에 기물별 이동 가능 노드 리스트 저장됨 >> 키로 기물 찾고 값으로 이동 가능 노드 리스트 반환됨

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
    }

    /* -------------------------------------------------------------------------- */
    /* 메인 API                                                                    */
    /* -------------------------------------------------------------------------- */

    public List<Node> GetMoves(Piece piece) // 키 넘기면 딕셔너리에서 이동 가능 노드 리스트 반환
    {
        if (!cache.ContainsKey(piece))
            cache[piece] = CalcMoves(piece);

        return cache[piece];
    }


    public void InvalidateAll() => cache.Clear();

    /* -------------------------------------------------------------------------- */
    /* 내부 : 이동 규칙                                                            */
    /* -------------------------------------------------------------------------- */

    private List<Node> CalcMoves(Piece p) // getmoves에서 호출 됨!! 직접 부르는거 아님
    {
        switch (p.pieceVariant)                     //  switch-case 핵심
        {
            case PieceVariant.Rook: return RayMoves(p, rookDirs);
            case PieceVariant.Bishop: return RayMoves(p, bishopDirs);
            case PieceVariant.Queen: return RayMoves(p, queenDirs);
            case PieceVariant.Knight: return KnightMoves(p);
            case PieceVariant.Pawn: return PawnMoves(p);   // “폰” 방향은 규칙에 맞게
            case PieceVariant.King: return KingMoves(p);
            default: return new();          // King 등은 필요시 추가
        }
    }

    /* ------------------------  방향·보조 루틴  --------------------------------- */

    // 직선/대각 ‘광선(Ray)’ 이동용
    static readonly Vector2Int[] rookDirs = { Vector2Int.up, Vector2Int.down,
                                                Vector2Int.left, Vector2Int.right };
    static readonly Vector2Int[] bishopDirs = { new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
    static readonly Vector2Int[] queenDirs = { Vector2Int.up,   Vector2Int.down,
                                                Vector2Int.left, Vector2Int.right,
                                                new(1,1), new(1,-1), new(-1,1), new(-1,-1) };

    private List<Node> RayMoves(Piece p, Vector2Int[] dirs)
    {
        List<Node> list = new();
        if (BoardManager.instance != null)
        {
            foreach (var d in dirs)
            {
                Vector2Int cur = new Vector2Int(p.x, p.y) + d;

                int count = 0;
                int radius = p.GetMoveRadius();
                while (IsFree(cur) && count <= radius)
                {
                    list.Add(BoardManager.instance.GetNode(cur));
                    cur += d;                                 // 같은 방향으로 전진
                    count++;
                }
            }
        }
        else
        {
            foreach (var d in dirs)
            {
                Vector2Int cur = new Vector2Int(p.x, p.y) + d;
                int count = 0;
                int radius = p.GetMoveRadius();
                while (IsFree(cur) && count <= radius)
                {
                    list.Add(MapManager.instance.GetNode(cur));
                    cur += d;                                 // 같은 방향으로 전진
                    count++;
                }
            }
        }
        return list;
    }

    //  기사(Knight) 고정 오프셋
    static readonly Vector2Int[] knightOffsets =
    {
        new(1,2), new(2,1), new(-1,2), new(-2,1),
        new(1,-2), new(2,-1), new(-1,-2), new(-2,-1)
    };

    private List<Node> KnightMoves(Piece p)
    {
        List<Node> list = new();
        if (BoardManager.instance != null)
        {
            foreach (var off in knightOffsets)
            {
                Vector2Int pos = new Vector2Int(p.x, p.y) + off;
                if (IsFree(pos)) list.Add(BoardManager.instance.GetNode(pos));
            }
        }
        else
        {
            foreach (var off in knightOffsets)
            {
                Vector2Int pos = new Vector2Int(p.x, p.y) + off;
                if (IsFree(pos)) list.Add(MapManager.instance.GetNode(pos));
            }
        }
        return list;
    }

    //  폰 예시 한 칸 전진만
    private List<Node> PawnMoves(Piece p)
    {
        if (BoardManager.instance != null)
        {
            List<Node> list = new();
            Vector2Int front = new(p.x, p.y + 1);   //  진영이 한쪽뿐이라 가정
            if (IsFree(front)) list.Add(BoardManager.instance.GetNode(front));
            return list;
        }
        else
        {
            List<Node> list = new();
            Vector2Int front = new(p.x, p.y - 1);   //  진영이 한쪽뿐이라 가정
            if (IsFree(front)) list.Add(MapManager.instance.GetNode(front));
            return list;
        }
    }

    private List<Node> KingMoves(Piece p)
    {
        if (p.node == null) return new(); // 킹이 노드에 소속되지 않은 경우

        return new List<Node>(p.node.nextNodes); // 그냥 연결된 nextNodes를 반환
    }


    /* ------------------------  공통 체크 --------------------------------------- */

    private bool IsFree(Vector2Int pos)
    {
        if (BoardManager.instance != null)
        {
            int w = BoardManager.instance.Width, h = BoardManager.instance.Height;
            if (pos.x < 0 || pos.x >= w || pos.y < 0 || pos.y >= h) return false;
            return !BoardManager.instance.IsBlocked(pos);      // 장애물,아군 기물만 차단
        }
        else {
            int w = MapManager.instance.Width, h = MapManager.instance.Height;
            if (pos.x < 0 || pos.x >= w || pos.y < 0 || pos.y >= h) return false;
            return !MapManager.instance.IsBlocked(pos);      // 장애물,아군 기물만 차단
        }
    }
}
