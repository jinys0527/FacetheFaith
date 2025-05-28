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

    private Coroutine currentCoroutine;

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

    private void ShowTurn(Image image)
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(FadeSequence(image));
    }

    public IEnumerator FadeSequence(Image img)
    {
        GameManager.instance.CurrentUIManager.isPopupOpen = true;
        yield return StartCoroutine(FadeToAlpha(img, 1f));
        yield return new WaitForSeconds(showDuration);
        yield return StartCoroutine(FadeToAlpha(img, 0f));
        GameManager.instance.CurrentUIManager.isPopupOpen = false;
    }

    private IEnumerator FadeToAlpha(Image img, float targetAlpha)
    {
        float startAlpha = img.color.a;
        float t = 0f;
        Color color = img.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            img.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        img.color = new Color(color.r, color.g, color.b, targetAlpha);
    }

    private void SetAlpha(Image img, float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }
}
