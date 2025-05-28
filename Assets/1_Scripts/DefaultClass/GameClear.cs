using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClear : MonoBehaviour
{
    public Image clear_image1;
    public Image clear_image2;
    public Image clear_image3;

    private void OnEnable()
    {
        FadeInGameClear();
    }
    public void FadeInGameClear()
    {
        StartCoroutine(FadeInClearCoroutine());
        GameManager.instance.CurrentUIManager.isPopupOpen = true;
    }

    IEnumerator FadeInClearCoroutine()
    {
        float fadeDuration = 1.0f; // Fade duration in seconds
        float elapsedTime = 0f;
        Color color1 = clear_image1.color;
        Color color2 = clear_image2.color;
        Color color3 = clear_image3.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            color1.a = alpha;
            color2.a = alpha;
            color3.a = alpha;
            clear_image1.color = color1;
            clear_image2.color = color2;
            clear_image3.color = color3;
            yield return null;
        }
    }
}
