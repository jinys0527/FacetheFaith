using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Image gameoverBackground_image;
    public Image gameoverText_image;
    public Image gameoverButton_image;

    private void OnEnable()
    {
        FadeInGameOver();
    }
    public void FadeInGameOver()
    {
        StartCoroutine(FadeInCoroutine());
        BaseUIManager.Instance.isPopupOpen = true;
    }

    IEnumerator FadeInCoroutine()
    {
        float fadeDuration = 1.0f; // Fade duration in seconds
        float elapsedTime = 0f;
        Color color1 = gameoverBackground_image.color;
        Color color2 = gameoverText_image.color;
        Color color3 = gameoverButton_image.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            color1.a = alpha;
            color2.a = alpha;
            color3.a = alpha;
            gameoverBackground_image.color = color1;
            gameoverText_image.color = color2;
            gameoverButton_image.color = color3;
            yield return null;
        }
    }
}
