using System.Collections.Generic;
using UnityEngine;



public class BoardManager : MonoBehaviour
{


    public static BoardManager instance;
    public static TestSpawner testSpawner;

    public bool IsInitialized => grid != null;

    [Header("Node Prefab")]
    [SerializeField] private GameObject nodePrefab;

    [Header("Tile Materials")]
    [Tooltip("��� �迭 5��")]
    [SerializeField] private Material[] whiteMats = new Material[5];

    [Tooltip("ȸ�� �迭 5��")]
    [SerializeField] private Material[] grayMats = new Material[5];

    [Header("Front-Tile (����)")]
    [SerializeField] Material frontWhiteMat;
    [SerializeField] Material frontBlackMat;   // ȸ�� ����

    public List<Node> frontTwoRows = new List<Node>();



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
        GenerateBoard();
    }


    public int width = 7;
    public int height = 7;

    private Node[,] grid;

    public int Width => width; // �� ��������
    public int Height => height;
    [SerializeField] private float nodeSize = 1f;

    public float GetNodeSize()
    {
        return nodeSize;
    }


    void GenerateBoard() // ü���� ����� ����
    {
        grid = new Node[Width, Height];

        float offsetX = (Width - 1) * 0.5f;

        for (int y = 0; y < Height; y++) //y �� h -1,-2�� �� ��Ȱ��ȭ x 5,6
            for (int x = 0; x < Width; x++)
            {
                var world = new Vector3((x - offsetX)*nodeSize, 0, y - 5);
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

                int num;
                if (MapManager.instance != null)
                {
                    num = MapManager.instance.currentStageNum;
                }
                else { // ������
                    num = 1;
                }
                
                if ((x == 0 || x == 6 || y >= 5)&& num<5)
                {
                    node.gameObject.SetActive(false); // 6,6 ��Ȱ��ȭ
                } 



                grid[x, y] = node;
                if (y < 2 && node.gameObject.activeSelf)
                {
                    frontTwoRows.Add(node);
                }

            }


    }

    public List<Node> GetFrontTwoRaws()
    {
        return frontTwoRows;
    }

    public Node GetNode(Vector2Int pos)
    {
        return grid[pos.x, pos.y];
    }

    public bool IsBlocked(Vector2Int pos) // �̵� ���� ���� üũ 
    {
        Node node = GetNode(pos);
        return node == null || node.IsOccupied()|| !node.gameObject.activeSelf;

    }


}
