import * as React from "react";
import { createRoot, Root } from "react-dom/client";
import DevUI from "./Agent_UI";
import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class DevUIPCF implements ComponentFramework.StandardControl<IInputs, IOutputs> {
  private _container: HTMLDivElement;
  private _root: Root; // React root
  private _context: ComponentFramework.Context<IInputs>;

  constructor() {
        // Empty
    }

  public init(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void, state: ComponentFramework.Dictionary, container: HTMLDivElement) {
    this._context = context;
    this._container = document.createElement("div");
    this._container.style.width = "100%";
    this._container.style.height = "100%";
    container.appendChild(this._container);

    // create react root
    this._root = createRoot(this._container);

    // render initial
    this.renderControl();
  }

  private renderControl() {
    const endpoint ="https://dynamics-ai-functions.azurewebsites.net/api/Dynmics-AI-Function?code=82_aA8j3R8_lQoZC0SY7xLq5MLXioBxrW89SHjRffZejAzFuKO73UQ==" ;//this._context?.parameters?.endpoint?.raw ?? this._context?.parameters?.apiEndpoint?.raw ?? "https://YOUR-AZURE-ENDPOINT.openai.azure.com";
    const apiKey = "";//this._context?.parameters?.apiKey?.raw ?? "YOUR_API_KEY";
    const agentName ="Dynamics AI Agent"; //this._context?.parameters?.agentName?.raw ?? "gpt-35-turbo";

    this._root.render(React.createElement(DevUI, { endpoint, apiKey, agentName }));
  }

  public updateView(context: ComponentFramework.Context<IInputs>): void {
    this._context = context;
    // re-render with updated params (if the host changed API key or endpoint via property pane)
    this.renderControl();
  }

  public getOutputs(): IOutputs {
    return {};
  }

  public destroy(): void {
    try {
      this._root.unmount();
    } catch (e) {
        console.log(e);
    }
  }
}

// PCF requires the exported init function name to match your manifest's constructor
// export function DevUIPCF_init(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void, state: ComponentFramework.Dictionary, container: HTMLDivElement) {
//   const ctl = new DevUIPCF();
//   ctl.init(context, notifyOutputChanged, state, container);
//   return ctl;
// }