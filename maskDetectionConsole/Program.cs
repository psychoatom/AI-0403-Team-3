using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace maskDetection
{
    public static class Program
    {
        static string predictionText = "";
        public static void Main()
        {
            Console.Write("Enter image file path: ");
            string imageFilePath = Console.ReadLine();

            int count = 1;
            DirectoryInfo d = new DirectoryInfo(@"*******");
            
            // the probability of predication of a class in string format
            string strProValue = "";
            // the probability of predication of a class in double format
            double proValue;
            // total probabilty of all the images;
            double totalProValue = 0;
            // average probability of tested images;
            double testingProValue;
            foreach (var file in d.GetFiles())
            {
                
                    MakePredictionRequest(file.FullName).Wait();
                    dynamic prediction = JsonConvert.DeserializeObject(predictionText);
                    foreach (var pred in prediction.predictions)
                    {
                        string tagname = pred.tagName;
                        //change the tagname to test different class
                        if (tagname == "NoMask")
                        {
                            count = count + 1;
                            //Console.WriteLine(prediction);
                            strProValue = pred.probability;
                            double.TryParse(strProValue, out proValue);

                            if (double.IsNaN(proValue) == false && double.IsInfinity(proValue) == false)
                            {
                               
                            totalProValue = proValue + totalProValue;
                            
                        }
                        }
                    }


            }

            testingProValue = totalProValue / count;

            Console.WriteLine("Total Number of Images:" + count);
            Console.WriteLine("Prediction for all the images: " + testingProValue);
            Console.WriteLine("\n\nHit ENTER to exit...");
            Console.ReadLine();
        }

        public static async Task<string> MakePredictionRequest(string imageFilePath)
        {
            var client = new HttpClient();
            
            // Request headers - replace this example key with your valid Prediction-Key.
            client.DefaultRequestHeaders.Add("Prediction-Key", "*****");

            // Prediction URL - replace this example URL with your valid Prediction URL.
            string url = "https://aiproject.cognitiveservices.azure.com/customvision/v3.0/Prediction/ecc60d7c-b3c8-4b65-9e60-1ae67ccb2ae4/classify/iterations/Iteration2/image";

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
                predictionText = await response.Content.ReadAsStringAsync();
            }
            return predictionText;
        }

        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }
}
