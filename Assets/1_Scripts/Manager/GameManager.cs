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
    public GameState currentState;

    bool sceneLoaded = true;
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
        SceneChageManager.Instance.ChangeGameState(GameState.Map);
    }

    void Update()
    {
        if(sceneLoaded)
            switch (currentState)
            {
                // GameState.BossBattle:
                //���⼭ Boss�� ������ �ٷ�ų� ���ų� break �� �ȽἭ battle state�� ���� update��
                case GameState.Stage_Battle:
                    if (PieceControlManager.instance != null)
                        if (PieceControlManager.instance.enabled)
                            PieceControlManager.instance.PieceControlUpdate();
                    if (BattleCardManager.BattleCardManagerInstance != null)
                        if (BattleCardManager.BattleCardManagerInstance.enabled)
                            BattleCardManager.BattleCardManagerInstance.CardUpdate();
                    
                    break;
                case GameState.Map:
                    if (PieceControlManager.instance != null)
                        if (PieceControlManager.instance.enabled)
                            PieceControlManager.instance.PieceControlUpdate();
                    break;
                //�ʿ��ϸ� �� �߰�
                default:
                    break;

            }

    }

    // ���� ���� �Լ�
    public void SetGameState(GameState newState)
    {
        currentState = newState;
        Debug.Log("Game State Changed to: " + currentState);
        sceneLoaded = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (BattleCardManager.BattleCardManagerInstance != null)
            if (BattleCardManager.BattleCardManagerInstance.enabled)
                BattleCardManager.BattleCardManagerInstance.enabled = false;


        // ���� ��ȯ�� ���� �߰� ó�� ����
        switch (newState)
        {
            case GameState.Title:
                Debug.Log("�� : MainMenu");
                MapManager.DestroyInstance();
                LoadScene("TITLE");
                break;
            case GameState.Playing:
                Debug.Log("�� : Synopsis");
                LoadScene("Synopsis");
                break;
            case GameState.Map:
                Debug.Log("�� : Map");
                SoundManager.Instance.PlayBGM(e_BGMname.s_Map);
                
                if (MapManager.instance != null)
                {
                    if (MapManager.instance.gameObject != null)
                    {
                        MapManager.instance.gameObject.SetActive(true); // MapManager ��Ȱ��ȭ
                        var kingObj = MapManager.instance.piecePrefab;
                        if (kingObj != null && kingObj.scene.IsValid())   // Missing ����
                            kingObj.SetActive(true);
                    }
                }
                LoadScene("MAP");
                break;

            case GameState.Stage_Battle:
                Debug.Log("�� : Battle");
                SoundManager.Instance.PlayBGM(e_BGMname.s_Battle);

                if (MapManager.instance != null) 
                {   
                    MapManager.instance.gameObject.SetActive(false); // MapManager ��Ȱ��ȭ
                    var kingObj = MapManager.instance.piecePrefab;
                    if (kingObj != null && kingObj.scene.IsValid())   // Missing ����
                        kingObj.SetActive(false);

                }
                BattleCardManager.BattleCardManagerInstance.enabled = true;

                LoadScene("STAGE_BATTLE");

                break;

                
            case GameState.Stage_Random:
                Debug.Log("�� : Event");
                if (MapManager.instance != null)
                {
                    MapManager.instance.gameObject.SetActive(false); // MapManager ��Ȱ��ȭ
                    var kingObj = MapManager.instance.piecePrefab;
                    if (kingObj != null && kingObj.scene.IsValid())   // Missing ����
                        kingObj.SetActive(false);

                }
                SceneManager.sceneLoaded -= OnSceneLoaded;

                int rand_value = UnityEngine.Random.Range(0, 3);
                if (rand_value == 0) { SetGameState(GameState.Stage_Rest); } // 1/3 Ȯ���� �޽�
                else if (rand_value == 1) { BattleCardManager.BattleCardManagerInstance.enabled = true; SetGameState(GameState.Stage_Battle); } // 1/3 Ȯ���� ����
                else { SetGameState(GameState.Stage_Treasure); } // 1/3 Ȯ���� ����
                break;

            case GameState.Stage_Rest:
                Debug.Log("�� : Rest");
                if (MapManager.instance != null)
                {
                    MapManager.instance.gameObject.SetActive(false); // MapManager ��Ȱ��ȭ
                    var kingObj = MapManager.instance.piecePrefab;
                    if (kingObj != null && kingObj.scene.IsValid())   // Missing ����
                        kingObj.SetActive(false);

                }
                LoadScene("STAGE_REST");
                break;

            case GameState.Stage_Treasure:
                Debug.Log("�� : Stage_Treasure");
                if (MapManager.instance != null)
                {
                    MapManager.instance.gameObject.SetActive(false); // MapManager ��Ȱ��ȭ
                    var kingObj = MapManager.instance.piecePrefab;
                    if (kingObj != null && kingObj.scene.IsValid())   // Missing ����
                        kingObj.SetActive(false);

                }
                LoadScene("Stage_Treasure");
                break;
        }
    }

    public void SetGameState(int state)
    {
        SetGameState((GameState)state);
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
        Debug.Log($"�� �ε� �Ϸ�: {scene.name}, �ε� ���: {mode}");
        sceneLoaded = true;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.instance.InitializeUIManagerForScene(currentState);
    }


}
