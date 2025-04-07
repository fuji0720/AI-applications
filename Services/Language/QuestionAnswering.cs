using Azure.AI.Language.QuestionAnswering;

namespace AI_applications.Services.Language
{
    public class QuestionAnswering
    {
        private readonly QuestionAnsweringClient _client;
        private readonly IConfiguration _configuration;
        private readonly string projectName;
        private readonly string deploymentName;

        public QuestionAnswering(QuestionAnsweringClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            projectName = _configuration["Language:QAProjectName"];
            deploymentName = _configuration["Language:QADeploymentName"];
        }

        public async Task UserInput()
        {
            try
            {
                do
                {
                    Console.WriteLine("\nAsk some questions to me... or 'q' to exit");
                    string question = Console.ReadLine();

                    if (question.ToLower() == "q")
                        break;

                    IReadOnlyList<KnowledgeBaseAnswer> list = await Answering(question);
                    await UserOutput(list);

                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<IReadOnlyList<KnowledgeBaseAnswer>> Answering(string question)
        {
            QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);

            AnswersResult result = await _client.GetAnswersAsync(question, project);
            IReadOnlyList<KnowledgeBaseAnswer> list = result.Answers;

            return list;
        }

        public async Task UserOutput(IReadOnlyList<KnowledgeBaseAnswer> list)
        {
            foreach (KnowledgeBaseAnswer answer in list)
            {
                Console.WriteLine("\n--------------------");
                Console.WriteLine($"Answer: {answer.Answer}");
                Console.WriteLine($"Confidence: {answer.Confidence:F2}");
                Console.WriteLine($"Source: {answer.Source}");
                Console.WriteLine("--------------------\n");
            }
        }
    }
}

