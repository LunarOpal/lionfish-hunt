using UnityEngine;

public class LionfishSpawner : MonoBehaviour
{
    public GameObject lionfishPrefab;
    private GameManager gameManager;
    private BoxCollider2D spawnArea;

    public float spawnInterval = 1f;
    public int maxLionfish = 40;
    public int minLionfish = 3;

    private int currentLionfishCount = 0;

    void Awake()
    {
        spawnArea = GetComponent<BoxCollider2D>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnLionfish", 2f, spawnInterval); // checks every 1 second if we need to spawn a lionfish
        currentLionfishCount = GameObject.FindGameObjectsWithTag("Lionfish").Length;
    }

    void SpawnLionfish()
    {
        // lionfish breeding is handled by the lionfish themselves
        // this just ensures that there are at least a couple of lionfish in the game
        // Debug.Log("Current Lionfish Count: " + currentLionfishCount);

        if (currentLionfishCount < minLionfish)
        {
            Bounds bounds = spawnArea.bounds;
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);

            Vector2 spawnPosition = new Vector2(randomX, randomY);

            GameObject newFish = Instantiate(lionfishPrefab, spawnPosition, Quaternion.identity);

            IncreaseCount();

            newFish.GetComponent<Lionfish>().spawner = this; // set the spawner reference for the new lionfish
        }



    }

    public int getCurrentLionfishCount()
    {
        return currentLionfishCount;
    }

    public void DecreaseCount()
    {
        currentLionfishCount--;
        // Debug.Log("Decrease called\n" + System.Environment.StackTrace);
        gameManager.increaseKillCount();

        // updates the environmental meter based on current number of fish
        gameManager.updateEnvironmentCount(currentLionfishCount);
    }

    public void IncreaseCount()
    {
        currentLionfishCount++;

        // updates the environmental meter based on current number of fish
        gameManager.updateEnvironmentCount(currentLionfishCount);
    }

    public Bounds getBounds()
    {
        return spawnArea.bounds;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
