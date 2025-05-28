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
        if (MapManager.instance.ismap && MapManager.instance.piecePrefab != null)
        {
            king = MapManager.instance.piecePrefab.GetComponent<Piece>();
        }

        if (king == null)
        {
            Debug.LogError("king�� null�Դϴ�. MapManager ������ Ȯ���ϼ���.");
        }
    }

    void Update()
    {
        if (isMoving || !Input.GetMouseButtonDown(0)) return;

        if (EventSystem.current.IsPointerOverGameObject()) return;   // UI �� Ŭ�� ����

        Node clickNode = GetClickNode();
        if(clickNode == null) return;

        if (CanMoveTo(clickNode))
        {
            clickNode.setMaterial(icon);
            StartCoroutine(MoveAndLoad(clickNode));
        }
    }

    private Node GetClickNode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("Node") ? hit.collider.GetComponent<Node>() : null;
    }

    private bool CanMoveTo(Node node)
    {
        Node curNode = king.node;   // ���� ��ġ

        return node.isStageNode
            && node.stageIndex == MapManager.instance.currentStageNum + 1
            && curNode != null
            && curNode.nextNodes.Contains(node);
    }

    IEnumerator MoveAndLoad(Node target)
    {
        isMoving = true;

        /* 1) �⹰ �ε巴�� �̵� */
        Vector3 start = king.transform.position;
        Vector3 end = target.transform.position + Vector3.up * 0.01f;

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

        SceneChangeManager.Instance.ChangeGameState(next); // �� ��ȯ
    }
}
