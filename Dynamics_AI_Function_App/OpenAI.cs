using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics_AI_Function_App
{
    public class OpenAI
    {
        public static ILogger<Function1> _logger { get; set; }
        private static AIAgent aIAgent { get; set; }
        private static AgentThread agentThread { get; set; }

        public static List<string> History = new();

        //public static async Task<HttpResponseData> Ask_OpenAI_Agent(string endPoint, string key, string input, HttpRequestData req)
        //{
        //    _logger.LogInformation("OpenAI_Agent started.");
        //    CRM_Tools._logger = _logger;

        //    //var input = req.ReadAsStringAsync().Result;
        //    string prompt = string.Join("\n", History) + "\nUser: " + input;

        //    string Ai_Agent_instructions = @$"You are a helpful Dynamics CRM assistant.

        //                After giving your answer, ALWAYS ask a follow-up question to keep the conversation going.
        //                The follow-up question must be relevant to the user’s last query.

        //                Do NOT show FetchXML or reveal any tool calls.

        //                Always respond in rich-text Markdown with:
        //                        - Emojis
        //                        - Headings
        //                        - Tables
        //                        - Bullet points
        //                        - Base64 PNG charts";


        //    AzureOpenAIClient client1 = new AzureOpenAIClient(new Uri(endPoint), new System.ClientModel.ApiKeyCredential(key));
        //    if (aIAgent == null)
        //    {
        //        _logger.LogInformation("ai AGnet is null.");


        //        aIAgent = client1.GetChatClient("gpt-5.1-chat")
        //                                        .CreateAIAgent(instructions: Ai_Agent_instructions,
        //                                        tools: [
        //                                            AIFunctionFactory.Create(CRM_Tools.GetDatafromDynamicsCRM,"get_data_from_dynamics_crm")
        //                                        ]);
        //    }

        //    if (aIAgent == null)
        //    {
        //        agentThread = aIAgent.GetNewThread();

        //    }


        //    var http_response = req.CreateResponse(HttpStatusCode.OK);

        //    Microsoft.Extensions.AI.ChatMessage chatMessage = new Microsoft.Extensions.AI.ChatMessage();
        //    //Microsoft.Extensions.AI.ChatMessage chatMessage = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, input);

        //    await foreach (var chunk in aIAgent.RunStreamingAsync(chatMessage, agentThread))
        //    {
        //        foreach (var eachContent in chunk.Contents)
        //        {
        //            if (eachContent.GetType() == typeof(TextContent))
        //            {
        //                var eachChunk_Content = ((TextContent)eachContent).Text;
        //                await http_response.WriteStringAsync(eachChunk_Content);
        //            }
        //        }
        //    }

        //    return http_response;
        //}

        public static async Task<HttpResponseData> Ask_OpenAI__Persistent_Agent(string endPoint, string key, string input, HttpRequestData req)
        {
            _logger.LogInformation("OpenAI_Agent started.");
            CRM_Tools._logger = _logger;

            var clear = req.Query["clear"];


            var record_id = req.Query["entity_id"];
            var entity_name = req.Query["entity_name"];

            if (!string.IsNullOrEmpty(record_id) && !string.IsNullOrEmpty(entity_name))
            {
                input = $"get record details using id= {record_id} and entity_schema_name ={entity_name}";
            }

            //var input = req.ReadAsStringAsync().Result;
            string prompt = string.Join("\n", History) + "\nUser: " + input;

            string Ai_Agent_instructions = """
                You are a Dynamics CRM AI assistant. You must only answer questions using the data returned by the available tools.
                - Tools: get_data_from_dynamics_crm, Create_activity_record_in_dynamics_CRM, Get_Audit_Changes_from_dynamics_crm
                - Do not provide any answer using prior knowledge of Dynamics or external sources.

                If the tool data does not fully answer the question, provide a helpful response by:
                - Summarizing what information is available
                - Suggesting next steps or queries the user can run
                - Highlighting what data is missing
                - Always use the tool output to generate your response.
                
                Do not show fetchXml query in response unless explicitly asks.

                Always flatten the CRM Entity data:
                    - Convert AliasedValue to direct value
                    - Convert EntityReference to name (Id if no name)
                    - Convert OptionSetValue to label (and integer)
                    - Convert Money to decimal
                    - Convert BooleanManagedProperty to boolean
                """;

            AzureOpenAIClient client1 = new AzureOpenAIClient(new Uri(endPoint), new System.ClientModel.ApiKeyCredential(key));
            //var ad = new AgentDefinition();



            if (aIAgent == null)
            {
                _logger.LogInformation("ai AGnet is null.");


                aIAgent = client1.GetChatClient("gpt-5.1-chat")
                                                .CreateAIAgent(instructions: Ai_Agent_instructions,
                                                tools: [
                                                    AIFunctionFactory.Create(CRM_Tools.GetDatafromDynamicsCRM,"get_data_from_dynamics_crm"),
                                                    AIFunctionFactory.Create(CRM_Tools.GetRetrieveRecordChangeHistory,"Get_Audit_Changes_from_dynamics_crm"),
                                                    AIFunctionFactory.Create(CRM_Tools.CreateActivity,"Create_activity_record_in_dynamics_CRM"),
                                                ]);
            }

            if (agentThread == null || clear?.ToLower() == "true")
            {
                _logger.LogInformation("Creating new thread.");
                agentThread = aIAgent.GetNewThread();
            }

            var http_response = req.CreateResponse(HttpStatusCode.OK);

            if (clear?.ToLower() != "true")
            {

                //Microsoft.Extensions.AI.ChatMessage chatMessage = new Microsoft.Extensions.AI.ChatMessage();
                Microsoft.Extensions.AI.ChatMessage chatMessage = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, input);

                await foreach (var chunk in aIAgent.RunStreamingAsync(chatMessage, agentThread))
                {
                    foreach (var eachContent in chunk.Contents)
                    {
                        if (eachContent.GetType() == typeof(TextContent))
                        {
                            var eachChunk_Content = ((TextContent)eachContent).Text;
                            await http_response.WriteStringAsync(eachChunk_Content);
                        }
                    }
                }
            }

            return http_response;
        }
    }

    //public class OpenAI1
    //{
    //    public ILogger<Function1> _logger { get; set; }
    //    private AIAgent aIAgent { get; set; }
    //    private AgentThread agentThread { get; set; }

    //    public List<string> History = new();

    //    //public static async Task<HttpResponseData> Ask_OpenAI_Agent(string endPoint, string key, string input, HttpRequestData req)
    //    //{
    //    //    _logger.LogInformation("OpenAI_Agent started.");
    //    //    CRM_Tools._logger = _logger;

    //    //    //var input = req.ReadAsStringAsync().Result;
    //    //    string prompt = string.Join("\n", History) + "\nUser: " + input;

    //    //    string Ai_Agent_instructions = @$"You are a helpful Dynamics CRM assistant.

    //    //                After giving your answer, ALWAYS ask a follow-up question to keep the conversation going.
    //    //                The follow-up question must be relevant to the user’s last query.

    //    //                Do NOT show FetchXML or reveal any tool calls.

    //    //                Always respond in rich-text Markdown with:
    //    //                        - Emojis
    //    //                        - Headings
    //    //                        - Tables
    //    //                        - Bullet points
    //    //                        - Base64 PNG charts";


    //    //    AzureOpenAIClient client1 = new AzureOpenAIClient(new Uri(endPoint), new System.ClientModel.ApiKeyCredential(key));
    //    //    if (aIAgent == null)
    //    //    {
    //    //        _logger.LogInformation("ai AGnet is null.");


    //    //        aIAgent = client1.GetChatClient("gpt-5.1-chat")
    //    //                                        .CreateAIAgent(instructions: Ai_Agent_instructions,
    //    //                                        tools: [
    //    //                                            AIFunctionFactory.Create(CRM_Tools.GetDatafromDynamicsCRM,"get_data_from_dynamics_crm")
    //    //                                        ]);
    //    //    }

    //    //    if (aIAgent == null)
    //    //    {
    //    //        agentThread = aIAgent.GetNewThread();

    //    //    }


    //    //    var http_response = req.CreateResponse(HttpStatusCode.OK);

    //    //    Microsoft.Extensions.AI.ChatMessage chatMessage = new Microsoft.Extensions.AI.ChatMessage();
    //    //    //Microsoft.Extensions.AI.ChatMessage chatMessage = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, input);

    //    //    await foreach (var chunk in aIAgent.RunStreamingAsync(chatMessage, agentThread))
    //    //    {
    //    //        foreach (var eachContent in chunk.Contents)
    //    //        {
    //    //            if (eachContent.GetType() == typeof(TextContent))
    //    //            {
    //    //                var eachChunk_Content = ((TextContent)eachContent).Text;
    //    //                await http_response.WriteStringAsync(eachChunk_Content);
    //    //            }
    //    //        }
    //    //    }

    //    //    return http_response;
    //    //}

    //    public async Task<HttpResponseData> Ask_OpenAI__Persistent_Agent(string endPoint, string key, string input, HttpRequestData req)
    //    {
    //        _logger.LogInformation("OpenAI_Agent started.");
    //        CRM_Tools._logger = _logger;

    //        //var input = req.ReadAsStringAsync().Result;
    //        string prompt = string.Join("\n", History) + "\nUser: " + input;

    //        string Ai_Agent_instructions = """
    //            You are a Microsoft Dynamics CRM data assistant.
    //            Do not show fetchXml query in response unless explicitly asks.
    //            Always flatten the CRM Entity data:
    //                - Convert AliasedValue to direct value
    //                - Convert EntityReference to name (Id if no name)
    //                - Convert OptionSetValue to label (and integer)
    //                - Convert Money to decimal
    //                - Convert BooleanManagedProperty to boolean
    //            """;

    //        AzureOpenAIClient client1 = new AzureOpenAIClient(new Uri(endPoint), new System.ClientModel.ApiKeyCredential(key));
    //        //var ad = new AgentDefinition();

    //        if (aIAgent == null)
    //        {
    //            _logger.LogInformation("ai AGnet is null.");


    //            aIAgent = client1.GetChatClient("gpt-5-chat")
    //                                            .CreateAIAgent(instructions: Ai_Agent_instructions,
    //                                            tools: [
    //                                                AIFunctionFactory.Create(CRM_Tools.GetDatafromDynamicsCRM,"get_data_from_dynamics_crm"),
    //                                                //AIFunctionFactory.Create(CRM_Tools.Get_Audit_Changes,"Get_Audit_Changes_from_dynamics_crm"),
    //                                                AIFunctionFactory.Create(CRM_Tools.CreateActivity,"Create_activity_record_in_dynamics_CRM"),
    //                                            ]);
    //        }
    //        if (agentThread == null)
    //        {
    //            _logger.LogInformation("Creating new thread.");
    //            agentThread = aIAgent.GetNewThread();
    //        }

    //        var http_response = req.CreateResponse(HttpStatusCode.OK);

    //        //Microsoft.Extensions.AI.ChatMessage chatMessage = new Microsoft.Extensions.AI.ChatMessage();
    //        Microsoft.Extensions.AI.ChatMessage chatMessage = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, input);

    //        await foreach (var chunk in aIAgent.RunStreamingAsync(chatMessage, agentThread))
    //        {
    //            foreach (var eachContent in chunk.Contents)
    //            {
    //                if (eachContent.GetType() == typeof(TextContent))
    //                {
    //                    var eachChunk_Content = ((TextContent)eachContent).Text;
    //                    await http_response.WriteStringAsync(eachChunk_Content);
    //                }
    //            }
    //        }

    //        return http_response;
    //    }
    //}
}
