//type StreamEvent = { type: string; delta?: string; text?: string };

export interface StreamEvent {
  type: string; // fallback
  delta?: string;
  text?: string;
  error?: string;
}

// Lightweight adapter: provides runAsStream and run methods. Replace with @azure/ai-agents-js for real integration.
export const agentAdapter = (config: { endpoint: string; apiKey: string; agentName?: string }) => {
  // NOTE: This adapter uses fetch and does not implement a true server-sent-events streaming protocol.
  // For production streaming, prefer the official SDK which exposes async iterators over events.

  async function run(prompt: string) {
    // Use Azure OpenAI chat completions endpoint or your agent endpoint.
    // Placeholder: user must fill in proper endpoint and api-key handling.
    //const url = `${config.endpoint}/openai/deployments/${config.agentName}/chat/completions?api-version=2024-02-15-preview`;
    const url = `${config.endpoint}`;
    const body = {
      messages: [{ role: "user", content: prompt }],
      //max_tokens: 800
    };

    const res = await fetch(url, {
      method: "POST",
      headers: {
        "Content-Type":"text/plain",//"application/json",
        //"api-key": config.apiKey
      },
      body:prompt //JSON.stringify(body)
    });

    const json = await res.text();
    // Try to extract assistant content from multiple plausible response shapes
    //const content = json?.choices?.[0]?.message?.content ?? json?.choices?.[0]?.text ?? JSON.stringify(json);
    return json;
  }

  async function* runAsStream(prompt: string) {
    // Fallback streaming that yields the full response at once (for environments without SSE)
    const full = await run(prompt);
    yield { type: "response.output_text", delta: full } as StreamEvent;
  }

  return { run, runAsStream };
};