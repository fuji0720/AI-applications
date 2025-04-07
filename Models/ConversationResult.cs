namespace AI_applications.Models
{
    public class ConversationResult
    {
        public string topIntent { get; set; }
        public Entities[] entities { get; set; }
        public Intents[] intents { get; set; }
    }

    public class Entities
    {
        public string category { get; set; }
        public float confidenceScore { get; set; }
        public string text { get; set; }
    }

    public class Intents
    {
        public string category { get; set; }
        public float confidenceScore { get; set; }
    }
}
