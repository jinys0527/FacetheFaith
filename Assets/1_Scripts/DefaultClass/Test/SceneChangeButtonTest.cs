using UnityEngine;

public class SceneChangeButtonTest : MonoBehaviour
{
    public GameState gameState;

    public void Press()
    {
        if (GameManager.instance.currentState == GameState.Title)
        {
            // ���� �ʱ�ȭ �ڵ� �״�� 
            PlayerManager.instance.ResetPieces();
            PlayerManager.instance.pawnCount = 1;
            PlayerManager.instance.knightCount = 1;
            PlayerManager.instance.bishopCount = 1;

            var list = BattleCardManager.BattleCardManagerInstance.initDeckIndices;
            list.Clear();
            list.AddRange(new[] { 0, 0, 1, 1, 3, 3, 8, 10, 11, 12 });

            SoundManager.Instance.StartFadeOut(SoundManager.Instance.bgmSource, 2.0f);
        }

        BattleCardManager.BattleCardManagerInstance.FinishBattle();

        BaseUIManager.Instance.isPopupOpen = false;
        /* GameState �� ���� �ٲٴ� ��� TransitionManager ���� �ѱ�� */
        SceneChageManager.Instance.ChangeGameState(gameState);
    }
}
