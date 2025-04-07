using Azure;
using Azure.AI.Language.Conversations;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.AI.Language.QuestionAnswering;
using Azure.AI.TextAnalytics;
using Azure.AI.Vision.ImageAnalysis;
using Azure.Storage.Blobs;
using AI_applications.Controllers;
using AI_applications.Services.Language;
using AI_applications.Services.Vision;
using AI_applications.Services.Speech;

namespace AI_applications.Services
{
    public static class MyConfigurations
    {
        public static IServiceCollection MyConfig(this IServiceCollection services)
        {
            string aiEndpoint = Environment.GetEnvironmentVariable("AIServiceEndpoint");
            string aiKey = Environment.GetEnvironmentVariable("AIServiceKey");
            string languageEndpoint = Environment.GetEnvironmentVariable("LanguageEndpoint");
            string languageKey = Environment.GetEnvironmentVariable("LanguageKey");
            string documentEndpoint = Environment.GetEnvironmentVariable("DocumentIntelligenceEndpoint");
            string documentKey = Environment.GetEnvironmentVariable("DocumentIntelligenceKey");
            string storageEndpoint = Environment.GetEnvironmentVariable("StorageEndpoint");

            var imageClient = new ImageAnalysisClient(new Uri(aiEndpoint), new AzureKeyCredential(aiKey));
            var textClient = new TextAnalyticsClient(new Uri(languageEndpoint), new AzureKeyCredential(languageKey));
            var documentClient = new DocumentAnalysisClient(new Uri(documentEndpoint), new AzureKeyCredential(documentKey));
            var qaClient = new QuestionAnsweringClient(new Uri(languageEndpoint), new AzureKeyCredential(languageKey));
            var conversationClient = new ConversationAnalysisClient(new Uri(languageEndpoint), new AzureKeyCredential(languageKey));
            var blobContainerClient = new BlobContainerClient(new Uri(storageEndpoint), new BlobClientOptions());

            services.AddSingleton(imageClient);
            services.AddSingleton(textClient);
            services.AddSingleton(documentClient);
            services.AddSingleton(qaClient);
            services.AddSingleton(conversationClient);
            services.AddSingleton(blobContainerClient);

            services.AddTransient<ConsoleController>();
            services.AddTransient<ImageAnalysis>();
            services.AddTransient<SpeechAudio>();
            services.AddTransient<SpeechTranslation>();
            services.AddTransient<TextAnalysis>();
            services.AddTransient<DocumentAnalysis>();
            services.AddTransient<QuestionAnswering>();
            services.AddTransient<Conversations>();

            return services;
        }
    }
}
