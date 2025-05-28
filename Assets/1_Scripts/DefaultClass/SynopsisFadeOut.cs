using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynopsisFadeOut : MonoBehaviour
{
    public GameObject[] synopsisObjects;
    public float firstWaitTime = 3.5f; // ���� ������ �Ѿ�� �� ��� �ð�
    public float fadeOutTime = 1.0f;
    public float waitBetween = 0.5f; // ���� ������ �Ѿ�� �� ��� �ð�


    private void Awake()
    {
        foreach (GameObject obj in synopsisObjects)
        {
            obj.SetActive(true);

            // �����ϰ� CanvasGroup �߰�
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
                // ù ȭ�鸸 ��� �� �����ֱ�
                yield return new WaitForSeconds(firstWaitTime);  // ���ϴ� ��� �ð�
            }

            if (count < synopsisObjects.Length) // ������ �� ����
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
                yield return new WaitForSeconds(0.1f); // ������ �� ���� �ð�
                SceneChangeManager.Instance.ChangeGameState(GameState.Map);
            }
        }
    }


}
