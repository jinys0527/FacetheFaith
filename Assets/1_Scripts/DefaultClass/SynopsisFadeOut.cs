using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynopsisFadeOut : MonoBehaviour
{
    public GameObject[] synopsisObjects;
    public float firstWaitTime = 3.5f; // 다음 컷으로 넘어가기 전 대기 시간
    public float fadeOutTime = 1.0f;
    public float waitBetween = 0.5f; // 다음 컷으로 넘어가기 전 대기 시간


    private void Awake()
    {
        foreach (GameObject obj in synopsisObjects)
        {
            obj.SetActive(true);

            // 안전하게 CanvasGroup 추가
            if (obj.GetComponent<CanvasGroup>() == null)
            {
                obj.AddComponent<CanvasGroup>();
            }

            obj.GetComponent<CanvasGroup>().alpha = 1f;
        }
    }

    void Start()
    {
        StartCoroutine(FadeOutSequence());
    }

    private IEnumerator FadeOutSequence()
    {
        int count = 0;
        foreach (GameObject obj in synopsisObjects)
        {
            count++;
            CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();

            if (count == 1)
            {
                // 첫 화면만 잠깐 더 보여주기
                yield return new WaitForSeconds(firstWaitTime);  // 원하는 대기 시간
            }

            if (count < synopsisObjects.Length) // 마지막 컷 제외
            {
                float t = 0f;
                while (t < fadeOutTime)
                {
                    t += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutTime);
                    yield return null;
                }

                canvasGroup.alpha = 0f;
                obj.SetActive(false);
                yield return new WaitForSeconds(waitBetween);
            }
            else
            {
                yield return new WaitForSeconds(0.1f); // 마지막 컷 유지 시간
                SceneChageManager.Instance.ChangeGameState(GameState.Map);
            }
        }
    }


}
