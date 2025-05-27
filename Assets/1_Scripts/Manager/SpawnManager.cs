using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    //public GameObject spherePrefab; // 인스펙터에서 연결
    public BoardManager boardManager;
    public GameObject obstaclePrefab;
    public int obstacleCount = 1;
    public static SpawnManager instance;
    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }
    void Start()
    {
        SpawnRandomObstacle(obstacleCount);
    }


    public void SpawnRandomObstacle(int count)
    {
        int width = boardManager.Width;   // 7
        int height = boardManager.Height; // 7

        int placed = 0;
        int maxTry = 100;

        while (placed < count && maxTry-- > 0)
        {
            Vector2Int randPos = new Vector2Int(Random.Range(0, width), Random.Range(2, height));
            Node node = boardManager.GetNode(randPos);

            if (node != null && !node.IsOccupied())
            {
                var obj = Instantiate(obstaclePrefab);
                obj.transform.position = node.transform.position + new Vector3(0, 0.7f, 0);
                node.isObstacle = true;
                placed++;
            }
        }
    }

}
