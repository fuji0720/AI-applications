using Azure;
using Azure.AI.TextAnalytics;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AI_applications.Models;

namespace AI_applications.Services.Language
{
    public class TextAnalysis
    {
        private readonly TextAnalyticsClient _textClient;
        private readonly BlobContainerClient _blobContainerClient;
        List<TextDocumentInput> inputs;
        List<TextResult> results;

        public TextAnalysis(TextAnalyticsClient textClient, BlobContainerClient blobContainerClient)
        {
            _textClient = textClient;
            _blobContainerClient = blobContainerClient;
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

                    await PrepareInput();
                    List<TextResult> list = await Analyze();
                    await UserOutput(list);

                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task PrepareInput()
        {
            AsyncPageable<BlobItem> items = _blobContainerClient.GetBlobsAsync();

            inputs = new List<TextDocumentInput>();
            results = new List<TextResult>();
            int i = 1;

            await foreach (BlobItem item in items)
            {
                string id = i.ToString();
                i++;

                BlobClient blobClient = _blobContainerClient.GetBlobClient(item.Name);
                Stream stream = await blobClient.OpenReadAsync();
                StreamReader reader = new StreamReader(stream);
                string text = await reader.ReadToEndAsync();

                TextDocumentInput input = new TextDocumentInput(id, text);
                inputs.Add(input);

                TextResult result = new TextResult();
                result.Id = id;
                result.Text = text;
                results.Add(result);
            }
        }

        public async Task<List<TextResult>> Analyze()
        {
            AnalyzeSentimentResultCollection sentiments = await _textClient.AnalyzeSentimentBatchAsync(inputs);
            ExtractKeyPhrasesResultCollection keyPhrases = await _textClient.ExtractKeyPhrasesBatchAsync(inputs);
            RecognizeEntitiesResultCollection categorizedEntities = await _textClient.RecognizeEntitiesBatchAsync(inputs);
            RecognizeLinkedEntitiesResultCollection linkedEntities = await _textClient.RecognizeLinkedEntitiesBatchAsync(inputs);

            foreach (AnalyzeSentimentResult item in sentiments)
            {
                string id = item.Id;
                results.FirstOrDefault(result => result.Id == id).Sentiment = item.DocumentSentiment;
            }

            foreach (ExtractKeyPhrasesResult item in keyPhrases)
            {
                string id = item.Id;
                results.FirstOrDefault(result => result.Id == id).KeyPhrases = item.KeyPhrases;
            }

            foreach (RecognizeEntitiesResult item in categorizedEntities)
            {
                string id = item.Id;
                results.FirstOrDefault(result => result.Id == id).Entities = item.Entities;
            }

            foreach (RecognizeLinkedEntitiesResult item in linkedEntities)
            {
                string id = item.Id;
                results.FirstOrDefault(result => result.Id == id).LinkedEntities = item.Entities;
            }

            return results;
        }

        public async Task UserOutput(List<TextResult> list)
        {
            foreach (TextResult text in list)
            {
                Console.WriteLine("\n--------------------");
                Console.WriteLine($"Id: {text.Id}");
                Console.WriteLine($"Text: {text.Text}");

                Console.WriteLine($"\nSentiment: {text.Sentiment.Sentiment}");

                Console.WriteLine($"\nKeyPhrases:");
                foreach (string item in text.KeyPhrases)
                {
                    Console.Write($"  {item}");
                }

                Console.WriteLine($"\nEntities:");
                foreach (CategorizedEntity item in text.Entities)
                {
                    Console.Write($"  {item.Text}");
                }

                Console.WriteLine($"\nLinkedEntities:");
                foreach (LinkedEntity item in text.LinkedEntities)
                {
                    Console.WriteLine($"  {item.Url}");
                }

                Console.WriteLine("--------------------\n");
            }
        }
    }
}

