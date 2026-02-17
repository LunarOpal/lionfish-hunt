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
    
    // TODO: Check hackathon docs for the specific URL. It is likely this one:
    private string apiUrl = "https://api.greenpt.ai/v1/chat/completions"; 

    public void SendChatMessage()
    {
        if (string.IsNullOrEmpty(playerInput.text)) return;

        string userText = playerInput.text;
        CreateBubble("Player: " + userText, new Color32(0, 68, 136, 255), Color.white, TextAlignmentOptions.MidlineRight);
        playerInput.text = "";

        // Switch to the real API call now
        StartCoroutine(PostToGreenPT(userText));
    }

    IEnumerator PostToGreenPT(string prompt)
    {
        // 1. Setup the data object
        RequestBody req = new RequestBody();
        req.model = "green-l-raw"; // Check docs for model name (e.g., "mistral-small" or "greenpt-flash")
        req.messages = new List<Message>();

        // System prompt (The "Context")
        req.messages.Add(new Message { role = "system", content = "You are an environmental expert regarding lionfish." });
        
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
    void CreateBubble(string msg, Color bubbleColor, Color textColor, TextAlignmentOptions alignment)
    {
        // 1. Create the bubble object
        GameObject go = Instantiate(messagePrefab, chatContent);

        // 2. Setup the Text (The words)
        TMP_Text text = go.GetComponentInChildren<TMP_Text>();
        text.text = msg;
        text.color = textColor;
        text.alignment = alignment; // Aligns text Left or Right

        // 3. Setup the Bubble (The background color)
        // We look for the "Image" component which controls the sprite color
        Image bubbleImage = go.GetComponentInChildren<Image>();
        if (bubbleImage != null)
        {
            bubbleImage.color = bubbleColor;
        }
        
        // 4. Force Unity to redraw the layout so it fits perfectly
        LayoutRebuilder.ForceRebuildLayoutImmediate(go.GetComponent<RectTransform>());
    }
}