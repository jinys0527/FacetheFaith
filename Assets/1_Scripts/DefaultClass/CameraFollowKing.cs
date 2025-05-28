using UnityEngine;

public class CameraFollowKing : MonoBehaviour
{
    public Transform target;        // ���� ��� (ŷ)
    public Vector3 offset = new Vector3(0, 0, -1); // z-1 �ڿ��� ����
    public float followSpeed = 5f;  // �ε巴�� ���󰡴� �ӵ�

    public void FollowKing()
    {
        target = GameObject.FindGameObjectWithTag("King").transform;
        if (target == null) return;

        Vector3 desiredPosition;
        desiredPosition.z = target.position.z + offset.z;
        desiredPosition.y = target.position.y + offset.y;

        transform.position = new Vector3(transform.position.x , desiredPosition.y, desiredPosition.z);
    }
}
