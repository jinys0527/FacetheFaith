using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapSelectionManager : MonoBehaviour
{
    [SerializeField] float moveTime = 0.4f;  
    [SerializeField] float sceneDelay = 0.6f;

    [Header("맵 X 아이콘 (고정)")]
    [SerializeField] private Material icon;

    Piece king;
    bool isMoving;

    void Start()
    {
        if (MapManager.instance.ismap)
        {
            king = MapManager.instance.piecePrefab.GetComponent<Piece>();
            //Debug.Log("왕 기물: " + king.name);
            //PieceControlManager.instance?.gameObject.SetActive(false);   // 혹시 모를 잔여 입력 제거
        }
    }

    void Update()
    {
        if (isMoving || !Input.GetMouseButtonDown(0)) return;

        if (EventSystem.current.IsPointerOverGameObject()) return;   // UI 위 클릭 차단

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("Node"))
        {
            Node node = hit.collider.GetComponent<Node>();

            
            Node curNode = king.node;   // 현재 위치
            
            bool isNextStage = node.stageIndex == MapManager.instance.currentStageNum + 1;
            bool isReachable = curNode != null && curNode.nextNodes.Contains(node);

            if (node.isStageNode && isNextStage && isReachable)
            {
                node.setMaterial(icon);
                StartCoroutine(MoveAndLoad(node));
            }
        }
    }



    IEnumerator MoveAndLoad(Node target)
    {
        isMoving = true;

        /* 1) 기물 부드럽게 이동 */
        Vector3 start = king.transform.position;
        Vector3 end = target.transform.position + Vector3.up * 0.01f;
        //Vector3 end = new Vector3 (target.transform.position.x, target.transform.position.y,-10) + Vector3.up * 0.01f;
        float t = 0;
        while (t < moveTime)
        {
            t += Time.deltaTime;
            king.transform.position = Vector3.Lerp(start, end, t / moveTime);

            yield return null;
        }
        king.transform.position = end;

        if (king.node != null)
        {
            king.node.currentPiece = null;
        }

        //  추가 2: 새 노드에 currentPiece 등록
        target.currentPiece = king.gameObject;

        //  추가 3: king 내부의 node 참조도 갱신
        king.node = target;


        /* 2) 스테이지 번호 갱신 */
        MapManager.instance.currentStageNum = target.stageIndex;

        /* 3) 잠시 대기 후 씬 전환 */
        yield return new WaitForSeconds(sceneDelay);

        GameState next =
            target.stageNodeType switch
            {
                StageNodeType.NormalBattle => GameState.Stage_Battle,
                StageNodeType.EliteBattle => GameState.Stage_Battle,
                StageNodeType.BossBattle => GameState.Stage_Battle,
                StageNodeType.Rest => GameState.Stage_Rest,
                StageNodeType.Treasure => GameState.Stage_Treasure,
                StageNodeType.Unknown => GameState.Stage_Random,
                _ => GameState.Map
            };
        Debug.Log("GM 접근 직전 : " + (GameManager.instance == null));

        //GameManager.instance.SetGameState(next); // 맵 로드 잠시 중단
        SceneChageManager.Instance.ChangeGameState(next); // 씬 전환
    }
}
