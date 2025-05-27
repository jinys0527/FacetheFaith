using UnityEngine;

public class CameraFollowKing : MonoBehaviour
{
    public Transform target;        // 따라갈 대상 (킹)
    public Vector3 offset = new Vector3(0, 0, -1); // z-1 뒤에서 따라감
    public float followSpeed = 5f;  // 부드럽게 따라가는 속도

    public void FollowKing()
    {
        target = GameObject.FindGameObjectWithTag("King").transform;
        if (target == null) return;

        Vector3 desiredPosition;
        desiredPosition.z = target.position.z + offset.z;
        desiredPosition.y = target.position.y + offset.y;
        //transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x , desiredPosition.y, desiredPosition.z);
    }
}
