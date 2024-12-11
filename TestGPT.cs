using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TestGPT : MonoBehaviour
{
    private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
    private const string ApiKey = "YOUR_API_KEY_HERE";

    public void AskChatGPT(string prompt)
    {
        StartCoroutine(SendRequest(prompt));
    }

    private IEnumerator SendRequest(string prompt)
    {
        string jsonBody = "{\"model\":\"gpt-3.5-turbo\",\"messages\":[{\"role\":\"user\",\"content\":\"" + prompt + "\"}]}";

        using (UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST"))
        {
            request.SetRequestHeader("Authorization", $"Bearer {ApiKey}");
            request.SetRequestHeader("Content-Type", "application/json");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Response: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
            }
        }
    }
}
