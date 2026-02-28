using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public float maxTime = 60f; // total game time in seconds
    public float maxEnvironment = 100f;
    public float startingEnvironment = 50f;
    public int fishEnvironmentMultiplier = 10;

    public UnityEngine.UI.Image timerFillImage; // timer bar
    public UnityEngine.UI.Image environmentalFillImage; // timer bar
    public TextMeshProUGUI killText; // reference to the UI text element for displaying the lionfish kills
    public Transform spotlight; // move this around during tutorial
    public TutorialStep[] steps;
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI nextIndicatorText;

    public CoralHealth coralBackground; // coral background object to change color based on environment health

    // all the results screen objects
    public GameObject resultsScreen;
    public GameObject NumberOfLionFishHunted;
    public TextMeshProUGUI NumberOfLionFishHuntedNum;
    public GameObject OceanHealth;
    public TextMeshProUGUI OceanHealthPercentage;
    public TextMeshProUGUI Blurb;
    public GameObject NextButton;
    public AudioClip resultsPopUp;
    public AudioClip uiPopUp;

    private float gameTime;
    private float environmentHP;
    private int killCount = 0; // number of lionfish killed
    
    private AudioSource audioSource;
    private bool tutorialPhase = true;
    private int currentStep = 0; //phase of tutorial, at phase 7 the gameplay starts
    private bool gameEnd = false;



    void Start()
    {
        gameTime = maxTime;
        environmentHP = startingEnvironment;
        environmentalFillImage.fillAmount = environmentHP/maxEnvironment;

        // results screen
        NumberOfLionFishHuntedNum.text = "";
        OceanHealthPercentage.text = "";
        Blurb.text = "";

        audioSource = GetComponent<AudioSource>();
        spotlight.gameObject.SetActive(true);
        tutorialText.gameObject.SetActive(true);
        nextIndicatorText.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentStep < steps.Length)
        {
            spotlight.position = steps[currentStep].target.position;
            tutorialText.text = steps[currentStep].message;
            if (Input.GetKeyDown(KeyCode.Space) | Input.GetMouseButtonDown(0))
            {
                currentStep++;
            }
        }
        else if (tutorialPhase)
        {
            tutorialPhase = false;
            spotlight.gameObject.SetActive(false);
            tutorialText.gameObject.SetActive(false);
            nextIndicatorText.gameObject.SetActive(false);
        }
        // decrement timer, start gameplay
        else if (gameTime > 0)
        {
            gameTime -= Time.deltaTime;

            gameTime = Mathf.Clamp(gameTime, 0, maxTime); // makes sure number doesn't go below min or above max
            // update timer (oxygen) bar visual
            timerFillImage.fillAmount = gameTime / maxTime;
            
            // kill count updating is caused by lionfish spawner
            // update kill count text
            killText.text = killCount.ToString();

            // environmental hp bar updating is also caused by lionfish spawner
            // update visual
            environmentalFillImage.fillAmount = environmentHP/maxEnvironment;
        
        
        } else if (gameEnd == false)
        {
            endGame();
        }


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

    public void endGame()
    {
        Time.timeScale = 0f;

        StartCoroutine(ShowSequence());
        gameEnd = true;

    }

    IEnumerator ShowSequence()
    {
        resultsScreen.SetActive(true);
        audioSource.PlayOneShot(resultsPopUp);
        
        yield return new WaitForSecondsRealtime(1f);

        NumberOfLionFishHuntedNum.text = killCount.ToString();
        NumberOfLionFishHunted.SetActive(true);
        audioSource.PlayOneShot(uiPopUp);

        yield return new WaitForSecondsRealtime(1f);

        if (environmentHP < 0) 
        {
            OceanHealthPercentage.text = "0%";
        } else
        {
            OceanHealthPercentage.text = ((environmentHP / maxEnvironment) * 100) .ToString() + "%";
        }
        OceanHealth.SetActive(true);
        audioSource.PlayOneShot(uiPopUp);
        
        yield return new WaitForSecondsRealtime(1f);

        if (environmentHP == 0)
        {
            Blurb.text = "Another beautiful reef taken by the invaders... try again next time!";
        } else if (environmentHP < 30)
        {
            Blurb.text = "The coral don't look so good... but keep on fighting the good fight!";
        } else if (environmentHP < 70)
        {
            Blurb.text = "The reefs are surviving but not quite thriving. You made a great effort!";
        } else
        {
            Blurb.text = "The ocean is colorful and healthy again. Awesome job!";
        }
        audioSource.PlayOneShot(uiPopUp);

        yield return new WaitForSecondsRealtime(1f);

        NextButton.SetActive(true);
        //audioSource.PlayOneShot(uiPopUp);
    }

    public bool getGameEnd()
    {
        return gameEnd;
    }

    // returns true/false on whether tutorial is active, affects movement of diver and lionfish
    public bool getTutorialPhase()
    {
        return tutorialPhase;
    }

    // returns phase of tutorial
    // only able to move in parts 1 and 2, talking about movement and attacking
    public int getCurrentStep()
    {
        return currentStep;
    }

}

