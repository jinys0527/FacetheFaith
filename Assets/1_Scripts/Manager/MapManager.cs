
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;



public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public static TestSpawner testSpawner;
    public GameObject KingPrefab;
    public GameObject piecePrefab;
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

    [Header("�� ������ (����)")]
    [SerializeField] private Material[] icon = new Material[7];

    [Header("���������� ���� ��� ������")]
    [SerializeField] private Material[] head_icon = new Material[7];

    public List<Node> frontTwoRows = new List<Node>();
    public int pathCount = 6;         // ��� ��
    public bool ismap = true; // true�� ���� ����
    private HashSet<Node> revealedPathNodes = new(); // �̹� ������ ��� ���� = ���� ��忡�� ���� ��� ���� �� ������ Road ����
    
    public int y_terms = 4; // y������ ��ĭ���� �������� Ÿ�� �ο����� �����ϴ� ����
    public int x_terms = 4; // x������ ��ĭ���� �������� Ÿ�� �ο����� �����ϴ� ����

    public float x_range = 0.5f; // x������ �������� �̵��� ����
    public float y_range = 0.5f; // y������ �������� �̵��� ����

    public float distance = 1f; // ��� �� �Ÿ�

    public int currentStageNum = 0; // ���� �������� ��ȣ -1 �� ����ó��
    string stageName; // �� �̸� (Stage_Battle,Stage_Random, Stage_Rest, Stage_Treasure)

    public Node selectedNode;  // ���������� ���õ� ��� ����

    Material dotMat;     // �� ���� �ε� & ĳ��

    public GameObject gameover_gameobj;

    private void Awake() // ���Ѱܵ� ���ϼ��� �����Ǵ°���
                         // ���� �Ŵ����� 
    {
        dotMat = Resources.Load<Material>("Materials/DotLineMat");
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (ismap)
            {
                if (grid == null)        // ���� ���� ���� �� ���� ����
                    GenerateBoard();

                if (piecePrefab == null || piecePrefab.Equals(null))
                    mapgener();

                DrawConnections();

                if (piecePrefab != null)
                {
                    Piece king = piecePrefab.GetComponent<Piece>();
                    RevealNextNodesOnly(king.node);
                }

                currentStageNum = 0;
            }

            if (gameover_gameobj == null)
            {
                gameover_gameobj = GameObject.FindWithTag("GameOver");
            }
            gameover_gameobj?.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int width = 9;
    public int height = 41;

    private Node[,] grid;

    public int Width => width; // �� ��������
    public int Height => height;
    public float nodeSize = 1f;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log($"�� �ε� �Ϸ�: {scene.name}, �ε� ���: {mode}");

        Camera.main.gameObject.GetComponent<CameraFollowKing>().FollowKing();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (gameover_gameobj == null)
        {
            gameover_gameobj = GameObject.FindWithTag("GameOver");
        }

        gameover_gameobj?.SetActive(false);
    }

    public int GetStageNum()
    {
        return currentStageNum; // ���� �������� ��ȣ ��������
    }
    
    void GenerateBoard()
    {
        grid = new Node[Width, Height];

        float offsetX = (Width - 1) * 0.5f;

        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                var world = new Vector3((x - offsetX) * nodeSize, 0, y - 5);
                Node node = Instantiate(nodePrefab, world, Quaternion.identity, transform)
                                .GetComponent<Node>();
                node.transform.localScale = new Vector3(nodeSize, 0, 1* nodeSize);
                //node.GetComponent<MeshRenderer>().materials = null;

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

                grid[x, y] = node;
                if (y < 2)
                {
                    frontTwoRows.Add(node);
                }
                var mr = node.GetComponent<Renderer>();
            }
    }

    public Node GetNode(Vector2Int pos)
    {
        return grid[pos.x, pos.y];
    }

    public bool IsBlocked(Vector2Int pos) // �̵� ���� ���� üũ 
    {
        Node node = GetNode(pos);
        return node == null || node.IsOccupied();

    }

    public void mapgener()// ������� ü���ǿ� Ư�� ��帶�� �������� Ÿ�� �ο�
    {

        if (piecePrefab != null)
        {
            Debug.Log(" ŷ �̹� ������ - mapgener() ���� �� ��");
            return;
        }


        foreach (var off_node in grid)
        {
            if (off_node != null)
                off_node.gameObject.SetActive(false);
        }
        int pathLength = Height - 1;
        int boardWidth = Width;
        int boardHeight = Height;
        StageNodeType type;
        Material mat;

        Dictionary<int, List<float>> usedXPerStageY = new(); // ���������� ���� x ��ǥ ����Ʈ


        for (int path = 0; path < pathCount; path++)
        {
            int center = Width / 2;

            int x = center; // ���� x ����
            int y = 0;

            Vector2Int pos = new Vector2Int(center, 0); // �÷��̾� ��ġ�� �߾����� ����
            Node node = GetNode(pos); // ��� ��������
            int stage = 0; // �������� ��ȣ

            if (piecePrefab == null || piecePrefab.Equals(null))
            {
                type = StageNodeType.King;

                // ��� ��������
                node.SetStageNode(type);

                //  ��� ��ġ�� �����ϵ�, z���� ��¦ ����
                Vector3 spawnPos = node.transform.position + new Vector3(0, 0, 0);
                //Vector3 spawnPos = node.transform.position + new Vector3(0, 0, -10);

                piecePrefab = Instantiate(KingPrefab, spawnPos, Quaternion.Euler(60,0,0));
                Debug.Log("ŷ ������");
                //node.setMaterial(icon[0]); // ŷ ��� ���׸��� ����
                node.transform.Find("Icon").GetComponent<MeshRenderer>().material = icon[0]; // ŷ ��� ���׸��� ����
                                                                                             //node.setMaterial(icon[0]);
                node.SetTileVisible(false);   //  Ÿ�� ����


                SetInitialAlpha(piecePrefab, 0f); // �⹰ ����ȭ

                // ���� �⹰ ���� ����
                node.currentPiece = piecePrefab;
                piecePrefab.GetComponent<Piece>().node = node;

                // DontDestroy ó��
                DontDestroyOnLoad(piecePrefab);
            }

            CameraFollowKing camFollow = Camera.main.GetComponent<CameraFollowKing>();
            if (camFollow != null)
            {
                camFollow.target = piecePrefab.transform;
            }

            Node prevNode = node;// ���� ���

            for (int step = 0; step < pathLength; step++)
            {
                if (path % 2 == 0)
                {
                    x += x_terms * Random.Range(-1, 2); // -4, 0, +4

                }
                else
                {
                    x += x_terms / 2 * Random.Range(-1, 2); // -2, 0, +2
                }
                y++;

                if ((y / y_terms) % 2 == 0)
                    x = Mathf.Clamp(x, 2, 6);  // ¦�� ���������� �߾� �������� ����
                else
                    x = Mathf.Clamp(x, 0, boardWidth - 1);  // Ȧ�� ���������� ��ü ���� ���

                y = Mathf.Clamp(y, 0, boardHeight - 1);

                if (y % y_terms == 0)
                {

                    // ���� �������� Ÿ��
                    if (y == y_terms * 5)
                    {
                        x = center;
                        type = StageNodeType.EliteBattle;
                        stageName = "Stage_Battle";
                        mat = icon[4];
                    }
                    else if (y == y_terms * 7)
                    {
                        type = StageNodeType.Treasure;
                        stageName = "Stage_Treasure";
                        mat = icon[5];
                    }
                    else if (y == y_terms * 10)
                    {
                        x = center;
                        type = StageNodeType.BossBattle;
                        stageName = "Stage_Battle";
                        mat = icon[6];
                    }
                    else
                    {
                        int rand = Random.Range(0, 100); // 0~99

                        if (rand < 53)
                        {
                            stageName = "Stage_Battle";
                            type = StageNodeType.NormalBattle; // �Ϲ����� Ȯ�� 53%
                            mat = icon[1];

                        }
                        else if (rand < 70)
                        {
                            stageName = "Stage_Rest";
                            type = StageNodeType.Rest; // ���� Ȯ�� 17%
                            mat = icon[2];
                        }
                        else
                        {
                            stageName = "Stage_Random";
                            type = StageNodeType.Unknown; // ���� Ȯ�� 30%
                            mat = icon[3];
                        }
                    }

                    pos = new Vector2Int(x, y);
                    node = GetNode(pos);

                    if (node.GridPos.y != 0 && node.GridPos.y != 20 && node.GridPos.y != 28 && node.GridPos.y != 40)

                    {
                        float baseX = node.transform.position.x;
                        float xOffset, zOffset;
                        float newX;
                        int tries = 0;
                        do
                        {
                            xOffset = Random.Range(-x_range, x_range);
                            zOffset = Random.Range(-y_range, y_range);
                            newX = baseX + xOffset;
                            tries++;
                            //Debug.Log("�ݺ�Ƚ�� : " + tries);
                            if (tries > 50) break; // ���ѷ��� ����
                        }
                        while (

                            usedXPerStageY.ContainsKey(y) &&
                            usedXPerStageY[y].Exists(prevX => Mathf.Abs(prevX - newX) < distance)
                        );

                        node.transform.position = new Vector3(newX, 0f, node.transform.position.z + zOffset);
                        if (!usedXPerStageY.ContainsKey(y))
                            usedXPerStageY[y] = new List<float>();
                        usedXPerStageY[y].Add(newX);
                    }
                    node.SetStageNode(type);
                    node.SetTileVisible(false);   //  Ÿ�� ����
                    //node.setMaterial(mat); // �������� Ÿ�� ��Ƽ���� ����  sprite
                    node.transform.Find("Icon").GetComponent<MeshRenderer>().material = mat; // ŷ ��� ���׸��� ����
                    stage++;
                    node.stageIndex = stage; // ���� �������� ��ȣ �ο�
                    node.sceneName = stageName; // �� �̸� �ο�
                    prevNode.nextNodes.Add(node); // ���� ��� �߰�
                    prevNode = node; // ���� ��带 ���� ��忡 ����

                }
            }
        }
    }

    public void RevealNextNodesOnly(Node current)
    {
        int yTarget = current.GridPos.y + 8;

        // ���� ��� ǥ�� + ���
        current.gameObject.SetActive(true);
        revealedPathNodes.Add(current);

        // ���� ���� ǥ�� + ���
        foreach (var next in current.nextNodes)
        {
            if (next != null)
            {
                next.gameObject.SetActive(true);
                revealedPathNodes.Add(next);
            }
        }

        // y+8 ��� ǥ�� + ���
        if (yTarget < Height)
        {
            for (int x = 0; x < Width; x++)
            {
                Node node = grid[x, yTarget];
                if (node != null && node.isStageNode && !revealedPathNodes.Contains(node))
                {
                    node.gameObject.SetActive(true);
                    revealedPathNodes.Add(node);
                }
            }
        }
    }

    public void DrawConnections()
    {
        float yOffset = 0.001f;   // �ٴڿ��� ��¦ ����
        float nodeR = 0.4f;       // ��� ������(= ���� ���� �Ÿ�)
        float margin = 0.2f;

        foreach (var node in grid)
        {
            if (node == null || node.nextNodes == null) continue;

            foreach (var next in node.nextNodes)
            {
                // ���� ���
                Vector3 a = node.transform.position;
                Vector3 b = next.transform.position;
                Vector3 dir = (b - a).normalized;

                // ��� �ܰ����� shrink
                Vector3 p0 = a + dir * (nodeR + margin) + Vector3.up * yOffset;
                Vector3 p1 = b - dir * (nodeR + margin) + Vector3.up * yOffset;

                // LineRenderer ����
                GameObject lineObj = new GameObject("Line");
                lineObj.transform.SetParent(transform, false);

                LineRenderer lr = lineObj.AddComponent<LineRenderer>();
                lr.positionCount = 2;
                lr.SetPosition(0, p0);
                lr.SetPosition(1, p1);
                lr.startWidth = lr.endWidth = 0.1f;

                // �ʼ�: ������ ����
                lr.textureMode = LineTextureMode.Tile;
                lr.alignment = LineAlignment.View;  // ī�޶� ���� ���
                lr.numCapVertices = 0;

                // ��Ƽ���� ����
                Material mat = new Material(Shader.Find("Sprites/Default"));
                mat.color = Color.white;

                Texture2D tex = Resources.Load<Texture2D>("TestImage/DottedLineTexture");
                if (tex != null)
                {
                    mat.mainTexture = tex;
                    lr.material = mat;

                    // �ٽ�: LineRenderer.material�� ���� ����
                    lr.material.mainTextureScale = new Vector2(2f, 1f);  // �ؽ�ó �ݺ� �� ������ Ȯ���� �������� ����
                }
                else
                {
                    Debug.LogWarning("DottedLineTexture2 �ؽ�ó�� Resources/TestImage/ ��ο��� ã�� �� �����ϴ�.");
                }
            }
        }
    }

    public void RestoreKingToSelectedNode()
    {
        if (selectedNode != null && piecePrefab != null)
        {
            piecePrefab.transform.position = selectedNode.transform.position + new Vector3(0, 0, 0f);

            Piece king = piecePrefab.GetComponent<Piece>();
            if (king != null)
            {
                king.node = selectedNode; // �Ǵ� king.currentNode = selectedNode; �� Piece Ŭ������ ����
            }

            selectedNode.currentPiece = piecePrefab;
        }
    }

    public static void DestroyInstance()
    {
        if (instance == null) return;

        //  ŷ �����յ� ���� ����
        if (instance.piecePrefab != null)
            Destroy(instance.piecePrefab);

        Destroy(instance.gameObject);   // MapManager ������Ʈ
        instance = null;
    }

    void SetInitialAlpha(GameObject root, float alpha)
    {
        // MeshRenderer, SkinnedMeshRenderer, SpriteRenderer ��� ����
        foreach (var r in root.GetComponentsInChildren<Renderer>())
        {

            if (r.material.HasProperty("_Color"))
            {
                Color c = r.material.color;
                c.a = alpha;
                r.material.color = c;
            }
        }
    }
}
