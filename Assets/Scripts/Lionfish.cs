using System.Runtime.CompilerServices;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;

public class Lionfish : MonoBehaviour
{
    public float breedInterval = 5f;
    public float speed = 2f;
    public int points = 1;
    public float breedProb = 0.3f;
    private Transform player;
    private float timer = 0f;
    private int swimDirection = 0;
    private float swimDuration = 1f;
    private float swimTimer = 0f;

    public LionfishSpawner spawner; // has the total count of lionfish

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        timer = Random.Range(0f, breedInterval - 1); // randomize initial breeding interval
    }

    void Update()
    {
        // timer to breed new lionfish
        timer += Time.deltaTime;

        // timer for random walk
        swimTimer += Time.deltaTime;

        // at the breed interval, stop and attempt to duplicate
        if (timer >= breedInterval)
        {
            Breed();
            timer = 0f;
            swimDirection = 0;
            swimDuration = 1f; // pause for a moment before swimming again
            swimTimer = 0f;
        } else
        {
            Bounds bounds = spawner.getBounds();
            if (bounds != null)
            {

            // set new direction and duration for swimming
                if (swimTimer >= swimDuration)
                {

                    if (transform.position.x < bounds.min.x)
                    {
                        swimDirection = 1; // swim right if too far left
                    }
                    else if (transform.position.x > bounds.max.x)
                    {
                        transform.position = new Vector2(bounds.max.x, transform.position.y);
                        swimDirection = -1; // swim left if too far right
                    }
                    
                    // otherwise randomize direction
                    else if (Random.Range(0, 2) == 1)
                    {
                        swimDirection = 1; // swim right
                    }
                    else
                    {
                        swimDirection = -1; // swim left
                    }

                    swimDuration = Random.Range(1f, breedInterval); // also randomize swim duration
                    swimTimer = 0f;
                }

                if (transform.position.x + swimDirection < bounds.min.x |
                    transform.position.x + swimDirection > bounds.max.x)
                {
                    // if continuing to swim causes out of bounds, stop and reset timers to pick a new direction next frame
                    swimDirection = 0;
                    swimTimer = swimDuration; // forces a new direction next frame
                }
                else
                {
                    // else continue swimming in the current direction
                    transform.Translate(new Vector2(swimDirection, 0) * speed * Time.deltaTime);
                }
            }


        }

    }

    void Breed()
    {
        if (spawner != null && spawner.getCurrentLionfishCount() >= spawner.maxLionfish)
        {
            return; // to avoid breaking the game with exponential growth lol
        }
        else if (Random.value < breedProb)
        {
            Instantiate(gameObject, getRandomNearbyPosition(), Quaternion.identity);
            if (spawner != null)
            {
                spawner.IncreaseCount();
            }
        }
    }

    // need to breed off screen
    Vector2 getRandomNearbyPosition()
    {
        float offsetX = Random.Range(-1f, 1f);
        float offsetY = Random.Range(-1f, 1f);
        return new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);
    }

    public void Die()
    {
        if (spawner != null)
        {
            spawner.DecreaseCount();
        }
        Destroy(gameObject);
    }
}