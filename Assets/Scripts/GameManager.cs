using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float gameTime = 60f; // total game time in seconds
    public int fishEnvironmentMultiplier = 10;

    public TextMeshProUGUI timerText; // reference to the UI text element for displaying the timer
    public TextMeshProUGUI killText; // reference to the UI text element for displaying the lionfish kills
    public TextMeshProUGUI environmentText; // environmental meter

    private int killCount = 0; // number of lionfish killed
    private int environmentCount = 40;

    // Update is called once per frame
    void Update()
    {
        // decrement timer
        if (gameTime > 0)
        {
            gameTime -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.Ceil(gameTime).ToString();
        }

        // kill count updating is caused by lionfish spawner
        // update kill count text
        killText.text = "Hunted: " + killCount.ToString();

        // environmental meter updating is also caused by lionfish spawner
        // update environment text
        environmentText.text = "Environment: " + environmentCount.ToString();
    }

    public void increaseKillCount()
    {
        killCount++;
    }

    public void updateEnvironmentCount(int currentFishNum)
    {
        if (currentFishNum * fishEnvironmentMultiplier > 100)
        {
            environmentCount = 0;
        }
        else
        {
            environmentCount = 100 - (currentFishNum * fishEnvironmentMultiplier);
        }

    }
}
