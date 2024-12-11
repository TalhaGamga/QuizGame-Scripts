using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LLMQuestionProvider", menuName = "Scriptable Objects/LLMQuestionProvider")]
public class LLMQuestionProvider : QuestionProviderBase
{
    private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
    private const string ApiKey = "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";

    List<Question> questionList;
    [SerializeField] int currentIndex = 0;

    public string topic;

    public override void Init(int questionCount)
    {
        questionList = new List<Question>();
        currentIndex = 0;

        questionList.Clear();

        if (topic == string.Empty || topic == null)
        {
            topic = "Culture";
        }

        for (int i = 0; i < questionCount; i++)
        {
            Question question = Provide();
            if (question != null)
            {
                questionList.Add(question);
            }
        }
    }

    public override Question ProvideQuestion()
    {
        if (currentIndex < questionList.Count)
        {
            if (questionList.Count > 0)
            {
                Question question = questionList[currentIndex];
                currentIndex++;
                return question;
            }
        }

        return null;
    }

    public override void Reset()
    {
        currentIndex = 0;
    }

    public override Question Provide()
    {
        Debug.Log("Provide method called.");

        try
        {
            // Call the asynchronous method synchronously
            Question question = Task.Run(() => FetchQuestionFromLLMAsync()).Result;
            Debug.Log($"Provide method returned: {question?.Text ?? "null"}");
            return question;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Provide method exception: {ex.Message}");
            return null;
        }
    }

    private async Task<Question> FetchQuestionFromLLMAsync()
    {
        using (var httpClient = new HttpClient())
        {
            try
            {
                // Prepare request payload
                var requestBody = new RequestPayload
                {
                    model = "gpt-4",
                    messages = new[]
                    {
                    new Message
                    {
                        role = "system",
                        content = "You are a quiz generator that generates questions in JSON format."
                    },
                    new Message
                    {
                        role = "user",
                        content = $"Generate a multiple-choice question about {topic} with four options and a correct answer."
                    }
                },
                    functions = new[]
                    {
                    new Function
                    {
                        name = "create_multiple_choice_question",
                        description = $"Generates a multiple-choice question about {topic} with four answer options and a correct answer.",
                        parameters = new Parameters
                        {
                            type = "object",
                            properties = new Property
                            {
                                Text = new PropertyDetail { type = "string", description = "The question text" },
                                A = new PropertyDetail { type = "string", description = "Option A" },
                                B = new PropertyDetail { type = "string", description = "Option B" },
                                C = new PropertyDetail { type = "string", description = "Option C" },
                                D = new PropertyDetail { type = "string", description = "Option D" },
                                CorrectAnswer = new PropertyDetail
                                {
                                    type = "string",
                                    @enum = new[] { "A", "B", "C", "D" },
                                    description = "The correct option"
                                },
                                Time = new PropertyDetail { type = "number", description = "Time allotted for the question in seconds" },
                                Point = new PropertyDetail { type = "integer", description = "Points awarded for the question" }
                            },
                            required = new[] { "Text", "A", "B", "C", "D", "CorrectAnswer", "Time", "Point" }
                        }
                    }
                },
                    max_tokens = 150,
                    temperature = 0.7f
                };

                string jsonBody = JsonUtility.ToJson(requestBody);
                Debug.Log($"Request Payload: {jsonBody}");

                // Create and send HTTP request
                var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
                {
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("Authorization", $"Bearer {ApiKey}");

                HttpResponseMessage response = await httpClient.SendAsync(request);

                // Check for HTTP errors
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    Debug.LogError($"API Error: {response.StatusCode} - {errorResponse}");
                    return null;
                }

                // Parse API response
                string responseContent = await response.Content.ReadAsStringAsync();
                Debug.Log($"API Response: {responseContent}");

                var responseObject = JsonUtility.FromJson<LLMResponse>(responseContent);

                // Extract arguments (the question data) from the response
                if (responseObject?.choices?[0]?.message?.function_call?.arguments != null)
                {
                    string arguments = responseObject.choices[0].message.function_call.arguments;
                    Debug.Log($"Extracted Arguments: {arguments}");

                    // Deserialize the arguments into a Question object
                    Question question = JsonUtility.FromJson<Question>(arguments);
                    Debug.Log($"Parsed Question: {question?.Text}");
                    return question;
                }

                Debug.LogError("Invalid API response: No arguments found.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception in FetchQuestionFromLLMAsync: {ex.Message}");
                return null;
            }
        }
    }



    [Serializable]
    private class RequestPayload
    {
        public string model;           // Specifies the GPT model, e.g., "gpt-4".
        public Message[] messages;     // Array of messages for the conversation.
        public Function[] functions;   // Array of function definitions.
        public int max_tokens;         // Maximum tokens for the response.
        public float temperature;      // Controls randomness in the model's output.
    }

    [Serializable]
    private class Message
    {
        public string role;            // Role of the sender: "system", "user", or "assistant".
        public string content;         // Content of the message.
    }

    [Serializable]
    private class Function
    {
        public string name;            // Name of the function, e.g., "create_multiple_choice_question".
        public string description;     // Description of the function.
        public Parameters parameters;  // Parameters schema for the function.
    }

    [Serializable]
    private class Parameters
    {
        public string type;            // Type of the parameters, usually "object".
        public Property properties;    // Schema of the properties.
        public string[] required;      // List of required properties.
    }

    [Serializable]
    private class Property
    {
        public PropertyDetail Text;            // Schema for the "Text" property.
        public PropertyDetail A;               // Schema for the "A" property.
        public PropertyDetail B;               // Schema for the "B" property.
        public PropertyDetail C;               // Schema for the "C" property.
        public PropertyDetail D;               // Schema for the "D" property.
        public PropertyDetail CorrectAnswer;   // Schema for the "CorrectAnswer" property.
        public PropertyDetail Time;            // Schema for the "Time" property.
        public PropertyDetail Point;           // Schema for the "Point" property.
    }

    [Serializable]
    private class PropertyDetail
    {
        public string type;                    // Data type, e.g., "string", "number".
        public string description;             // Description of the property.
        public string[] @enum;                 // Allowed values for enum types (e.g., "A", "B", "C", "D").
    }

    [Serializable]
    private class LLMResponse
    {
        public Choice[] choices;               // Array of choices returned by the model.
    }

    [Serializable]
    private class Choice
    {
        public MessageContent message;         // Message content for the choice.
    }

    [Serializable]
    private class MessageContent
    {
        public string content;                 // Content of the assistant's response (nullable).
        public FunctionCall function_call;     // Details of the function call, including arguments.
    }

    [Serializable]
    private class FunctionCall
    {
        public string name;                    // Name of the function called, e.g., "create_multiple_choice_question".
        public string arguments;               // JSON string of the function arguments (contains the question data).
    }

}