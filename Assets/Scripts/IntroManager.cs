using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Manages the introductory sequence for Lionfish Hunt.
/// Flow: Typewriter text -> Wait for Input -> Next Screen -> Title Scene.
/// </summary>
public class IntroManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI introTextDisplay;
    [SerializeField] private GameObject nextIndicator; 

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.03f;
    
    // This value is the default, but remember to check the Inspector!
    // It must match the name in your Build Settings exactly.
    [SerializeField] private string nextSceneName = "TitleScene"; 

    private string[] introScreens = new string[]
    {
        "The Atlantic reefs are under attack. \n\nAn invasive species—the Lionfish—has arrived from the Indo-Pacific, with no natural predators to stop them.",
        "Equipped with venomous spines and an insatiable appetite, a single lionfish can reduce native fish populations by 79% in just five weeks.",
        "As a volunteer diver, your mission is critical. Cull the invasive population and protect the biodiversity of our oceans.\n\n<b>The reef is counting on you.</b>"
    };

    private int _currentIndex = 0;
    private bool _isTyping = false;
    private bool _isTransitioning = false;
    private string _currentFullText = "";

    private void Start()
    {
        if (introTextDisplay == null)
        {
            Debug.LogError("IntroManager: Text Display is missing! Assign it in the Inspector.");
            return;
        }
        
        if (nextIndicator != null) nextIndicator.SetActive(false);
        
        DisplayCurrentScreen();
    }

    /// <summary>
    /// Triggered by Space or Click via Player Input component.
    /// </summary>
    public void OnContinue(InputAction.CallbackContext context)
    {
        // Guard: Only trigger on Performed phase to avoid multiple signals per click
        // Also guard against clicking while the scene is already changing
        if (!context.performed || _isTransitioning) return;

        if (_isTyping)
        {
            // Skip the typewriter effect and show full text immediately
            StopAllCoroutines();
            introTextDisplay.text = _currentFullText;
            _isTyping = false;
            if (nextIndicator != null) nextIndicator.SetActive(true);
        }
        else
        {
            _currentIndex++;
            
            // Check if we have more screens to show
            if (_currentIndex < introScreens.Length)
            {
                DisplayCurrentScreen();
            }
            else
            {
                // No more screens, proceed to Title Scene
                FinishIntro();
            }
        }
    }

    private void DisplayCurrentScreen()
    {
        _currentFullText = introScreens[_currentIndex];
        if (nextIndicator != null) nextIndicator.SetActive(false);
        StartCoroutine(TypeText(_currentFullText));
    }

    private IEnumerator TypeText(string text)
    {
        _isTyping = true;
        introTextDisplay.text = "";
        
        foreach (char c in text.ToCharArray())
        {
            introTextDisplay.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        _isTyping = false;
        if (nextIndicator != null) nextIndicator.SetActive(true);
    }

    private void FinishIntro()
    {
        _isTransitioning = true;
        Debug.Log($"Intro Complete. Transitioning to: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }
}