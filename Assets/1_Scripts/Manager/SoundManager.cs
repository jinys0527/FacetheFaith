using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public enum e_SFXname
{
    s_Boss_Appears = 0,
    s_Button_Click = 1,
    s_Card_Buff,
    s_Card_Draw,
    s_Card_Use,
    s_Clear,
    s_Conversion,
    s_Death,
    s_Enemy_Appears,
    s_Enemy_Attack,
    s_Enemy_Die,
    s_Hits_Damage_1,
    s_Hits_Damage_2,
    s_Piece_Broken,
    s_Piece_Effect_Bishop,
    s_Piece_Effect_Knight,
    s_Piece_Effect_Rook,
    s_Piece_Effect_Pawn,
    s_Piece_Effect_Queen,
    s_Piece_Move,
    s_Select,
    s_Turn
}

public enum e_BGMname
{
    s_Battle = 0,
    s_Map,
    s_synopsis,
    s_Title,
}

public class SoundManager : MonoBehaviour
{
    public string bgmSourceName;
    public string[] sfxSourceName = new string[3];

    public string mainSliderName;
    public string bgmSliderName;
    public string sfxSliderName;

    public static SoundManager Instance;

    public AudioSource bgmSource;
    private const int SFX_SOURCE_POOL_SIZE = 3;
    private AudioSource[] sfxSource = new AudioSource[SFX_SOURCE_POOL_SIZE];
    private AudioSource currentSfxSource;

    public AudioClip[] bgmClip;
    public AudioClip[] sfxClip;

    public Slider mainVolumSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private int sfxIndex = 0;

    public GameObject settingCanvas;
    public GameObject canvas;
    public GameObject optionCanvas;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 변경 감지
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if (settingCanvas == null)
        {
            settingCanvas = GameObject.FindWithTag("Setting");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayRegisterButtons());
        if (settingCanvas == null && GameManager.instance.currentState == GameState.Title)
        {
            settingCanvas = GameObject.FindWithTag("Setting");
        }
        if (optionCanvas == null && GameManager.instance.currentState == GameState.Map)
        {
            optionCanvas = GameObject.Find("Option_canvas");
        }
    }

    void FindSettingCanvas()
    {
        if (settingCanvas == null && GameManager.instance.currentState == GameState.Title)
        {
            settingCanvas = GameObject.FindWithTag("Setting");
        }
        if (optionCanvas == null && GameManager.instance.currentState == GameState.Map)
        {
            canvas = GameObject.Find("Canvas_Overlay");
            optionCanvas = canvas.transform.Find("Option_canvas").gameObject;
        }
        if (settingCanvas != null)
        {
            settingCanvas?.SetActive(false);
        }
        if (optionCanvas != null)
        {
            optionCanvas?.SetActive(false);
        }
    }

    IEnumerator DelayRegisterButtons()
    {
        // UI 로딩이 끝날 때까지 한 프레임 대기
        yield return null;
        RegisterAllButtons();
        FindSettingCanvas();
        FindSliderAndSource();

        PlayBGM(e_BGMname.s_Title);
        FadeIn(1.5f, bgmSource);
    }

    public void FindSliderAndSource()
    {
        bgmSource = GameObject.Find(bgmSourceName)?.GetComponent<AudioSource>();

        for (int i = 0; i < sfxSourceName.Length; i++)
        {
            sfxSource[i] = GameObject.Find(sfxSourceName[i])?.GetComponent<AudioSource>();
        }

        if (GameManager.instance.currentState == GameState.Title)
        {
            mainVolumSlider = settingCanvas.transform.Find(mainSliderName)?.GetComponent<Slider>();
            bgmSlider = settingCanvas.transform.Find(bgmSliderName)?.GetComponent<Slider>();
            sfxSlider = settingCanvas.transform.Find(sfxSliderName)?.GetComponent<Slider>();
        }
        else if (GameManager.instance.currentState == GameState.Map)
        {
            mainVolumSlider = optionCanvas.transform.Find(mainSliderName)?.GetComponent<Slider>();
            bgmSlider = optionCanvas.transform.Find(bgmSliderName)?.GetComponent<Slider>();
            sfxSlider = optionCanvas.transform.Find(sfxSliderName)?.GetComponent<Slider>();
        }
    }

    public void SetMasterVolume(float value)
    {
        if (bgmSource != null && mainVolumSlider != null && bgmSlider != null)
            bgmSource.volume = mainVolumSlider.value * bgmSlider.value;

        foreach (var source in sfxSource)
        {
            if (source != null)
                source.volume = mainVolumSlider.value * sfxSlider.value;
        }
    }

    public void SetSFXVolume(float value)
    {
        foreach (var source in sfxSource)
        {
            if (source != null)
                source.volume = mainVolumSlider.value * sfxSlider.value;
        }
    }

    public void SetBGMVolume(float value)
    {
        if (bgmSource != null)
            bgmSource.volume = mainVolumSlider.value * bgmSlider.value;
    }

    public void StartFadeIn(AudioSource targetSource, float duration)
    {
        StartCoroutine(FadeIn(duration, targetSource));
    }

    public void StartFadeOut(AudioSource targetSource, float duration)
    {
        StartCoroutine(FadeOut(duration, targetSource));
    }

    IEnumerator FadeIn(float duration, AudioSource targetSource)
    {
        float startVolume = 0f;
        float targetVolume = targetSource.volume;
        targetSource.volume = startVolume;
        targetSource.Play();

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            targetSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
            yield return null;
        }
        targetSource.volume = targetVolume;
    }

    IEnumerator FadeOut(float duration, AudioSource targetSource)
    {
        float startVolume = targetSource.volume;
        float targetVolume = 0f;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            targetSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
            yield return null;
        }
        targetSource.volume = targetVolume;
        targetSource.Stop();
        targetSource.volume = 1f;
    }

    public void PlaySFX(e_SFXname clipName)
    {
        currentSfxSource = sfxSource[(sfxIndex + 1) % SFX_SOURCE_POOL_SIZE];
        currentSfxSource.PlayOneShot(sfxClip[(int)clipName]);
        sfxIndex++;
    }

    public void PlayBGM(e_BGMname clipName, bool fade = false)
    {
        if (fade)
        {
            StartCoroutine(FadeOutAndSwitchBGM(clipName));
        }
        else
        {
            bgmSource.clip = bgmClip[(int)clipName];
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    private IEnumerator FadeOutAndSwitchBGM(e_BGMname newClip)
    {
        yield return StartCoroutine(FadeOut(1f, bgmSource));
        bgmSource.clip = bgmClip[(int)newClip];
        yield return StartCoroutine(FadeIn(1f, bgmSource));
    }

    void RegisterAllButtons()
    {
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();

        foreach (Button btn in buttons)
        {
            if (btn.name == "TurnEndButton")
                continue;

            // 클릭 이벤트 등록
            btn.onClick.RemoveListener(OnAnyButtonClicked);
            btn.onClick.AddListener(OnAnyButtonClicked);

            // 마우스 오버 이벤트 등록
            AddHoverSoundEvent(btn);
        }
    }

    void AddHoverSoundEvent(Button button)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        // 기존 hover 이벤트 제거
        trigger.triggers.RemoveAll(e => e.eventID == EventTriggerType.PointerEnter);

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((eventData) => { PlaySFX(e_SFXname.s_Select); });

        trigger.triggers.Add(entry);
    }


    void OnAnyButtonClicked()
    {
        if (!GameManager.instance.CurrentUIManager.isPopupOpen)
        {
            PlaySFX(e_SFXname.s_Button_Click);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
