using Azure.AI.TextAnalytics;

namespace AI_applications.Models
{
    public class TextResult
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public DocumentSentiment Sentiment { get; set; }
        public KeyPhraseCollection KeyPhrases { get; set; }
        public CategorizedEntityCollection Entities { get; set; }
        public LinkedEntityCollection LinkedEntities { get; set; }
    }
}
