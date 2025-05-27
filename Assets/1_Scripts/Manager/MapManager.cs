
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
    [Tooltip("흰색 계열 5개")]
    [SerializeField] private Material[] whiteMats = new Material[5];

    [Tooltip("회색 계열 5개")]
    [SerializeField] private Material[] grayMats = new Material[5];

    [Header("Front-Tile (고정)")]
    [SerializeField] Material frontWhiteMat;
    [SerializeField] Material frontBlackMat;   // 회색 노드용

    [Header("맵 아이콘 (고정)")]
    [SerializeField] private Material[] icon = new Material[7];

    [Header("스테이지에 따른 상단 아이콘")]
    [SerializeField] private Material[] head_icon = new Material[7];

    public List<Node> frontTwoRows = new List<Node>();
    public int pathCount = 6;         // 경로 수
    public bool ismap = true; // true면 지도 상태
    private HashSet<Node> revealedPathNodes = new(); // 이미 밝혀진 경로 저장 = 현재 노드에서 다음 노드 선택 시 지나온 Road 저장
    
    public int y_terms = 4; // y축으로 몇칸마다 스테이지 타입 부여할지 결정하는 변수
    public int x_terms = 4; // x축으로 몇칸마다 스테이지 타입 부여할지 결정하는 변수

    public float x_range = 0.5f; // x축으로 랜덤으로 이동할 범위
    public float y_range = 0.5f; // y축으로 랜덤으로 이동할 범위

    public float distance = 1f; // 노드 간 거리

    public int currentStageNum = 0; // 현재 스테이지 번호 -1 은 예외처리
    string stageName; // 씬 이름 (Stage_Battle,Stage_Random, Stage_Rest, Stage_Treasure)

    public Node selectedNode;  // 마지막으로 선택된 노드 저장

    Material dotMat;     // 한 번만 로드 & 캐싱

    public GameObject gameover_gameobj;

    private void Awake() // 씬넘겨도 유일성이 보존되는거지
                         // 보드 매니저는 
    {
        dotMat = Resources.Load<Material>("Materials/DotLineMat");
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (ismap)
            {
                if (grid == null)        // 맵이 아직 생성 안 됐을 때만
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

    public int Width => width; // 값 내보내기
    public int Height => height;
    public float nodeSize = 1f;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log($"씬 로드 완료: {scene.name}, 로드 방식: {mode}");

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
        return currentStageNum; // 현재 스테이지 번호 가져오기
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

    public bool IsBlocked(Vector2Int pos) // 이동 가능 여부 체크 
    {
        Node node = GetNode(pos);
        return node == null || node.IsOccupied();

    }

    public void mapgener()// 만들어진 체스판에 특정 노드마다 스테이지 타입 부여
    {

        if (piecePrefab != null)
        {
            Debug.Log(" 킹 이미 존재함 - mapgener() 실행 안 함");
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

        Dictionary<int, List<float>> usedXPerStageY = new(); // 스테이지별 사용된 x 좌표 리스트


        for (int path = 0; path < pathCount; path++)
        {
            int center = Width / 2;

            int x = center; // 시작 x 고정
            int y = 0;

            Vector2Int pos = new Vector2Int(center, 0); // 플레이어 위치는 중앙으로 고정
            Node node = GetNode(pos); // 노드 가져오기
            int stage = 0; // 스테이지 번호

            if (piecePrefab == null || piecePrefab.Equals(null))
            {
                type = StageNodeType.King;

                // 노드 가져오기
                node.SetStageNode(type);

                //  노드 위치로 생성하되, z값만 살짝 조절
                Vector3 spawnPos = node.transform.position + new Vector3(0, 0, 0);
                //Vector3 spawnPos = node.transform.position + new Vector3(0, 0, -10);

                piecePrefab = Instantiate(KingPrefab, spawnPos, Quaternion.Euler(60,0,0));
                Debug.Log("킹 생성됨");
                //node.setMaterial(icon[0]); // 킹 노드 머테리얼 설정
                node.transform.Find("Icon").GetComponent<MeshRenderer>().material = icon[0]; // 킹 노드 머테리얼 설정
                                                                                             //node.setMaterial(icon[0]);
                node.SetTileVisible(false);   //  타일 숨김


                SetInitialAlpha(piecePrefab, 0f); // 기물 투명화

                // 노드와 기물 서로 연결
                node.currentPiece = piecePrefab;
                piecePrefab.GetComponent<Piece>().node = node;

                // DontDestroy 처리
                DontDestroyOnLoad(piecePrefab);
            }

            CameraFollowKing camFollow = Camera.main.GetComponent<CameraFollowKing>();
            if (camFollow != null)
            {
                camFollow.target = piecePrefab.transform;
            }

            Node prevNode = node;// 이전 노드

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
                    x = Mathf.Clamp(x, 2, 6);  // 짝수 스테이지는 중앙 라인으로 제한
                else
                    x = Mathf.Clamp(x, 0, boardWidth - 1);  // 홀수 스테이지는 전체 라인 허용

                y = Mathf.Clamp(y, 0, boardHeight - 1);

                if (y % y_terms == 0)
                {

                    // 고정 스테이지 타입
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
                            type = StageNodeType.NormalBattle; // 일반전투 확률 53%
                            mat = icon[1];

                        }
                        else if (rand < 70)
                        {
                            stageName = "Stage_Rest";
                            type = StageNodeType.Rest; // 상점 확률 17%
                            mat = icon[2];
                        }
                        else
                        {
                            stageName = "Stage_Random";
                            type = StageNodeType.Unknown; // 미지 확률 30%
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
                            //Debug.Log("반복횟수 : " + tries);
                            if (tries > 50) break; // 무한루프 방지
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
                    node.SetTileVisible(false);   //  타일 숨김
                    //node.setMaterial(mat); // 스테이지 타입 머티리얼 설정  sprite
                    node.transform.Find("Icon").GetComponent<MeshRenderer>().material = mat; // 킹 노드 머테리얼 설정
                    stage++;
                    node.stageIndex = stage; // 현재 스테이지 번호 부여
                    node.sceneName = stageName; // 씬 이름 부여
                    prevNode.nextNodes.Add(node); // 다음 노드 추가
                    prevNode = node; // 현재 노드를 이전 노드에 저장

                }
            }
        }
    }

    public void RevealNextNodesOnly(Node current)
    {
        int yTarget = current.GridPos.y + 8;

        // 현재 노드 표시 + 기록
        current.gameObject.SetActive(true);
        revealedPathNodes.Add(current);

        // 다음 노드들 표시 + 기록
        foreach (var next in current.nextNodes)
        {
            if (next != null)
            {
                next.gameObject.SetActive(true);
                revealedPathNodes.Add(next);
            }
        }

        // y+8 노드 표시 + 기록
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
        float yOffset = 0.001f;   // 바닥에서 살짝 띄우기
        float nodeR = 0.4f;       // 노드 반지름(= 선을 빼낼 거리)
        float margin = 0.2f;

        foreach (var node in grid)
        {
            if (node == null || node.nextNodes == null) continue;

            foreach (var next in node.nextNodes)
            {
                // 방향 계산
                Vector3 a = node.transform.position;
                Vector3 b = next.transform.position;
                Vector3 dir = (b - a).normalized;

                // 노드 외곽으로 shrink
                Vector3 p0 = a + dir * (nodeR + margin) + Vector3.up * yOffset;
                Vector3 p1 = b - dir * (nodeR + margin) + Vector3.up * yOffset;

                // LineRenderer 생성
                GameObject lineObj = new GameObject("Line");
                lineObj.transform.SetParent(transform, false);

                LineRenderer lr = lineObj.AddComponent<LineRenderer>();
                lr.positionCount = 2;
                lr.SetPosition(0, p0);
                lr.SetPosition(1, p1);
                lr.startWidth = lr.endWidth = 0.1f;

                // 필수: 점선용 설정
                lr.textureMode = LineTextureMode.Tile;
                lr.alignment = LineAlignment.View;  // 카메라 기준 평면
                lr.numCapVertices = 0;

                // 머티리얼 설정
                Material mat = new Material(Shader.Find("Sprites/Default"));
                mat.color = Color.white;

                Texture2D tex = Resources.Load<Texture2D>("TestImage/DottedLineTexture");
                if (tex != null)
                {
                    mat.mainTexture = tex;
                    lr.material = mat;

                    // 핵심: LineRenderer.material에 직접 설정
                    lr.material.mainTextureScale = new Vector2(2f, 1f);  // 텍스처 반복 수 높여야 확실히 점선으로 보임
                }
                else
                {
                    Debug.LogWarning("DottedLineTexture2 텍스처를 Resources/TestImage/ 경로에서 찾을 수 없습니다.");
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
                king.node = selectedNode; // 또는 king.currentNode = selectedNode; ← Piece 클래스에 따라
            }

            selectedNode.currentPiece = piecePrefab;
        }
    }

    public static void DestroyInstance()
    {
        if (instance == null) return;

        //  킹 프리팹도 같이 제거
        if (instance.piecePrefab != null)
            Destroy(instance.piecePrefab);

        Destroy(instance.gameObject);   // MapManager 오브젝트
        instance = null;
    }

    void SetInitialAlpha(GameObject root, float alpha)
    {
        // MeshRenderer, SkinnedMeshRenderer, SpriteRenderer 모두 대응
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
