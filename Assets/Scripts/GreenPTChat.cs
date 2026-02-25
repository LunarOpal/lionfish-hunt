using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;

[System.Serializable]
public class Message
{
    public string role;
    public string content;
}

[System.Serializable]
public class RequestBody
{
    public string model;
    public List<Message> messages;
}

[System.Serializable]
public class ResponseRoot
{
    public List<Choice> choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

public class GreenPTChat : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField playerInput;
    public Transform chatContent;
    public GameObject messagePrefab;

    [Header("GreenPT Config")]

    private string apiKey = Secrets.GreenPTApiKey; 
    private string apiUrl = "https://api.greenpt.ai/v1/chat/completions"; 

    private GameObject currentLoadingBubble;

    public void SendChatMessage()
    {
        if (string.IsNullOrEmpty(playerInput.text)) return;

        string userText = playerInput.text;
        CreateBubble("Player: " + userText, new Color32(0, 68, 136, 255), Color.white, TextAlignmentOptions.MidlineRight);
        playerInput.text = "";

        currentLoadingBubble = CreateBubble("GreenPT: Thinking...", new Color32(32, 178, 170, 150), Color.black, TextAlignmentOptions.MidlineLeft);

        // Switch to the real API call now
        StartCoroutine(PostToGreenPT(userText));
    }

    IEnumerator PostToGreenPT(string prompt)
    {
        // 1. Setup the data object
        RequestBody req = new RequestBody();
        req.model = "green-l-raw";
        req.messages = new List<Message>();

        // System prompt
        string systemPrompt = 
            "You are GreenPT, a marine biology AI assistant for a diver. Your goal: answer the user's query ideally with a fact related to the invasive lionfish. " +
            "CRITICAL RULES: " +
            "1. ABSOLUTELY NO MARKDOWN. You are strictly forbidden from using asterisks (*), hashtags (#), or any text formatting. Use pure plain text only. " +
            "2. STRICT LENGTH LIMIT: Your entire response MUST be under 250 characters. " +
            "3. Tone: Professional, urgent, and educational. Focus on the destructive impact of lionfish.";

        req.messages.Add(new Message { role = "system", content = systemPrompt });
        
        // User prompt
        req.messages.Add(new Message { role = "user", content = prompt });

        // 2. Convert to JSON
        string json = JsonUtility.ToJson(req);

        // 3. Create Request
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        
        // 4. Set Headers (Important for GreenPT/OpenAI style)
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (currentLoadingBubble != null)
        {
            Destroy(currentLoadingBubble);
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse the response
            ResponseRoot response = JsonUtility.FromJson<ResponseRoot>(request.downloadHandler.text);
            if (response.choices != null && response.choices.Count > 0)
            {
                string aiText = response.choices[0].message.content;
                CreateBubble("GreenPT: " + aiText, new Color32(32, 178, 170, 255), Color.black, TextAlignmentOptions.MidlineLeft);
            }
        }
        else
        {
            Debug.LogError("Error: " + request.error + " | " + request.downloadHandler.text);
            CreateBubble("Error: " + request.error, Color.red, Color.white, TextAlignmentOptions.MidlineLeft);
        }
    }

    // The "Upgraded" Bubble Maker
    GameObject CreateBubble(string msg, Color bubbleColor, Color textColor, TextAlignmentOptions alignment)
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
        
        return go; // Return the created bubble
    }
    // This function will be triggered by the InputField
    public void OnInputSubmit(string text)
    {
        // 1. Check if the "Return" (Main Enter) or "KeypadEnter" was pressed
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // 2. Call your existing message sending logic
            SendChatMessage();
            
            // 3. Keep the cursor inside the text box so the player can type again immediately
            playerInput.ActivateInputField(); 
        }
    }
}