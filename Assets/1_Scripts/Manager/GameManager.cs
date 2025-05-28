using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public enum GameState
{
    Title, // Title
    Playing, // ?
    Map,
    Stage_Battle,
    Stage_Random, // Stage_Random
    Stage_Rest, // Stage_Rest
    Stage_Treasure//Stage_Treasure �߰� ����
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public BaseUIManager CurrentUIManager { get; private set; }

    public VideoPlayer videoPlayer;
    public VideoClip clip;

    //public GameState currentState { get; private set; }
    public GameState currentState = GameState.Title;

    bool sceneLoaded = true;

    private static readonly Dictionary<GameState, string> SceneNames = new Dictionary<GameState, string>
    {
        { GameState.Title, "Title" },
        { GameState.Playing, "Synopsis" },
        { GameState.Map, "MAP" },
        { GameState.Stage_Battle, "Stage_Battle" },
        { GameState.Stage_Rest, "STAGE_REST" },
        { GameState.Stage_Treasure, "STAGE_TREASURE" }
    };

    private void Awake()
    {
        // �̱��� �ʱ�ȭ
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }

        if (videoPlayer != null && clip != null)
        {
            videoPlayer.clip = clip;
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Play();
        }
        InitializeUIManagerForScene(currentState);
    }

    public void InitializeUIManagerForScene(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Title:
                CurrentUIManager = FindObjectOfType<TitleUIManager>();
                break;
            case GameState.Map:
                CurrentUIManager = FindObjectOfType<MapUIManager>();
                break;
            case GameState.Stage_Battle:
                CurrentUIManager = FindObjectOfType<BattleUIManager>();
                break;
            case GameState.Stage_Rest:
                CurrentUIManager = FindObjectOfType<RestUIManager>();
                break;
        }

        CurrentUIManager?.Initialize();
        CurrentUIManager?.SetupButtonEvents();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("���� ��� �Ϸ�");
        SceneChangeManager.Instance.ChangeGameState(GameState.Map);
    }

    void Update()
    {
        if (sceneLoaded)
            switch (currentState)
            {
                case GameState.Stage_Battle:
                    if (PieceControlManager.instance?.enabled == true)
                    {
                        PieceControlManager.instance.PieceControlUpdate();
                    }
                    if (BattleCardManager.instance?.enabled == true)
                    {
                        BattleCardManager.instance.CardUpdate();
                    }
                    break;

                default:
                    break;
            }

    }

    void SetBattleCardManagerActive(bool active)
    {
        if (BattleCardManager.instance != null)
            BattleCardManager.instance.enabled = active;
    }

    // ���� ���� �Լ�
    public void SetGameState(GameState newState)
    {
        currentState = newState;
 
        sceneLoaded = false;
        SceneManager.sceneLoaded += OnSceneLoaded;

        SetBattleCardManagerActive(false);

        // ���� ��ȯ�� ���� �߰� ó�� ����
        switch (newState)
        {
            case GameState.Title:
                HandleTitleState();
                break;
            case GameState.Playing:
                HandlePlayingState();
                break;
            case GameState.Map:
                HandleMapState();
                break;

            case GameState.Stage_Battle:
                HandleBattleState();
                break;

            case GameState.Stage_Random:
                HandleRandomState();
                break;

            case GameState.Stage_Rest:
                HandleRestState();
                break;

            case GameState.Stage_Treasure:
                HandleTreasureState();
                break;
        }
    }

    public void SetGameState(int state) => SetGameState((GameState)state);

    private void LoadSceneByState(GameState state)
    {
        if (SceneNames.TryGetValue(state, out string sceneName))
        {
            LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning($"[GameManager] GameState {state}�� ���� �� �̸��� ��ϵ��� �ʾҽ��ϴ�.");
        }
    }

    void HandleTitleState()
    {
        MapManager.DestroyInstance();
        LoadSceneByState(GameState.Title);
    }

    void HandlePlayingState()
    {
        LoadSceneByState(GameState.Playing);
    }

    void HandleMapState()
    {
        SoundManager.Instance.PlayBGM(e_BGMname.s_Map, true);
        SetMapManagerActive(true);
        LoadSceneByState(GameState.Map);
    }

    void HandleBattleState()
    {
        SoundManager.Instance.PlayBGM(e_BGMname.s_Battle, true);
        SetMapManagerActive(false);
        SetBattleCardManagerActive(true);
        LoadSceneByState(GameState.Stage_Battle);
    }

    void HandleRestState()
    {
        SetMapManagerActive(false);
        LoadSceneByState(GameState.Stage_Rest);
    }

    void HandleRandomState()
    {
        SetMapManagerActive(false);
        SceneManager.sceneLoaded -= OnSceneLoaded;

        int rand = Random.Range(0, 3);
        if (rand == 0) SetGameState(GameState.Stage_Rest);
        else if (rand == 1) { SetBattleCardManagerActive(true); SetGameState(GameState.Stage_Battle); }
        else SetGameState(GameState.Stage_Treasure);
    }

    void HandleTreasureState()
    {
        SetMapManagerActive(false);
        LoadSceneByState(GameState.Stage_Treasure);
    }

    void SetMapManagerActive(bool isActive)
    {
        if (MapManager.instance != null)
        {
            MapManager.instance.gameObject.SetActive(isActive);
            var kingObj = MapManager.instance.piecePrefab;
            if (kingObj != null && kingObj.scene.IsValid())
                kingObj.SetActive(isActive);
        }
    }

    // �� �̸����� �ε�
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f; // ������ ���� ������ �簳
        SceneManager.LoadScene(sceneName);
    }

    // �� �ε����� �ε�
    public void LoadScene(int index)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(index);
    }

    // ���� �� �ٽ� �ε�
    public void ReloadScene()
    {
        Time.timeScale = 1f;
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneLoaded = true;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.instance.InitializeUIManagerForScene(currentState);
    }
}
