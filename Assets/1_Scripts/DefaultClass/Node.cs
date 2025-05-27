using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum NodeColorType
{
    None,
    White,
    Gray
}

public enum StageNodeType
{
    NormalBattle,   // 일반전투 
    Rest,           // 휴식(상점)
    Treasure,       // 보물방
    Unknown,        // 미지

    EliteBattle,    // 정예전투
    BossBattle,     // 보스
    Road,           // 로드
    King,

}

public class Node : MonoBehaviour
{
    public Vector2Int GridPos { get; private set; }
    public NodeColorType ColorType { get; private set; }
    
    public StageNodeType stageNodeType = StageNodeType.Road; // 기본값은 Road

    public List<Node> nextNodes = new();

    public eMonsterAttackType monsterAttackType;    // 몬스터 공격 타입 저장ㄴ

    [SerializeField] MeshRenderer meshRenderer;// 전반 머테리얼
    [SerializeField] MeshRenderer frontRenderer;  // 전면부 머테리얼

    [Header("상태")]
    public GameObject currentPiece;         // 기물이 있는 지 여부
    public bool isObstacle;                 // 장애물인지
    public bool isStageNode = false;

    public int stageIndex = -1; // 1~10 스테이지 번호, -1은 비스테이지 노드
    public string sceneName;  // 이 노드를 클릭하면 전환할 씬 이름

    public void Init(Vector2Int pos, NodeColorType color, Material mat, Material frontMat) //노드 초기화 함수 
    {
        GridPos = pos;
        ColorType = color;
        currentPiece = null;
        isObstacle = false;
        isStageNode = false;
        meshRenderer.material = mat;   // 전달받은 머테리얼 적용
        frontRenderer.material = frontMat;  // 정면 머테리얼 적용
    }
    
    public void SetStageNode(StageNodeType style) //노드의 스타일을 설정 = 노드의 스테이지 타입을 설정
    {
        isStageNode = true;
        stageNodeType = style;
        gameObject.SetActive(true);
    }
    
    public void setMaterial(Material mat) 
    {
        transform.GetChild(1).GetComponent<MeshRenderer>().material = mat;
    }

    void OnMouseEnter() // 마우스 커서가 노드에 올라왔을 때
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return; //  UI 위에 커서가 있으면 무시

        if (GameManager.instance.currentState == GameState.Map)
            BaseUIManager.Instance.ShowStageInfo(stageNodeType);
    }

    public void SetTileVisible(bool visible)
    {
        if (meshRenderer != null)
            meshRenderer.enabled = visible;
    }

    public bool IsOccupied() => currentPiece != null || isObstacle;
}
