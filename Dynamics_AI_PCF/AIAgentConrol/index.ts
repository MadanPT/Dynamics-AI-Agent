// import { IInputs, IOutputs } from "./generated/ManifestTypes";
// import { ChatGPTService } from "./AI_Agent_Service";

// export class AIAgentConrol implements ComponentFramework.StandardControl<IInputs, IOutputs> {

//     private container: HTMLDivElement;
//     private input: HTMLInputElement;
//     private button: HTMLButtonElement;
//     private chatWindow: HTMLDivElement;

//     private service: ChatGPTService;

//     /**
//      * Empty constructor.
//      */
//     constructor() {
//         // Empty
//     }

//     /**
//      * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
//      * Data-set values are not initialized here, use updateView.
//      * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
//      * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
//      * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
//      * @param container If a control is marked control-type='standard', it will receive an empty div element within which it can render its content.
//      */
//     public init(
//         context: ComponentFramework.Context<IInputs>,
//         notifyOutputChanged: () => void,
//         state: ComponentFramework.Dictionary,
//         container: HTMLDivElement
//     ): void {
//         // Add control initialization code
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

//      private async onSend(): Promise<void> {
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

//     /**
//      * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
//      * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
//      */
//     public updateView(context: ComponentFramework.Context<IInputs>): void {
//         // Add code to update control view
//     }

//     /**
//      * It is called by the framework prior to a control receiving new data.
//      * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as "bound" or "output"
//      */
//     public getOutputs(): IOutputs {
//         return {
//             prompt: this.input.value
//         };
//     }

//     /**
//      * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
//      * i.e. cancelling any pending remote calls, removing listeners, etc.
//      */
//     public destroy(): void {
//         // Add code to cleanup control if necessary
//     }
// }
