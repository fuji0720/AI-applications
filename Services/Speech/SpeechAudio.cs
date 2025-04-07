using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace AI_applications.Services.Speech
{
    public class SpeechAudio
    {
        private readonly IConfiguration _configuration;
        private readonly string _region;
        private readonly string _key;
        private static SpeechConfig speechConfig;
        private static AudioConfig audioConfig;

        public SpeechAudio(IConfiguration configuration)
        {
            _configuration = configuration;
            _region = _configuration["Speech:Region"];
            _key = Environment.GetEnvironmentVariable("AIServiceKey");

            speechConfig = SpeechConfig.FromSubscription(_key, _region);
            audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        }

        public async Task UserInput()
        {
            try
            {
                do
                {
                    string userText = "";
                    Console.WriteLine("\nPress any key... or 'q' to exit");
                    userText = Console.ReadLine();

                    if (userText.ToLower() == "q")
                        break;

                    string command = await RecognizeSpeech();
                    Console.WriteLine($"\nSpeech: {command}\n");

                    string responseText = "";
                    if (command.ToLower() == "what time is it?")
                    {
                        DateTime now = DateTime.Now;
                        responseText = "The time is " + now.Hour.ToString() + ":" + now.Minute.ToString("D2");
                    }
                    else
                    {
                        responseText = "Sorry, I can't respond to your request. Please request 'What time is it?'.";
                    }

                    Console.WriteLine(responseText);
                    await SynthesizeSpeech(responseText);

                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<string> RecognizeSpeech()
        {
            using SpeechRecognizer speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

            Console.WriteLine("Ready to use speech service.\nSpeak now...");
            SpeechRecognitionResult speech = await speechRecognizer.RecognizeOnceAsync();

            string command = "";
            if (speech.Reason == ResultReason.RecognizedSpeech)
            {
                command = speech.Text;
            }
            else
            {
                Console.WriteLine(speech.Reason);
            }

            return command;
        }

        public async Task SynthesizeSpeech(string responseText)
        {
            speechConfig.SpeechSynthesisVoiceName = _configuration["Speech:VoiceName1"];
            using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer(speechConfig);

            SpeechSynthesisResult result = await speechSynthesizer.SpeakTextAsync(responseText);
            if (result.Reason != ResultReason.SynthesizingAudioCompleted)
            {
                Console.WriteLine(result.Reason);
            }
        }
    }
}
