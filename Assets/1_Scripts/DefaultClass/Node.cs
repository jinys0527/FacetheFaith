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
    NormalBattle,   // �Ϲ����� 
    Rest,           // �޽�(����)
    Treasure,       // ������
    Unknown,        // ����

    EliteBattle,    // ��������
    BossBattle,     // ����
    Road,           // �ε�
    King,

}

public class Node : MonoBehaviour
{
    public Vector2Int GridPos { get; private set; }
    public NodeColorType ColorType { get; private set; }
    
    public StageNodeType stageNodeType = StageNodeType.Road; // �⺻���� Road

    public List<Node> nextNodes = new();

    public eMonsterAttackType monsterAttackType;    // ���� ���� Ÿ�� ���夤

    [SerializeField] MeshRenderer meshRenderer;// ���� ���׸���
    [SerializeField] MeshRenderer frontRenderer;  // ����� ���׸���

    [Header("����")]
    public GameObject currentPiece;         // �⹰�� �ִ� �� ����
    public bool isObstacle;                 // ��ֹ�����
    public bool isStageNode = false;

    public int stageIndex = -1; // 1~10 �������� ��ȣ, -1�� �������� ���
    public string sceneName;  // �� ��带 Ŭ���ϸ� ��ȯ�� �� �̸�

    public void Init(Vector2Int pos, NodeColorType color, Material mat, Material frontMat) //��� �ʱ�ȭ �Լ� 
    {
        GridPos = pos;
        ColorType = color;
        currentPiece = null;
        isObstacle = false;
        isStageNode = false;
        meshRenderer.material = mat;   // ���޹��� ���׸��� ����
        frontRenderer.material = frontMat;  // ���� ���׸��� ����
    }
    
    public void SetStageNode(StageNodeType style) //����� ��Ÿ���� ���� = ����� �������� Ÿ���� ����
    {
        isStageNode = true;
        stageNodeType = style;
        gameObject.SetActive(true);
    }
    
    public void setMaterial(Material mat) 
    {
        transform.GetChild(1).GetComponent<MeshRenderer>().material = mat;
    }

    void OnMouseEnter() // ���콺 Ŀ���� ��忡 �ö���� ��
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return; //  UI ���� Ŀ���� ������ ����

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
