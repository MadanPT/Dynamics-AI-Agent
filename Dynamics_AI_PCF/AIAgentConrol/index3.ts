import * as React from "react";
// import * as ReactDOM from "react-dom";
import AgentChat, { AgentChatProps } from "./AgentChat";
import { IInputs, IOutputs } from "./generated/ManifestTypes";
import { createRoot, Root } from "react-dom/client";

export class DynamicsAIAgent implements ComponentFramework.StandardControl<IInputs, IOutputs> {

    private container!: HTMLDivElement;
    private root!: Root;
private current_recordid?:string;
private current_entityname?:string;
private _onSend: (text: string, entity_name?:string, record_id?:string) => Promise<string>;
private _onClear: () => Promise<string>;

    public init(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void, state: ComponentFramework.Dictionary, container: HTMLDivElement) {
        this.container = container;

        this.current_recordid = context.parameters.entity_id.raw || "";
        this.current_entityname = context.parameters.entity_name.raw || "";   
        this._onSend=this.onSend.bind(this);
        this._onClear=this.onClear.bind(this);
        this.root = createRoot(this.container);
        this.root.render(React.createElement(AgentChat,{onSend:this._onSend,onClear:this._onClear}));
    }

    public updateView(context: ComponentFramework.Context<IInputs>): void {
        const recordid = context.parameters.entity_id.raw || "";
        const entity_name = context.parameters.entity_name.raw || "";   
       // if(recordid!==this.current_recordid || entity_name!== this.current_entityname){
            //this.root.render(React.createElement(AgentChat,{onSend:this._onSend,entity_name:entity_name,record_id:recordid}));
        //}
    }

    destroy() {
        if (this.root) {
            this.root.unmount();
        }
    
    }

     private async onSend(text: string, entity_name?:string, record_id?:string): Promise<string> {
            // Send to your Azure Agent
            // Example:
            const recId=record_id!=="val" && record_id!=="" && text===""?record_id:"";
            const enName=entity_name!=="val" && entity_name!=="" && text===""?entity_name:"";

        const endpoint =`https://dynamics-ai-functions.azurewebsites.net/api/Dynmics-AI-Function?code=82_aA8j3R8_lQoZC0SY7xLq5MLXioBxrW89SHjRffZejAzFuKO73UQ==&entity_id=${recId}&entity_name=${enName}` ;//this._context?.parameters?.endpoint?.raw ?? this._context?.parameters?.apiEndpoint?.raw ?? "https://YOUR-AZURE-ENDPOINT.openai.azure.com";
        // const endpoint=`http://localhost:7270/api/Dynmics-AI-Function?entity_id=${recId}&entity_name=${enName}`;

            const response = await fetch(endpoint, {
                method: "POST",
                headers: { "Content-Type": "text/plain" },
                body: text //JSON.stringify({ message: text })
            });

            const result = await response.text();
            return result;  // adjust to your API
        };

  private async onClear(): Promise<string> {

        const endpoint =`https://dynamics-ai-functions.azurewebsites.net/api/Dynmics-AI-Function?code=82_aA8j3R8_lQoZC0SY7xLq5MLXioBxrW89SHjRffZejAzFuKO73UQ==&clear=true` ;//this._context?.parameters?.endpoint?.raw ?? this._context?.parameters?.apiEndpoint?.raw ?? "https://YOUR-AZURE-ENDPOINT.openai.azure.com";
        // const endpoint=`http://localhost:7270/api/Dynmics-AI-Function?entity_id=${recId}&entity_name=${enName}`;

            const response = await fetch(endpoint, {
                method: "POST",
                headers: { "Content-Type": "text/plain" },
                body: "" //JSON.stringify({ message: text })
            });

            const result = await response.text();
            return result;  // adjust to your API
  };
}
