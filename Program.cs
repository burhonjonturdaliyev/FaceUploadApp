using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string username = "admin";
        string password = "abc@1234";
        string faceUrl = "http://192.168.1.100/ISAPI/Intelligent/FDLib/FDModify?format=json";

        var faceData = new
        {
            faceLibType = "blackFD",
            FDID = "1",
            FPID = "44"
        };

        string faceDataJson = System.Text.Json.JsonSerializer.Serialize(faceData);
        string imagePath = "bb.jpg";

        using (HttpClientHandler handler = new HttpClientHandler { Credentials = new NetworkCredential(username, password) })
        using (HttpClient client = new HttpClient(handler))
        {
            using (var content = new MultipartFormDataContent())
            {
                // Add JSON part
                var jsonContent = new StringContent(faceDataJson, Encoding.UTF8, "application/json");
                content.Add(jsonContent, "FaceDataRecord");

                // Add image part
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                content.Add(imageContent, "FaceImage", Path.GetFileName(imagePath));

                try
                {
                    // Send the PUT request
                    HttpResponseMessage response = await client.PutAsync(faceUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response:");
                        Console.WriteLine(responseContent);
                    }
                    else
                    {
                        Console.WriteLine($"Request failed. Status Code: {response.StatusCode}");
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Error Details:");
                        Console.WriteLine(errorContent);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred:");
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
