/*
*This is auto generated from the ControlManifest.Input.xml file
*/

// Define IInputs and IOutputs Type. They should match with ControlManifest.
export interface IInputs {
    prompt: ComponentFramework.PropertyTypes.StringProperty;
    entity_id: ComponentFramework.PropertyTypes.StringProperty;
    entity_name: ComponentFramework.PropertyTypes.StringProperty;
}
export interface IOutputs {
    prompt?: string;
}
