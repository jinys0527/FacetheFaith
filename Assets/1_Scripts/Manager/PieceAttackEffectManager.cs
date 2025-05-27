using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PieceAttackEffectManager : MonoBehaviour
{
    public GameObject target;
    public float moveSpeed = 10f; // 속도 조절용 변수

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Monster");
    }

    public void FinishEffect()
    {
        Destroy(gameObject, 0.5f);
    }
   
    public void Update()
    {
        if(gameObject.CompareTag("Effect"))
        {
            if (target == null) return;

            // 목표 지점 설정
            Vector3 targetPos = target.transform.position + Vector3.up * 9f;

            // 시간 기반 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }
}
