using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;

public class GreenPTChat : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField playerInput;
    public Transform chatContent;
    public GameObject messagePrefab; // A simple UI Text prefab

    [Header("API Config")]
    private string apiKey = ""; // Leave empty per instructions
    private string endpoint = "[https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-preview-09-2025:generateContent](https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-preview-09-2025:generateContent)";

    // Call this from your "Ask" button
    public void SendChatMessage()
    {
        if (string.IsNullOrEmpty(playerInput.text)) return;

        string userText = playerInput.text;
        CreateBubble("Player: " + userText, Color.black);
        playerInput.text = "";

        // OFFLINE MODE: Toggle between these for testing
        StartCoroutine(MockResponse()); 
        // StartCoroutine(PostToGreenPT(userText)); 
    }

    IEnumerator PostToGreenPT(string prompt)
    {
        // System context to keep the AI focused on Lionfish
        string systemPrompt = "You are an environmental expert helping players understand why lionfish are invasive in Florida.";
        
        // Construct the JSON (Simplified for Gemini API)
        string json = "{\"contents\": [{\"parts\":[{\"text\":\"" + prompt + "\"}]}], \"systemInstruction\": {\"parts\": [{\"text\":\"" + systemPrompt + "\"}]}}";
        
        UnityWebRequest request = new UnityWebRequest($"{endpoint}?key={apiKey}", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Note: You'll need a JSON parser (like SimpleJSON) for the full response 
            // but for now we'll just log the raw text
            CreateBubble("GreenPT: " + request.downloadHandler.text, Color.green);
        }
        else
        {
            CreateBubble("GreenPT: (Offline) Lionfish are invasive because they have no natural predators in the Atlantic!", Color.cyan);
        }
    }

    // Use this while on the plane to test UI layout!
    IEnumerator MockResponse()
    {
        yield return new WaitForSeconds(1);
        CreateBubble("GreenPT: That's a great question about lionfish!", Color.green);
    }

    void CreateBubble(string msg, Color color)
    {
        GameObject go = Instantiate(messagePrefab, chatContent);
        TMP_Text text = go.GetComponentInChildren<TMP_Text>();
        text.text = msg;
        text.color = color;
    }
}