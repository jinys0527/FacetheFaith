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
    Stage_Treasure//Stage_Treasure 추가 예정
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
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
        }
        else
        {
            Destroy(gameObject); // 중복 제거
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
        Debug.Log("영상 재생 완료");
        SceneChageManager.Instance.ChangeGameState(GameState.Map);
    }

    void Update()
    {
        if(sceneLoaded)
            switch (currentState)
            {
                // GameState.BossBattle:
                //여기서 Boss만 있을거 다루거나 말거나 break 문 안써서 battle state랑 같이 update함
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
                //필요하면 더 추가
                default:
                    break;

            }

    }

    // 상태 변경 함수
    public void SetGameState(GameState newState)
    {
        currentState = newState;
        Debug.Log("Game State Changed to: " + currentState);
        sceneLoaded = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (BattleCardManager.BattleCardManagerInstance != null)
            if (BattleCardManager.BattleCardManagerInstance.enabled)
                BattleCardManager.BattleCardManagerInstance.enabled = false;


        // 상태 전환에 따른 추가 처리 가능
        switch (newState)
        {
            case GameState.Title:
                Debug.Log("씬 : MainMenu");
                MapManager.DestroyInstance();
                LoadScene("TITLE");
                break;
            case GameState.Playing:
                Debug.Log("씬 : Synopsis");
                LoadScene("Synopsis");
                break;
            case GameState.Map:
                Debug.Log("씬 : Map");
                SoundManager.Instance.PlayBGM(e_BGMname.s_Map);
                
                if (MapManager.instance != null)
                {
                    if (MapManager.instance.gameObject != null)
                    {
                        MapManager.instance.gameObject.SetActive(true); // MapManager 비활성화
                        var kingObj = MapManager.instance.piecePrefab;
                        if (kingObj != null && kingObj.scene.IsValid())   // Missing 방지
                            kingObj.SetActive(true);
                    }
                }
                LoadScene("MAP");
                break;

            case GameState.Stage_Battle:
                Debug.Log("씬 : Battle");
                SoundManager.Instance.PlayBGM(e_BGMname.s_Battle);

                if (MapManager.instance != null) 
                {   
                    MapManager.instance.gameObject.SetActive(false); // MapManager 비활성화
                    var kingObj = MapManager.instance.piecePrefab;
                    if (kingObj != null && kingObj.scene.IsValid())   // Missing 방지
                        kingObj.SetActive(false);

                }
                BattleCardManager.BattleCardManagerInstance.enabled = true;

                LoadScene("STAGE_BATTLE");

                break;

                
            case GameState.Stage_Random:
                Debug.Log("씬 : Event");
                if (MapManager.instance != null)
                {
                    MapManager.instance.gameObject.SetActive(false); // MapManager 비활성화
                    var kingObj = MapManager.instance.piecePrefab;
                    if (kingObj != null && kingObj.scene.IsValid())   // Missing 방지
                        kingObj.SetActive(false);

                }
                SceneManager.sceneLoaded -= OnSceneLoaded;

                int rand_value = UnityEngine.Random.Range(0, 3);
                if (rand_value == 0) { SetGameState(GameState.Stage_Rest); } // 1/3 확률로 휴식
                else if (rand_value == 1) { BattleCardManager.BattleCardManagerInstance.enabled = true; SetGameState(GameState.Stage_Battle); } // 1/3 확률로 전투
                else { SetGameState(GameState.Stage_Treasure); } // 1/3 확률로 보물
                break;

            case GameState.Stage_Rest:
                Debug.Log("씬 : Rest");
                if (MapManager.instance != null)
                {
                    MapManager.instance.gameObject.SetActive(false); // MapManager 비활성화
                    var kingObj = MapManager.instance.piecePrefab;
                    if (kingObj != null && kingObj.scene.IsValid())   // Missing 방지
                        kingObj.SetActive(false);

                }
                LoadScene("STAGE_REST");
                break;

            case GameState.Stage_Treasure:
                Debug.Log("씬 : Stage_Treasure");
                if (MapManager.instance != null)
                {
                    MapManager.instance.gameObject.SetActive(false); // MapManager 비활성화
                    var kingObj = MapManager.instance.piecePrefab;
                    if (kingObj != null && kingObj.scene.IsValid())   // Missing 방지
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

    // 씬 이름으로 로드
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f; // 멈췄을 수도 있으니 재개
        SceneManager.LoadScene(sceneName);
    }

    // 씬 인덱스로 로드
    public void LoadScene(int index)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(index);
    }

    // 현재 씬 다시 로드
    public void ReloadScene()
    {
        Time.timeScale = 1f;
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"씬 로드 완료: {scene.name}, 로드 방식: {mode}");
        sceneLoaded = true;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.instance.InitializeUIManagerForScene(currentState);
    }


}
