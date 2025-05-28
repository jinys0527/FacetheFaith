 using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance { get; private set; }

    [Header("����ũ �̹���")]
    [SerializeField] RectTransform leftMask;
    [SerializeField] RectTransform rightMask;
    [SerializeField] GameObject touchMask;

    [Header("�ִϸ��̼�")]
    [SerializeField] float animDuration = 0.8f;
    [SerializeField] float holdTime = 0.6f;

    Vector2 leftOffPos;   // ȭ�� ��
    Vector2 rightOffPos;  // ȭ�� ��

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

        // ó�� �� ���� �ܰ� ��ġ ĳ��
        leftOffPos = leftMask.anchoredPosition;
        rightOffPos = rightMask.anchoredPosition;
    }

    /* �ܺο��� ȣ���� �� �ϳ��� �޼��� */
    public void ChangeGameState(GameState nextState)
    {

        if (isTransitioning) return;
        SoundManager.Instance.PlaySFX(e_SFXname.s_Conversion);

        isTransitioning = true;

        SceneManager.sceneLoaded -= OnSceneLoaded; // �ߺ� ��� ����
        SceneManager.sceneLoaded += OnSceneLoaded;

        StartCoroutine(DoTransition(() => GameManager.instance.SetGameState(nextState)));
    }

    /* �� �̸����� ���� �ε��ϰ� ������ �����ε� �߰� */
    public void ChangeScene(string sceneName)
    {
        if (isTransitioning) return;

        isTransitioning = true;

        SceneManager.sceneLoaded -= OnSceneLoaded; // �ߺ� ��� ����
        SceneManager.sceneLoaded += OnSceneLoaded;

        StartCoroutine(DoTransition(() => SceneManager.LoadScene(sceneName)));
    }

    /* ���� �ִϸ��̼� ���� �� �ݹ� ���� */
    IEnumerator DoTransition(System.Action onComplete)
    {
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
        // �� �̻� �ߺ� ȣ����� �ʵ��� ����
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // �� ������ �����̵�-�ƿ� �ڷ�ƾ ����
        StartCoroutine(SlideOut());
    }

    IEnumerator SlideOut()
    {
        Vector2 centerLeft = leftMask.anchoredPosition;   // ���� �߾�
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
}
