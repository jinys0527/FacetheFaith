using UnityEngine;

public class SceneChangeButtonTest : MonoBehaviour
{
    public GameState gameState;

    //public void Press()
    //{
    //    if (GameManager.instance.currentState == GameState.Title)
    //    {
    //        // 기존 초기화 코드 그대로 
    //        PlayerManager.instance.ResetPieces();
    //        PlayerManager.instance.pawnCount = 1;
    //        PlayerManager.instance.knightCount = 1;
    //        PlayerManager.instance.bishopCount = 1;

    //        var list = BattleCardManager.instance.initDeckIndices;
    //        list.Clear();
    //        list.AddRange(new[] { 0, 0, 1, 1, 3, 3, 8, 10, 11, 12 });

    //        SoundManager.Instance.StartFadeOut(SoundManager.Instance.bgmSource, 2.0f);
    //    }

    //    BattleCardManager.instance.FinishBattle();

    //    GameManager.instance.CurrentUIManager.isPopupOpen = false;
    //    /* GameState 를 직접 바꾸는 대신 TransitionManager 에게 넘긴다 */
    //    SceneChangeManager.Instance.ChangeGameState(gameState);
    //}
}
