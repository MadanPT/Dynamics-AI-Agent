// export class ChatGPTService {
//     private apiKey: string;

//     constructor(apiKey: string) {
//         this.apiKey = apiKey;
//     }

//     async ask_Azure_AI_API(prompt: string): Promise<string> {
//         const url = "https://YOUR-AZURE-OPENAI-ENDPOINT/openai/deployments/YOUR_MODEL/chat/completions?api-version=2024-02-15-preview";

//         const body = {
//             messages: [{ role: "user", content: prompt }],
//             max_tokens: 300,
//             temperature: 0.2
//         };

//         const response = await fetch(url, {
//             method: "POST",
//             headers: {
//                 "Content-Type": "application/json",
//                 "api-key": this.apiKey
//             },
//             body: JSON.stringify(body)
//         });

//         const json = await response.json();
//         return json.choices[0].message.content;
//     }
// }
