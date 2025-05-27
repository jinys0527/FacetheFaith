using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ImageClickHandler : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    RectTransform rectTransform;
    Image myImage;

    public float left;
    public float right;
    public bool isOpen = false;

    public Vector3 targetPosition = new Vector3(-450, 0, 0); // 목표 위치
    private float moveSpeed = 200.0f;       // 이동 속도
    private float bounceHeight = 3f;    // 튀는 높이
    private float bounceDamping = 0;   // 튀는 감쇠 계수

    private bool reachedTarget = true;
    private float bounceVelocity = 0.0f;
    private float currentHeight = 0.0f;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        myImage = GetComponent<Image>();
    }

    void Update()
    {
        if (!reachedTarget)
        {
            // 이동 중
            moveSpeed += Time.deltaTime * 1500;
            rectTransform.anchoredPosition = Vector3.MoveTowards(rectTransform.anchoredPosition, targetPosition, (moveSpeed * Time.deltaTime));

            // 도달 여부 확인
            if (targetPosition.x == left)
            {
                if (rectTransform.anchoredPosition.x <= targetPosition.x)
                {
                    reachedTarget = true;
                    moveSpeed = moveSpeed/700;
                    bounceDamping = Mathf.Floor(moveSpeed * 100) / 100f;
                    bounceHeight = bounceDamping;
                }
            }
            else
            {
                if (rectTransform.anchoredPosition.x >= targetPosition.x)
                {
                    reachedTarget = true;
                    moveSpeed = moveSpeed / 700;
                    bounceDamping = Mathf.Floor(moveSpeed * 100) / 100f;
                    bounceHeight = bounceDamping;
                }
            }
        }
        else if (bounceDamping != 0)
        {
            // 통통 튀는 중 (Y 방향만)
            currentHeight += bounceVelocity;
            bounceVelocity -= 9f * Time.deltaTime; // 중력 효과

            if (currentHeight < 0)
            {
                currentHeight = 0;
                bounceDamping -= bounceHeight/3 + 0.05f;
                bounceVelocity = bounceDamping; // 튐 + 감쇠
            }

            // 적용
            Vector3 bouncePos;
            if(targetPosition.x == left)
                bouncePos = targetPosition + Vector3.right * currentHeight;
            else
                bouncePos = targetPosition + Vector3.left * currentHeight;
            rectTransform.anchoredPosition = bouncePos;

            if (bounceDamping <= 0)
                bounceDamping = 0;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (clickedObject == gameObject)
        {
            myImage.color = new Color(0.7f, 0.7f, 0.7f, 1);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (clickedObject == gameObject)
        {
            isOpen = !isOpen;
            reachedTarget = false;
            if (targetPosition.x == left)
                targetPosition.x = right;
            else 
                targetPosition.x = left;
            moveSpeed = 200;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        myImage.color = new Color(1, 1, 1, 1);
    }

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = !isOpen;
            reachedTarget = false;
            if (targetPosition.x == left)
                targetPosition.x = right;
            else
                targetPosition.x = left;
            moveSpeed = 200;
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            isOpen = !isOpen;
            reachedTarget = false;
            if (targetPosition.x == left)
                targetPosition.x = right;
            else
                targetPosition.x = left;
            moveSpeed = 200;
        }
    }
}
