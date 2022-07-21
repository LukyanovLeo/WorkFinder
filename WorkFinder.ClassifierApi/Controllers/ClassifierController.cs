using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TextsClassifierAPI.Helpers;
using TextsClassifierAPI.Models;
using TextsClassifierAPI.Models.Requests;
using TextsClassifierAPI.Models.Responses;

namespace TextsClassifierAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class ClassifierController : ControllerBase
    {
        private TextsClassifierOptions ApiOptions { get; }

        public ClassifierController(IOptions<TextsClassifierOptions> config)
        {
            ApiOptions = config.Value;
        }

        [HttpPost("Classify")]
        public ClassificationResponse Classify([FromBody] ClassificationRequest request)
        {
            var nn = new NeuralNetworkHelper(ApiOptions);
            var result = nn.ClassifyText(request.Text);
            if (result.Score < 0.85)
                result.CategoryId = null;
            return result;
        }

        [HttpPost("Train")]
        public string Train()
        {
            var nn = new NeuralNetworkHelper(ApiOptions);
            var pipeline = nn.PrepareData();
            nn.BuildAndTrainModel(pipeline);
            nn.Evaluate();
            nn.SaveModelAsFile();
            return "success";
        }
    }
}
