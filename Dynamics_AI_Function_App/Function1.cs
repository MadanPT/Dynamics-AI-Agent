using Azure.AI.OpenAI;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;


namespace Dynamics_AI_Function_App
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Dynmics-AI-Function")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            HttpResponseData ai_response = null;
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");

                var record_id = req.Query["record_id"];

                _logger.LogInformation($"record_id : {record_id}");

                var message = req.ReadAsStringAsync().Result;

                string response = $"{message}.";

                _logger.LogInformation($"input {message}");

                string endpoint2 = "https://madha-miqy8w3f-eastus2.openai.azure.com/";
                string key = "<key>";

                //AzureOpenAIClient client1 = new AzureOpenAIClient(new Uri(endpoint2), new System.ClientModel.ApiKeyCredential(key));

                //// string ai_response = "";
                //OpenAI _openAI = new OpenAI();
                //_openAI._logger = _logger;
                //ai_response = await _openAI.Ask_OpenAI__Persistent_Agent(endpoint2, key, message, req);
                //CRM_Tools._logger = _logger;
                //var rr = CRM_Tools.GetRetrieveRecordChangeHistory("account", "88cea450-cb0c-ea11-a813-000d3a1b1223");

                OpenAI._logger = _logger;
                ai_response = await OpenAI.Ask_OpenAI__Persistent_Agent(endpoint2, key, message, req);

                return ai_response;
                // return new OkObjectResult("");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return ai_response;
            //req.CreateResponse();

        }

        //public sealed record TextResponse(string Text);

        //[Function(nameof(RunOrchestrationAsync))]
        //public static async Task<object> RunOrchestrationAsync([OrchestrationTrigger] TaskOrchestrationContext context)
        //{
        //    // Get the prompt from the orchestration input
        //    string prompt = context.GetInput<string>() ?? throw new InvalidOperationException("Prompt is required");

        //    // Get both agents
        //    DurableAIAgent physicist = context.GetAgent("PhysicistAgent");
        //    DurableAIAgent chemist = context.GetAgent("ChemistAgent");

        //    // Start both agent runs concurrently
        //    Task<AgentRunResponse<TextResponse>> physicistTask = physicist.RunAsync<TextResponse>(prompt);

        //    Task<AgentRunResponse<TextResponse>> chemistTask = chemist.RunAsync<TextResponse>(prompt);

        //    // Wait for both tasks to complete using Task.WhenAll
        //    await Task.WhenAll(physicistTask, chemistTask);

        //    // Get the results
        //    TextResponse physicistResponse = (await physicistTask).Result;
        //    TextResponse chemistResponse = (await chemistTask).Result;

        //    // Return the result as a structured, anonymous type
        //    return new
        //    {
        //        physicist = physicistResponse.Text,
        //        chemist = chemistResponse.Text,
        //    };
        //}

        //// POST /multiagent/run
        //[Function(nameof(StartOrchestrationAsync))]
        //public static async Task<HttpResponseData> StartOrchestrationAsync(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "multiagent/run")] HttpRequestData req,
        //    [DurableClient] DurableTaskClient client)
        //{
        //    // Read the prompt from the request body
        //    string? prompt = await req.ReadAsStringAsync();
        //    if (string.IsNullOrWhiteSpace(prompt))
        //    {
        //        HttpResponseData badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
        //        await badRequestResponse.WriteAsJsonAsync(new { error = "Prompt is required" });
        //        return badRequestResponse;
        //    }

        //    string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
        //        orchestratorName: nameof(RunOrchestrationAsync),
        //        input: prompt);

        //    HttpResponseData response = req.CreateResponse(HttpStatusCode.Accepted);
        //    await response.WriteAsJsonAsync(new
        //    {
        //        message = "Multi-agent concurrent orchestration started.",
        //        prompt,
        //        instanceId,
        //        statusQueryGetUri = GetStatusQueryGetUri(req, instanceId),
        //    });
        //    return response;
        //}

        //// GET /multiagent/status/{instanceId}
        //[Function(nameof(GetOrchestrationStatusAsync))]
        //public static async Task<HttpResponseData> GetOrchestrationStatusAsync(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "multiagent/status/{instanceId}")] HttpRequestData req,
        //    string instanceId,
        //    [DurableClient] DurableTaskClient client)
        //{
        //    OrchestrationMetadata? status = await client.GetInstanceAsync(
        //        instanceId,
        //        getInputsAndOutputs: true,
        //        req.FunctionContext.CancellationToken);

        //    if (status is null)
        //    {
        //        HttpResponseData notFound = req.CreateResponse(HttpStatusCode.NotFound);
        //        await notFound.WriteAsJsonAsync(new { error = "Instance not found" });
        //        return notFound;
        //    }

        //    HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
        //    await response.WriteAsJsonAsync(new
        //    {
        //        instanceId = status.InstanceId,
        //        runtimeStatus = status.RuntimeStatus.ToString(),
        //        input = status.SerializedInput is not null ? (object)status.ReadInputAs<JsonElement>() : null,
        //        output = status.SerializedOutput is not null ? (object)status.ReadOutputAs<JsonElement>() : null,
        //        failureDetails = status.FailureDetails
        //    });
        //    return response;
        //}

        //private static string GetStatusQueryGetUri(HttpRequestData req, string instanceId)
        //{
        //    // NOTE: This can be made more robust by considering the value of
        //    //       request headers like "X-Forwarded-Host" and "X-Forwarded-Proto".
        //    string authority = $"{req.Url.Scheme}://{req.Url.Authority}";
        //    return $"{authority}/api/multiagent/status/{instanceId}";
        //}
    }
}
