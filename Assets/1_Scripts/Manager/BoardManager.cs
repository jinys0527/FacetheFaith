using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BoardManager : MonoBehaviour, IBoard
{
    public static BoardManager instance;
    public static TestSpawner testSpawner;

    public bool IsInitialized => grid != null;

    [Header("Node Prefab")]
    [SerializeField] private GameObject nodePrefab;

    [Header("Tile Materials")]
    [Tooltip("흰색 계열 5개")]
    [SerializeField] private Material[] whiteMats = new Material[5];

    [Tooltip("회색 계열 5개")]
    [SerializeField] private Material[] grayMats = new Material[5];

    [Header("Front-Tile (고정)")]
    [SerializeField] Material frontWhiteMat;
    [SerializeField] Material frontBlackMat;   // 회색 노드용

    public List<Node> frontTwoRows = new List<Node>();

    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        GenerateBoard();
    }

    public int width = 7;
    public int height = 7;

    private Node[,] grid;

    public int Width => width; // 값 내보내기
    public int Height => height;
    [SerializeField] private float nodeSize = 1f;

    public float GetNodeSize()
    {
        return nodeSize;
    }

    void GenerateBoard() // 체스판 만드는 놈임
    {
        frontTwoRows.Clear(); // 재생성 시 중복 방지

        grid = new Node[Width, Height];

        float offsetX = (Width - 1) * 0.5f;
        int frontRowLimit = 2;
        int inactiveRowStart = Height - 2; // 하드코딩 5 대신 유동적 처리

        for (int y = 0; y < Height; y++) //y 가 h -1,-2일 때 비활성화 x 5,6
        {
            for (int x = 0; x < Width; x++)
            {
                var world = new Vector3((x - offsetX) * nodeSize, 0, y - inactiveRowStart);
                Node node = Instantiate(nodePrefab, world, Quaternion.identity, transform)
                                .GetComponent<Node>();
                node.transform.localScale = new Vector3(nodeSize, 0.5f, 1);

                bool isWhite = (x + y) % 2 == 1;
                Material mat = isWhite
                    ? whiteMats[Random.Range(0, whiteMats.Length)]
                    : grayMats[Random.Range(0, grayMats.Length)];

                Material frontmat = isWhite
                    ? frontWhiteMat
                    : frontBlackMat;

                node.Init(new Vector2Int(x, y),
                          isWhite ? NodeColorType.White : NodeColorType.Gray,
                          mat, frontmat);

                int currentStageNum = MapManager.instance != null ? MapManager.instance.currentStageNum : 1;

                if ((x == 0 || x == Width - 1 || y >= inactiveRowStart) && currentStageNum < 5)
                {
                    node.gameObject.SetActive(false); // 6,6 비활성화
                }

                grid[x, y] = node;

                if (y < frontRowLimit && node.gameObject.activeSelf)
                {
                    frontTwoRows.Add(node);
                }
            }
        }   
    }

    public List<Node> GetFrontTwoRows()
    {
        return frontTwoRows;
    }

    public Node GetNode(Vector2Int pos)
    {
        return grid[pos.x, pos.y];
    }

    public bool IsBlocked(Vector2Int pos) // 이동 가능 여부 체크 
    {
        Node node = GetNode(pos);
        return node == null || node.IsOccupied()|| !node.gameObject.activeSelf;
    }
}
