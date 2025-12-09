// import { IInputs, IOutputs } from "./generated/ManifestTypes";
// import { ChatGPTService } from "./AI_Agent_Service";

// export class ChatGPTPCF implements ComponentFramework.StandardControl<IInputs, IOutputs> {
//     private container: HTMLDivElement;
//     private input: HTMLInputElement;
//     private button: HTMLButtonElement;
//     private chatWindow: HTMLDivElement;

//     private service: ChatGPTService;

//     constructor(private context:ComponentFramework.Context<IInputs>, private notifyOutputChanged:() => void, private state:ComponentFramework.Dictionary, private host:HTMLDivElement) {}

//     public init(context:ComponentFramework.Context<IInputs>, notifyOutputChanged:() => void, state:ComponentFramework.Dictionary, container:HTMLDivElement): void {
//         this.container = document.createElement("div");
//         this.container.classList.add("chatgpt-container");

//         this.chatWindow = document.createElement("div");
//         this.chatWindow.classList.add("chat-window");

//         this.input = document.createElement("input");
//         this.input.placeholder = "Ask ChatGPT...";
//         this.input.classList.add("chat-input");

//         this.button = document.createElement("button");
//         this.button.innerText = "Send";
//         this.button.classList.add("chat-button");

//         this.button.addEventListener("click", () => this.onSend());

//         this.container.appendChild(this.chatWindow);
//         this.container.appendChild(this.input);
//         this.container.appendChild(this.button);

//         container.appendChild(this.container);

//         this.service = new ChatGPTService(context.parameters.apikey.raw ?? "");
//     }

//     private async onSend(): Promise<void> {
//         const question = this.input.value.trim();
//         if (!question) return;

//         this.addMessage("You", question);
//         this.input.value = "";

//         const reply = await this.service.ask_Azure_AI_API(question);
//         this.addMessage("ChatGPT", reply);
//     }

//     private addMessage(sender: string, message: string) {
//         const div = document.createElement("div");
//         div.className = sender === "You" ? "msg-user" : "msg-gpt";
//         div.innerHTML = `<strong>${sender}:</strong> ${message}`;
//         this.chatWindow.appendChild(div);
//         this.chatWindow.scrollTop = this.chatWindow.scrollHeight;
//     }

//     public updateView(context: ComponentFramework.Context<IInputs>): void {
//         console.log("updatevie");
//     }

//     public getOutputs(): IOutputs {
//         return {
//             prompt: this.input.value
//         };
//     }

//     public destroy(): void {
//         console.log("destroy called.");
//     }
// }
