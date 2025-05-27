using System.Collections.Generic;
using UnityEngine;

/// ���� �� ��� �⹰�� �̵� ���� ������ ��ꡤĳ���Ѵ�.
public class MovementManager : MonoBehaviour
{
    public static MovementManager instance;

    //   Piece �̵� ���� ��� ���  (�� �ϸ��� ����)
    private readonly Dictionary<Piece, List<Node>> cache = new(); 
    // ��ųʸ��� �⹰�� �̵� ���� ��� ����Ʈ ����� >> Ű�� �⹰ ã�� ������ �̵� ���� ��� ����Ʈ ��ȯ��

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

    /* -------------------------------------------------------------------------- */
    /* ���� API                                                                    */
    /* -------------------------------------------------------------------------- */

    public List<Node> GetMoves(Piece piece) // Ű �ѱ�� ��ųʸ����� �̵� ���� ��� ����Ʈ ��ȯ
    {
        if (!cache.ContainsKey(piece))
            cache[piece] = CalcMoves(piece);

        return cache[piece];
    }


    public void InvalidateAll() => cache.Clear();

    /* -------------------------------------------------------------------------- */
    /* ���� : �̵� ��Ģ                                                            */
    /* -------------------------------------------------------------------------- */

    private List<Node> CalcMoves(Piece p) // getmoves���� ȣ�� ��!! ���� �θ��°� �ƴ�
    {
        switch (p.pieceVariant)                     //  switch-case �ٽ�
        {
            case PieceVariant.Rook: return RayMoves(p, rookDirs);
            case PieceVariant.Bishop: return RayMoves(p, bishopDirs);
            case PieceVariant.Queen: return RayMoves(p, queenDirs);
            case PieceVariant.Knight: return KnightMoves(p);
            case PieceVariant.Pawn: return PawnMoves(p);   // ������ ������ ��Ģ�� �°�
            case PieceVariant.King: return KingMoves(p);
            default: return new();          // King ���� �ʿ�� �߰�
        }
    }

    /* ------------------------  ���⡤���� ��ƾ  --------------------------------- */

    // ����/�밢 ������(Ray)�� �̵���
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
                    cur += d;                                 // ���� �������� ����
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
                    cur += d;                                 // ���� �������� ����
                    count++;
                }
            }
        }
        return list;
    }

    //  ���(Knight) ���� ������
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

    //  �� ���� �� ĭ ������
    private List<Node> PawnMoves(Piece p)
    {
        if (BoardManager.instance != null)
        {
            List<Node> list = new();
            Vector2Int front = new(p.x, p.y + 1);   //  ������ ���ʻ��̶� ����
            if (IsFree(front)) list.Add(BoardManager.instance.GetNode(front));
            return list;
        }
        else
        {
            List<Node> list = new();
            Vector2Int front = new(p.x, p.y - 1);   //  ������ ���ʻ��̶� ����
            if (IsFree(front)) list.Add(MapManager.instance.GetNode(front));
            return list;
        }
    }

    private List<Node> KingMoves(Piece p)
    {
        if (p.node == null) return new(); // ŷ�� ��忡 �Ҽӵ��� ���� ���

        return new List<Node>(p.node.nextNodes); // �׳� ����� nextNodes�� ��ȯ
    }


    /* ------------------------  ���� üũ --------------------------------------- */

    private bool IsFree(Vector2Int pos)
    {
        if (BoardManager.instance != null)
        {
            int w = BoardManager.instance.Width, h = BoardManager.instance.Height;
            if (pos.x < 0 || pos.x >= w || pos.y < 0 || pos.y >= h) return false;
            return !BoardManager.instance.IsBlocked(pos);      // ��ֹ�,�Ʊ� �⹰�� ����
        }
        else {
            int w = MapManager.instance.Width, h = MapManager.instance.Height;
            if (pos.x < 0 || pos.x >= w || pos.y < 0 || pos.y >= h) return false;
            return !MapManager.instance.IsBlocked(pos);      // ��ֹ�,�Ʊ� �⹰�� ����
        }
    }
}
