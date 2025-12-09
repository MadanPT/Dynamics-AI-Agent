using Azure.AI.OpenAI;
using Dynamics_AI_Function_App;
using Microsoft.Agents.AI;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;
using OpenAI;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

//string endPoint = "https://madha-miqy8w3f-eastus2.openai.azure.com/";
//string key = "7rXuEGCNFNdwJj2V5RF70I2adcsp44s1AudTxka2zFygqLvbGTtrJQQJ99BLACHYHv6XJ3w3AAAAACOGGSMx";

//AzureOpenAIClient client1 = new AzureOpenAIClient(new Uri(endPoint), new System.ClientModel.ApiKeyCredential(key));
//AIAgent agent = client1.GetChatClient("gpt-5-chat")
//                                .CreateAIAgent(instructions: "you are a Dynamics CRM Agent. Do not show queries.",
//                                tools: [
//                                    AIFunctionFactory.Create(CRM_Tools.GetDatafromDynamicsCRM,"get_data_from_dynamics_crm")
//                                ]);
//using IHost app = FunctionsApplication
//    .CreateBuilder(args)
//    .ConfigureFunctionsWebApplication()
//    .ConfigureDurableAgents(options => options.AddAIAgent(agent))
//    .Build();
//app.Run();

builder.Build().Run();
