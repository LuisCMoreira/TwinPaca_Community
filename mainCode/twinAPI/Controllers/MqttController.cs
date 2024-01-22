using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


[ApiController]
[Route("api/mqtt")]
public class MqttController : ControllerBase
{
    private readonly MqttService mqttService;

    public MqttController(MqttService mqttService)
    {
        this.mqttService = mqttService;
    }

    [HttpGet("data/features")]
    public IActionResult GetTopic1Data()
    {
        string latestData = mqttService.GetLatestJsonData();
        
        if (!string.IsNullOrEmpty(latestData))
        {
            return Ok(latestData);
        }

        return NotFound("No data available for 'IoT/myDevice_V0/someId/features'");
    }



  [HttpGet("data/features/testVar1_Coil1")]
    public IActionResult GetTopic1TestVar1_Coil1()
    {
        string latestData = mqttService.GetLatestJsonData();

        if (!string.IsNullOrEmpty(latestData))
        {
            // Parse the JSON data and extract the specific property
            var json = JObject.Parse(latestData);
            var testVar1_Coil1 = json["testVar1_Coil1"];

            return Ok($"{testVar1_Coil1}");
        }

        return NotFound("No data available for 'IoT/myDevice_V0/someId/features'");
    }

    [HttpGet("data/features/testVar2_Register10")]
    public IActionResult GetTopic1TestVar2_Register10()
    {
        string latestData = mqttService.GetLatestJsonData();

        if (!string.IsNullOrEmpty(latestData))
        {
            // Parse the JSON data and extract the specific property
            var json = JObject.Parse(latestData);
            var testVar2_Register10 = json["testVar2_Register10"];

       

    

            return Ok($"{testVar2_Register10}");
        }

        return NotFound("No data available for 'IoT/myDevice_V0/someId/features'");
    }

}
