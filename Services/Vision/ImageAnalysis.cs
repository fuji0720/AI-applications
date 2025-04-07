using Azure.AI.Vision.ImageAnalysis;

namespace AI_applications.Services.Vision
{
    public class ImageAnalysis
    {
        private readonly ImageAnalysisClient _client;

        public ImageAnalysis(ImageAnalysisClient client)
        {
            _client = client;
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

                    Console.WriteLine("\nSending the sample to Azure AI Services endpoint...");

                    ImageAnalysisResult result = await AnalyzeImage();
                    await UserOutput(result);

                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<ImageAnalysisResult> AnalyzeImage()
        {
            VisualFeatures visualFeatures = VisualFeatures.Caption |
                VisualFeatures.DenseCaptions |
                VisualFeatures.Tags;

            string imageFile = "Samples/images/street.jpg";

            using FileStream stream = new FileStream(imageFile, FileMode.Open);
            BinaryData data = BinaryData.FromStream(stream);
            stream.Close();

            ImageAnalysisResult result = await _client.AnalyzeAsync(data, visualFeatures);

            return result;
        }

        static async Task UserOutput(ImageAnalysisResult result)
        {
            if (result.Caption.Text != null)
            {
                Console.WriteLine("\nCaption:");
                Console.WriteLine($"  '{result.Caption.Text}', Confidence {result.Caption.Confidence:F2}");
            }

            if (result.DenseCaptions.Values.Count > 0)
            {
                Console.WriteLine("\nDense Captions:");
                foreach (DenseCaption denseCaption in result.DenseCaptions.Values)
                {
                    Console.WriteLine($"  Caption: '{denseCaption.Text}', Confidence: {denseCaption.Confidence:F2}");
                }
            }

            if (result.Tags.Values.Count > 0)
            {
                Console.WriteLine($"\nTags:");
                foreach (DetectedTag tag in result.Tags.Values)
                {
                    Console.WriteLine($"  '{tag.Name}', Confidence: {tag.Confidence:F2}");
                }
            }
        }
    }
}
