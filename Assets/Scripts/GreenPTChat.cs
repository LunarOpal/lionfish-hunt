using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;

// 1. [Preserve] prevents WebGL's aggressive IL2CPP compiler from deleting these classes
[System.Serializable]
[UnityEngine.Scripting.Preserve]
public class Message
{
    public string role;
    public string content;
}

[System.Serializable]
[UnityEngine.Scripting.Preserve]
public class RequestBody
{
    public string model;
    public List<Message> messages;
}

[System.Serializable]
[UnityEngine.Scripting.Preserve]
public class ResponseRoot
{
    public List<Choice> choices;
}

[System.Serializable]
[UnityEngine.Scripting.Preserve]
public class Choice
{
    public Message message;
}

public class GreenPTChat : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField playerInput;
    [SerializeField] private Transform chatContent;
    [SerializeField] private GameObject messagePrefab;

    [Header("GreenPT Config")]
    [Tooltip("Check this if building for WebGL to bypass CORS issues.")]
    [SerializeField] private bool useCorsProxyForWebGL = true; 
    
    [Tooltip("Change this if the current proxy goes down during judging!")]
    [SerializeField] private string proxyUrl = "https://api.codetabs.com/v1/proxy?quest=";

    // Replaced Secrets.GreenPTApiKey with a placeholder. Ensure your Secrets class is accessible.
    private string apiKey = Secrets.GreenPTApiKey; 
    private string baseApiUrl = "https://api.greenpt.ai/v1/chat/completions"; 

    private GameObject currentLoadingBubble;

    public void SendChatMessage()
    {
        if (string.IsNullOrEmpty(playerInput.text)) return;

        string userText = playerInput.text;
        CreateBubble("Player: " + userText, new Color32(0, 68, 136, 255), Color.white, TextAlignmentOptions.MidlineRight);
        playerInput.text = "";

        currentLoadingBubble = CreateBubble("GreenPT: Thinking...", new Color32(32, 178, 170, 150), Color.black, TextAlignmentOptions.MidlineLeft);

        StartCoroutine(PostToGreenPT(userText));
    }

    private IEnumerator PostToGreenPT(string prompt)
    {
        RequestBody req = new RequestBody
        {
            model = "green-l-raw",
            messages = new List<Message>()
        };

        string systemPrompt = 
            "You are GreenPT, a marine biology AI assistant for a diver. Your goal: answer the user's query ideally with a fact related to the invasive lionfish. " +
            "CRITICAL RULES: " +
            "1. ABSOLUTELY NO MARKDOWN. You are strictly forbidden from using asterisks (*), hashtags (#), or any text formatting. Use pure plain text only. " +
            "2. STRICT LENGTH LIMIT: Your entire response MUST be under 250 characters. " +
            "3. Tone: Professional, urgent, and educational. Focus on the destructive impact of lionfish.";

        req.messages.Add(new Message { role = "system", content = systemPrompt });
        req.messages.Add(new Message { role = "user", content = prompt });

        string json = JsonUtility.ToJson(req);

        // 2. CORS Proxy routing for WebGL environments
        string requestUrl = baseApiUrl;
        if (useCorsProxyForWebGL && Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // FIX: Swapped to a variable proxy URL to allow hot-swapping in the editor.
            // Using codetabs by default as it is often more tolerant of POST requests with Auth headers.
            requestUrl = proxyUrl + baseApiUrl;
        }

        // Hackathon Debug Tip: Press F12 in your browser on itch.io. 
        // If this logs "{}" instead of your data, WebGL is stripping your JSON classes!
        Debug.Log("GreenPT Payload: " + json);

        using (UnityWebRequest request = new UnityWebRequest(requestUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            // Wait for the request to finish
            yield return request.SendWebRequest();

            if (currentLoadingBubble != null)
            {
                Destroy(currentLoadingBubble);
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                ResponseRoot response = JsonUtility.FromJson<ResponseRoot>(request.downloadHandler.text);
                if (response != null && response.choices != null && response.choices.Count > 0)
                {
                    string aiText = response.choices[0].message.content;
                    CreateBubble("GreenPT: " + aiText, new Color32(32, 178, 170, 255), Color.black, TextAlignmentOptions.MidlineLeft);
                }
                else
                {
                    CreateBubble("GreenPT: (No response data found)", Color.red, Color.white, TextAlignmentOptions.MidlineLeft);
                }
            }
            else
            {
                Debug.LogError("API Error: " + request.error + " | " + request.downloadHandler.text);
                CreateBubble("Connection Error: " + request.error, Color.red, Color.white, TextAlignmentOptions.MidlineLeft);
            }
        } // UnityWebRequest is automatically disposed here to prevent memory leaks
    }

    private GameObject CreateBubble(string msg, Color bubbleColor, Color textColor, TextAlignmentOptions alignment)
    {
        GameObject go = Instantiate(messagePrefab, chatContent);
        
        TMP_Text text = go.GetComponentInChildren<TMP_Text>();
        text.text = msg;
        text.color = textColor;
        text.alignment = alignment;

        Image bubbleImage = go.GetComponentInChildren<Image>();
        if (bubbleImage != null)
        {
            bubbleImage.color = bubbleColor;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(go.GetComponent<RectTransform>());
        
        return go;
    }

    public void OnInputSubmit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendChatMessage();
            playerInput.ActivateInputField(); 
        }
    }
}