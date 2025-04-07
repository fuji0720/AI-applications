using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace AI_applications.Services.Language
{
    public class DocumentAnalysis
    {
        private readonly DocumentAnalysisClient _client;
        private readonly IConfiguration _configuration;
        private readonly string modelId;
        private readonly string sample;

        public DocumentAnalysis(DocumentAnalysisClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            modelId = _configuration["DocumentIntelligence:ModelId"];
            sample = Environment.GetEnvironmentVariable("invoice-1");
        }

        public async Task UserInput()
        {
            try
            {
                do
                {
                    Console.WriteLine("\nPress any key... or 'q' to exit");
                    string userText = Console.ReadLine();

                    if (userText.ToLower() == "q")
                        break;

                    Console.WriteLine("\nSending the sample to Azure AI Services endpoint...\n\n");

                    AnalyzedDocument document = await Analyze();
                    await UserOutput(document);

                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<AnalyzedDocument> Analyze()
        {
            Uri testFile = new Uri(sample);

            AnalyzeDocumentOperation operation = await _client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, modelId, testFile);
            IReadOnlyList<AnalyzedDocument> documents = operation.Value.Documents;
            AnalyzedDocument document = documents[0];

            return document;
        }

        public async Task UserOutput(AnalyzedDocument document)
        {
            Console.WriteLine("\n--------------------");
            Console.WriteLine($"Type: {document.DocumentType}");
            Console.WriteLine($"Confidence: {document.Confidence:F2}\n\n");

            foreach (KeyValuePair<string, DocumentField> field in document.Fields)
            {
                Console.WriteLine($"Key: {field.Key}");
                Console.WriteLine($"ExpectedType: {field.Value.ExpectedFieldType}");
                Console.WriteLine($"Confidence: {field.Value.Confidence:F2}");
                Console.WriteLine($"Content: {field.Value.Content}\n");
            }
            Console.WriteLine("--------------------");
        }
    }
}

