using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : BaseUIManager
{
    [Header("Title Popups")]
    public GameObject settingPopup;
    public GameObject creditPopup;
    public GameObject quitGameCheckPopup;

    [Header("버튼")]
    [SerializeField] Button[] settingButton;
    [SerializeField] Button[] creditButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button startGameButton;
    [SerializeField] Button yesQuitButton;
    [SerializeField] Button noQuitButton;
    [SerializeField] Button[] backButton;

    public override void SetupButtonEvents()
    {
        // 메인 버튼들
        foreach(var setting in settingButton)
        {
            setting?.onClick.AddListener(ToggleSetting);
        }
        
        foreach(var credit in creditButton)
        {
            credit?.onClick.AddListener(ToggleCredit);
        }
        
        quitButton?.onClick.AddListener(ToggleQuitGame);
        startGameButton?.onClick.AddListener(() => Press(GameState.Playing));

        // 팝업 내부 버튼들
        yesQuitButton?.onClick.AddListener(QuitGame);
        noQuitButton?.onClick.AddListener(CloseAllPopups);
        foreach (var back in backButton)
        {
            back?.onClick.AddListener(CloseAllPopups);
        }
    }

    public override void Initialize()
    {
        SetupButtonEvents();
       
        settingPopup = GameObject.Find("Setting_canvas");
        creditPopup = transform.Find("Credit_canvas").gameObject;
        quitGameCheckPopup = transform.Find("QuitGameCheck_canvas").gameObject;
        CloseAllPopups();
    }

    public override void CloseAllPopups()
    {
        settingPopup?.SetActive(false);
        creditPopup?.SetActive(false);
        quitGameCheckPopup?.SetActive(false);
        isPopupOpen = false;
    }

    public void StartGame()
    {
        // 게임 초기화
        PlayerManager.instance.ResetPieces();
        var list = BattleCardManager.BattleCardManagerInstance.initDeckIndices;
        list.Clear();
        list.AddRange(new[] { 0, 0, 1, 1, 3, 3, 8, 10, 11, 12 });

        // 씬 전환
        SceneChageManager.Instance.ChangeGameState(GameState.Map);
    }

    public void ToggleSetting() => TogglePopup(settingPopup);
    public void ToggleCredit() => TogglePopup(creditPopup);
    public void ToggleQuitGame() => TogglePopup(quitGameCheckPopup);

    private void TogglePopup(GameObject popup)
    {
        CloseAllPopups();
        popup?.SetActive(true);
        isPopupOpen = true; 
    }

    public void QuitGame() => Application.Quit();
}
