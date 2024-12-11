using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LLMClient : MonoBehaviour
{
    [System.Serializable]
    public class RequestPayload
    {
        public string model;
        public string prompt;
    }

    [System.Serializable]
    public class LLMResponse
    {
        public string model;
        public string created_at;
        public string response;
        public bool done;
    }

    private string url = "http://127.0.0.1:9090/api/generate";

    void Start()
    {
        StartCoroutine(SendRequest());
    }

    IEnumerator SendRequest()
    {
        // Create the request payload
        RequestPayload requestData = new RequestPayload
        {
            model = "llama3.2",
            prompt = "Generate a question in JSON format with the following structure: { \"Text\": \"string\", \"A\": \"string\", \"B\": \"string\", \"C\": \"string\", \"D\": \"string\", \"CorrectAnswer\": \"A|B|C|D\" }"
        };

        // Convert the request data to JSON
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log("Request Data: " + jsonData);

        // Create the UnityWebRequest with JSON data
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Set the request headers
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("HTTP Error: " + request.responseCode + " - " + request.error);
        }
        else
        {
            string rawResponse = request.downloadHandler.text;
            Debug.Log("Raw Response: " + rawResponse);

            try
            {
                // Attempt to parse the response into the desired object
                LLMResponse response = JsonUtility.FromJson<LLMResponse>(rawResponse);
                Debug.Log($"Parsed Response: {response.response}");

                // Additional processing (if needed)
                if (response.done)
                {
                    Debug.Log("Full Response: " + response.response);
                }
                else
                {
                    Debug.LogWarning("Response incomplete, but done flag not set.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error parsing response: " + ex.Message);
            }
        }
    }
}
