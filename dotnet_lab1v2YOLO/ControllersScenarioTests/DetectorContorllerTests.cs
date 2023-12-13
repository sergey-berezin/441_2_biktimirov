using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using YoloMVC.Models;

namespace ControllersScenarioTests
{
    public class DetectorContorllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _fixture;

        public DetectorContorllerTests(WebApplicationFactory<Program> fixture) => _fixture = fixture;
        

        [Fact]
        public async Task CorrectScenarioWithOneObject()
        {
            var client = _fixture.CreateClient();
            string base64Image = Convert.ToBase64String(File.ReadAllBytes("C:\\Users\\bikmish\\workspace\\cmc\\4 курс\\C# dotnet\\lab1\\dotnet_YOLO\\dotnet_lab1v2YOLO\\dataset\\cat.jpg"));
            var response = await client.PostAsJsonAsync("https://localhost:7074/DetectObjects", base64Image);
            var objectImages = JsonConvert.DeserializeObject<List<DetectedResult>>(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //checking the fact of existence
            Assert.Equal(1, objectImages.Count);

            Assert.Equal("cat", objectImages[0].Class);
            
            //checking for correct format
            Assert.Equal(0.90, Math.Round(objectImages[0].Confidence, 2));
        }

        [Fact]
        public async Task CorrectScenarioWithSeveralObject()
        {
            var client = _fixture.CreateClient();
            string base64Image = Convert.ToBase64String(File.ReadAllBytes("C:\\Users\\bikmish\\workspace\\cmc\\4 курс\\C# dotnet\\lab1\\dotnet_YOLO\\dotnet_lab1v2YOLO\\dataset\\catWithChair.jpg"));
            var response = await client.PostAsJsonAsync("https://localhost:7074/DetectObjects", base64Image);
            var objectImages = JsonConvert.DeserializeObject<List<DetectedResult>>(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(3, objectImages.Count);

            //checking for correct format and order
            Assert.True(Math.Round(objectImages.Last(x => x.Class == "cat").Confidence, 2) == 0.95);
            Assert.True(Math.Round(objectImages.Last(x => x.Class == "chair").Confidence, 2) == 0.73);
        }

        [Fact]
        public async Task ErrorScenarioInvalidBase64()
        {
            var client = _fixture.CreateClient();
            string base64Image = "eminem";
            var response = await client.PostAsJsonAsync("https://localhost:7074/DetectObjects", base64Image);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ErrorScenarioNullBase64()
        {
            var client = _fixture.CreateClient();
            string base64Image = null;
            var response = await client.PostAsJsonAsync("https://localhost:7074/DetectObjects", base64Image);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ErrorScenarioWrongUri()
        {
            var client = _fixture.CreateClient();
            string base64Image = Convert.ToBase64String(File.ReadAllBytes("C:\\Users\\bikmish\\workspace\\cmc\\4 курс\\C# dotnet\\lab1\\dotnet_YOLO\\dotnet_lab1v2YOLO\\dataset\\catWithChair.jpg"));
            var response = await client.PostAsJsonAsync("https://thebestobjectdetectionsiteever.net/detectus", base64Image);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}