using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapSelectionManager : MonoBehaviour
{
    [SerializeField] float moveTime = 0.4f;  
    [SerializeField] float sceneDelay = 0.6f;

    [Header("�� X ������ (����)")]
    [SerializeField] private Material icon;

    Piece king;
    bool isMoving;

    void Start()
    {
        if (MapManager.instance.ismap)
        {
            king = MapManager.instance.piecePrefab.GetComponent<Piece>();
            //Debug.Log("�� �⹰: " + king.name);
            //PieceControlManager.instance?.gameObject.SetActive(false);   // Ȥ�� �� �ܿ� �Է� ����
        }
    }

    void Update()
    {
        if (isMoving || !Input.GetMouseButtonDown(0)) return;

        if (EventSystem.current.IsPointerOverGameObject()) return;   // UI �� Ŭ�� ����

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("Node"))
        {
            Node node = hit.collider.GetComponent<Node>();

            
            Node curNode = king.node;   // ���� ��ġ
            
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

        /* 1) �⹰ �ε巴�� �̵� */
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

        //  �߰� 2: �� ��忡 currentPiece ���
        target.currentPiece = king.gameObject;

        //  �߰� 3: king ������ node ������ ����
        king.node = target;


        /* 2) �������� ��ȣ ���� */
        MapManager.instance.currentStageNum = target.stageIndex;

        /* 3) ��� ��� �� �� ��ȯ */
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
        Debug.Log("GM ���� ���� : " + (GameManager.instance == null));

        //GameManager.instance.SetGameState(next); // �� �ε� ��� �ߴ�
        SceneChageManager.Instance.ChangeGameState(next); // �� ��ȯ
    }
}
