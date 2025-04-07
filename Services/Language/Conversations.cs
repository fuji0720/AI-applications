using Azure;
using Azure.AI.Language.Conversations;
using Azure.Core;
using Azure.Core.Serialization;
using AI_applications.Models;

namespace AI_applications.Services.Language
{
    public class Conversations
    {
        private readonly ConversationAnalysisClient _client;
        private readonly IConfiguration _configuration;
        private readonly string projectName;
        private readonly string deploymentName;

        public Conversations(ConversationAnalysisClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            projectName = _configuration["Language:ClockProjectName"];
            deploymentName = _configuration["Language:ClockDeploymentName"];
        }

        public async Task UserInput()
        {
            try
            {
                do
                {
                    Console.WriteLine("\nTry asking me for the time... or 'q' to exit");
                    string userText = Console.ReadLine();

                    if (userText.ToLower() == "q")
                        break;

                    ConversationResult result = await GetIntent(userText);

                    ExecuteIntent(result);

                    Console.WriteLine($"Top intent: {result.topIntent}\n");
                    Console.WriteLine("--------------------\n");

                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<ConversationResult> GetIntent(string userText)
        {
            var data = new
            {
                analysisInput = new
                {
                    conversationItem = new
                    {
                        id = "1",
                        participantId = "1",
                        text = userText,
                    }
                },
                parameters = new
                {
                    projectName,
                    deploymentName,
                    // Use Utf16CodeUnit for strings in .NET.
                    stringIndexType = "Utf16CodeUnit",
                },
                kind = "Conversation",
            };

            RequestContent requestContent = RequestContent.Create(data);
            Response response = await _client.AnalyzeConversationAsync(requestContent);

            dynamic responseContent = response.Content.ToDynamicFromJson(JsonPropertyNames.CamelCase);
            dynamic prediction = responseContent.Result.Prediction;

            ConversationResult result = new ConversationResult
            {
                topIntent = prediction.TopIntent,
                entities = prediction.Entities,
                intents = prediction.Intents,
            };

            return result;
        }

        public void ExecuteIntent(ConversationResult result)
        {
            string topIntent = "";
            if (result.intents[0].confidenceScore > 0.5)
                topIntent = result.topIntent;

            switch (topIntent)
            {
                case "GetTime":
                    GetTime();
                    break;
                default:
                    break;
            }

            static void GetTime()
            {
                DateTime now = DateTime.Now;
                string time = now.ToString("t");
                Console.WriteLine(time);
            }
        }
    }
}
