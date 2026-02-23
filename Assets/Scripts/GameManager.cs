using UnityEngine;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float maxTime = 60f; // total game time in seconds
    public float maxEnvironment = 100f;
    public float startingEnvironment = 50f;
    public int fishEnvironmentMultiplier = 10;

    public UnityEngine.UI.Image timerFillImage; // timer bar
    public UnityEngine.UI.Image environmentalFillImage; // timer bar
    public TextMeshProUGUI killText; // reference to the UI text element for displaying the lionfish kills

    public CoralHealth coralBackground; // coral background object to change color based on environment health

    private float gameTime;
    private float environmentHP;
    private int killCount = 0; // number of lionfish killed



    void Start()
    {
        gameTime = maxTime;
        environmentHP = startingEnvironment;
        environmentalFillImage.fillAmount = environmentHP/maxEnvironment;
    }

    // Update is called once per frame
    void Update()
    {
        // decrement timer
        if (gameTime > 0)
        {
            gameTime -= Time.deltaTime;

            gameTime = Mathf.Clamp(gameTime, 0, maxTime); // makes sure number doesn't go below min or above max
            // update timer (oxygen) bar visual
            timerFillImage.fillAmount = gameTime / maxTime;
        }

        // kill count updating is caused by lionfish spawner
        // update kill count text
        killText.text = killCount.ToString();

        // environmental hp bar updating is also caused by lionfish spawner
        // update visual
        environmentalFillImage.fillAmount = environmentHP/maxEnvironment;
    }

    public void increaseKillCount()
    {
        killCount++;
    }

    public void updateEnvironmentCount(int currentFishNum)
    {
        if (currentFishNum * fishEnvironmentMultiplier > maxEnvironment)
        {
            environmentHP = 0;
        }
        else
        {
            environmentHP = maxEnvironment - (currentFishNum * fishEnvironmentMultiplier);
        }

        // update coral background color based on environment health
        coralBackground.coralHealthCheck(maxEnvironment - (currentFishNum * fishEnvironmentMultiplier));

    }
}
