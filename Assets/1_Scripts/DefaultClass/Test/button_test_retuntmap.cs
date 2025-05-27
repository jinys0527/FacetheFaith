using UnityEngine;
using UnityEngine.SceneManagement;

public class TestReturnButton : MonoBehaviour
{
    public void ReturnToMap()
    {
        MapManager.instance.gameObject.SetActive(true);
        MapManager.instance.piecePrefab.SetActive(true);

        GameManager.instance.SetGameState(GameState.Map);

    }
}
