using Microsoft.AspNetCore.Mvc;
using AI_applications.Services.Language;
using AI_applications.Services.Speech;
using AI_applications.Services.Vision;

namespace AI_applications.Controllers
{
    public class ConsoleController : Controller
    {
        private readonly ImageAnalysis _imageAnalysis;
        private readonly SpeechAudio _speechAudio;
        private readonly SpeechTranslation _speechTranslation;
        private readonly TextAnalysis _textAnalysis;
        private readonly DocumentAnalysis _documentAnalysis;
        private readonly QuestionAnswering _questionAnswering;
        private readonly Conversations _conversations;

        public ConsoleController(
            ImageAnalysis imageAnalysis,
            SpeechAudio speechAudio,
            SpeechTranslation speechTranslation,
            TextAnalysis textAnalysis,
            DocumentAnalysis documentAnalysis,
            QuestionAnswering questionAnswering,
            Conversations conversations)
        {
            _imageAnalysis = imageAnalysis;
            _speechAudio = speechAudio;
            _speechTranslation = speechTranslation;
            _textAnalysis = textAnalysis;
            _documentAnalysis = documentAnalysis;
            _questionAnswering = questionAnswering;
            _conversations = conversations;
        }

        public async Task Index()
        {
            do
            {
                Console.WriteLine("\nSelect a service...");
                Console.WriteLine("1 = Vision: ImageAnalysis");
                Console.WriteLine("2 = Speech: SpeechAudio");
                Console.WriteLine("3 = Speech: SpeechTranslation");
                Console.WriteLine("4 = Language: TextAnalytics");
                Console.WriteLine("5 = Language: FormRecognizer");
                Console.WriteLine("6 = Language: QuestionAnswering");
                Console.WriteLine("7 = Language: Conversations");
                Console.WriteLine("q = quit");
                string select = Console.ReadLine();
                
                if (select == "q")
                {
                    break;
                }

                switch (select)
                {
                    case "1":
                        await _imageAnalysis.UserInput();
                        break;
                    case "2":
                        await _speechAudio.UserInput();
                        break;
                    case "3":
                        await _speechTranslation.UserInput();
                        break;
                    case "4":
                        await _textAnalysis.UserInput();
                        break;
                    case "5":
                        await _documentAnalysis.UserInput();
                        break;
                    case "6":
                        await _questionAnswering.UserInput();
                        break;
                    case "7":
                        await _conversations.UserInput();
                        break;
                    default:
                        break;
                }

                Console.WriteLine("------------------------------------------");
            } while (true);
        }
    }
}
