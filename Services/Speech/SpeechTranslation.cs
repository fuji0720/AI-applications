using System.Text;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;

namespace AI_applications.Services.Speech
{
    public class SpeechTranslation
    {
        private readonly IConfiguration _configuration;
        private readonly string _region;
        private readonly string _key;
        private static SpeechConfig speechConfig;
        private static AudioConfig audioConfig;
        private static SpeechTranslationConfig translationConfig;

        public SpeechTranslation(IConfiguration configuration)
        {
            _configuration = configuration;
            _region = _configuration["Speech:Region"];
            _key = Environment.GetEnvironmentVariable("AIServiceKey");

            speechConfig = SpeechConfig.FromSubscription(_key, _region);
            audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            translationConfig = SpeechTranslationConfig.FromSubscription(_key, _region);
            translationConfig.SpeechRecognitionLanguage = "en-US";
            translationConfig.AddTargetLanguage("fr");
            translationConfig.AddTargetLanguage("es");
            translationConfig.AddTargetLanguage("hi");
        }

        public async Task UserInput()
        {
            try
            {
                do
                {
                    Console.InputEncoding = Encoding.Unicode;
                    Console.OutputEncoding = Encoding.Unicode;

                    Console.WriteLine("\nRecognition language: en-US");
                    Console.WriteLine("Select a target language: fr = French, es = Spanish, hi = Hindi\nor anything else to exit");

                    string targetLanguage = Console.ReadLine().ToLower();

                    if (translationConfig.TargetLanguages.Contains(targetLanguage))
                    {
                        string translation = await TranslateSpeech(targetLanguage);
                        await SynthesizeSpeech(targetLanguage, translation);
                    }
                    else
                    {
                        break;
                    }

                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<string> TranslateSpeech(string targetLanguage)
        {
            using TranslationRecognizer translator = new TranslationRecognizer(translationConfig, audioConfig);

            Console.WriteLine("\nSpeak now...");
            TranslationRecognitionResult result = await translator.RecognizeOnceAsync();

            if (result.Reason != ResultReason.TranslatedSpeech)
            {
                Console.WriteLine(result.Reason);
            }
            string translation = result.Translations[targetLanguage];

            Console.WriteLine($"Speak: '{result.Text}'");
            Console.WriteLine($"Translation: '{translation}'");

            return translation;
        }

        public async Task SynthesizeSpeech(string targetLanguage, string translation)
        {
            var voices = new Dictionary<string, string>
            {
                ["fr"] = _configuration["Speech:VoiceName2"],
                ["es"] = _configuration["Speech:VoiceName3"],
                ["hi"] = _configuration["Speech:VoiceName4"]
            };

            speechConfig.SpeechSynthesisVoiceName = voices[targetLanguage];
            using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig);

            SpeechSynthesisResult result = await speechSynthesizer.SpeakTextAsync(translation);
            if (result.Reason != ResultReason.SynthesizingAudioCompleted)
            {
                Console.WriteLine(result.Reason);
            }
        }
    }
}
