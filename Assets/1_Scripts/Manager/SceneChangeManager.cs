 using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class SceneChageManager : MonoBehaviour
{
    public static SceneChageManager Instance { get; private set; }

    [Header("마스크 이미지")]
    [SerializeField] RectTransform leftMask;
    [SerializeField] RectTransform rightMask;
    [SerializeField] GameObject touchMask;

    [Header("애니메이션")]
    [SerializeField] float animDuration = 0.8f;
    [SerializeField] float holdTime = 0.6f;

    Vector2 leftOffPos;   // 화면 밖
    Vector2 rightOffPos;  // 화면 밖

    float canvasWidth;
    private bool isTransitioning = false;

    void Awake()
    {
        touchMask.SetActive(false);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 처음 한 번만 외곽 위치 캐싱
        leftOffPos = leftMask.anchoredPosition;
        rightOffPos = rightMask.anchoredPosition;
    }

    /* 외부에서 호출할 단 하나의 메서드 */
    public void ChangeGameState(GameState nextState)
    {

        if (isTransitioning) return;
        SoundManager.Instance.PlaySFX(e_SFXname.s_Conversion);

        isTransitioning = true;

        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(DoTransition(() => GameManager.instance.SetGameState(nextState)));
    }

    /* 씬 이름으로 직접 로드하고 싶으면 오버로드 추가 */
    public void ChangeScene(string sceneName)
    {
        if (isTransitioning) return;

        isTransitioning = true;

        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(DoTransition(() => SceneManager.LoadScene(sceneName)));
    }

    /* 공통 애니메이션 수행 후 콜백 실행 */
    IEnumerator DoTransition(System.Action onComplete)
    {
        Debug.Log("DoTransition");
        Vector2 lStart = leftMask.anchoredPosition;
        Vector2 rStart = rightMask.anchoredPosition;

        Vector2 lEnd = new Vector2(0, lStart.y);
        Vector2 rEnd = new Vector2(0, rStart.y);
        touchMask.SetActive(true);
        float t = 0f;
        while (t < animDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0, 1, t / animDuration);
            leftMask.anchoredPosition = Vector2.Lerp(lStart, lEnd, k);
            rightMask.anchoredPosition = Vector2.Lerp(rStart, rEnd, k);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(holdTime);

        onComplete?.Invoke();
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 더 이상 중복 호출되지 않도록 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // 새 씬에서 슬라이드-아웃 코루틴 실행
        StartCoroutine(SlideOut());
    }
    IEnumerator SlideOut()
    {
        Vector2 centerLeft = leftMask.anchoredPosition;   // 현재 중앙
        Vector2 centerRight = rightMask.anchoredPosition;

        float t = 0f;
        while (t < animDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0, 1, t / animDuration);

            leftMask.anchoredPosition = Vector2.Lerp(centerLeft, leftOffPos, k);
            rightMask.anchoredPosition = Vector2.Lerp(centerRight, rightOffPos, k);

            yield return null;
        }
        isTransitioning = false;
        touchMask.SetActive(false);

    }

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    //{
    //    Debug.Log($"씬 로드 완료: {scene.name}, 로드 방식: {mode}");
    //    float t = 0f;
    //    while (t < animDuration)
    //    {
    //        t += Time.unscaledDeltaTime; // 델타타임 추가

    //        float k = Mathf.SmoothStep(0, 1, t / animDuration); // 0~1로 보간

    //        Vector2 lStart = leftMask.anchoredPosition; // 
    //        Vector2 rStart = rightMask.anchoredPosition;

    //        Vector2 lEnd = new Vector2(0, lStart.y);
    //        Vector2 rEnd = new Vector2(0, rStart.y);

    //        leftMask.anchoredPosition = Vector2.Lerp(lEnd, lStart, k);
    //        rightMask.anchoredPosition = Vector2.Lerp(rEnd, rStart, k);
    //    }
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}
}
