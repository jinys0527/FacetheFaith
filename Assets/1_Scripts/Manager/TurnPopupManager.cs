using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TurnPopupManager : MonoBehaviour
{
    public static TurnPopupManager Instance { get; private set; }

    public Image playerTurn_image;
    public Image enemyTurn_image;

    public float fadeDuration = 1.0f;
    public float showDuration = 1.5f;

    private void Awake()
    {
        // 싱글톤 패턴 처리
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 초기 상태는 비활성 (투명)
        SetAlpha(playerTurn_image, 0f);
        SetAlpha(enemyTurn_image, 0f);
    }

    public void ShowPlayerTurn()
    {
        StartCoroutine(FadeSequence(playerTurn_image));
    }

    public void ShowEnemyTurn()
    {
        StartCoroutine(FadeSequence(enemyTurn_image));
    }

    public IEnumerator FadeSequence(Image img)
    {
        yield return StartCoroutine(FadeIn(img));
        yield return new WaitForSeconds(showDuration);
        yield return StartCoroutine(FadeOut(img));
    }

    private IEnumerator FadeIn(Image img)
    {
        float t = 0f;
        Color color = img.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            img.color = color;
            yield return null;
        }
        color.a = 1f;
        img.color = color;
        BaseUIManager.Instance.isPopupOpen = true;
    }

    private IEnumerator FadeOut(Image img)
    {
        float t = 0f;
        Color color = img.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            img.color = color;
            yield return null;
        }
        color.a = 0f;
        img.color = color;
        BaseUIManager.Instance.isPopupOpen = false;
    }

    private void SetAlpha(Image img, float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }
}
