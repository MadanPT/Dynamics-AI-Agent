import * as React from "react";
import { agentAdapter } from "./Agent_Adapter";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import rehypeRaw from "rehype-raw";


//type Message = { role: "user" | "assistant"; content: string };
export interface Message {
  //id?: string;
  role: "user" | "assistant";
  content: string;
  // name?: string;          // For functions
  // toolCallId?: string;    // For tool responses
}
const styles = `
/* Row containing avatar + bubble */
.msg-row {
    display: flex;
    align-items: flex-start;
    margin-bottom: 12px;
    width: 100%;
}

/* Left alignment for AI */
.msg-row.ai {
    justify-content: flex-start;
}

/* Right alignment for user */
.msg-row.user {
    justify-content: flex-end;
}

/* Avatars */
.avatar {
    width: 32px;
    height: 32px;
    border-radius: 50%;
    font-size: 13px;
    font-weight: 600;
    display: flex;
    align-items: center;
    justify-content: center;
    margin: 0 8px;
    flex-shrink: 0;
}

/* AI avatar */
.ai-avatar {
    background: #e5e7eb;
    color: #111;
}

/* User avatar */
.user-avatar {
    background: #0f62fe;
    color: white;
}

/* Chat bubbles */
.msg {
    max-width: 80%;
    padding: 10px;
    border-radius: 8px;
    line-height: 1.5;
}

/* AI bubble */
.msg.ai {
    background: #fff;
    border: 1px solid #ddd;
    margin-right: auto;
}

/* User bubble */
.msg.user {
    background: #e6f0ff;
    margin-left: auto;
}

/* Typing indicator container */
.typing-row {
    display: flex;
    align-items: center;
    margin: 6px 0;
}

/* Three dots animation */
.typing-indicator {
    display: flex;
    gap: 3px;
    padding: 8px 12px;
    background: #fff;
    border-radius: 8px;
    border: 1px solid #ddd;
}

.typing-indicator span {
    width: 6px;
    height: 6px;
    background: #999;
    border-radius: 50%;
    display: inline-block;
    animation: typing 1.4s infinite ease-in-out both;
}

.typing-indicator span:nth-child(1) {
    animation-delay: -0.32s;
}
.typing-indicator span:nth-child(2) {
    animation-delay: -0.16s;
}

@keyframes typing {
    0%, 80%, 100% { opacity: 0.3; transform: scale(0.8); }
    40% { opacity: 1; transform: scale(1.2); }
}

`;

export default function DevUI(props: { endpoint: string; apiKey: string; agentName?: string; showHeader?: boolean }) {
  const { endpoint, apiKey, agentName, showHeader = true } = props;
  const [messages, setMessages] = React.useState<Message[]>([]);
  const [input, setInput] = React.useState("");
  const [isSending, setIsSending] = React.useState(false);
  const adapterRef = React.useRef(agentAdapter({ endpoint, apiKey, agentName: agentName ?? "gpt-35-turbo" }));
  const chatWindowRef = React.useRef<HTMLDivElement | null>(null);

  React.useEffect(() => {
    // inject styles once
    if (!document.getElementById("devui-pcf-styles")) {
      const style = document.createElement("style");
      style.id = "devui-pcf-styles";
      style.textContent = styles;
      document.head.appendChild(style);
    }
  }, []);

  //const chatWindowRef = React.useRef(null);

    // Auto scroll to bottom on new messages
    React.useEffect(() => {
        if (chatWindowRef.current) {
            chatWindowRef.current.scrollTop = chatWindowRef.current.scrollHeight;
        }
    }, [messages]);

  async function sendMessage() {
    const text = input.trim();
    if (!text) return;
    setMessages((m) => [...m, { role: "user", content: text }]);
    setInput("");
    setIsSending(true);

    try {
      const stream = adapterRef.current.runAsStream(text);
      let assistantContent = "";
      for await (const event of stream) {
        if (event.type === "response.output_text") {
          assistantContent += event.delta ?? event.text ?? "";
          // update in-flight assistant message
          setMessages((prev) => {
            const copy = prev.slice();
            // if last message is assistant, replace it; otherwise push
            if (copy.length && copy[copy.length - 1].role === "assistant") {
              copy[copy.length - 1] = { role: "assistant", content: assistantContent };
            } else {
              copy.push({ role: "assistant", content: assistantContent });
            }
            return copy;
          });
        }
      }
    } catch (err) {
      setMessages((m) => [...m, { role: "assistant", content: "Error: " + String(err) }]);
    }

    setIsSending(false);
  }

  return (
          <div className="devui-pcf">
  
              {showHeader && (
                  <div className="devui-header">
                      <div className="devui-title">{agentName}</div>
                      <div className="small"></div>
                  </div>
              )}
  
              <div className="devui-body">
  
                  {/* Scrollable chat window */}
                  <div className="messages" ref={chatWindowRef}>
                      {messages.map((m, i) => (
                        //   <div key={i} className={`msg ${m.role === "user" ? "user" : "ai"}`}>
                          <div className="msg msg-user">
                            <div className="bubble">
                              {/* Markdown + HTML + Multimedia */}
                              <ReactMarkdown
                                  children={m.content}
                                  remarkPlugins={[remarkGfm]}
                                  rehypePlugins={[rehypeRaw]}
                                  components={{
                                      img: (props) => (
                                          <img {...props} style={{ maxWidth: "100%", borderRadius: 8 }} />
                                      ),
                                      video: (props) => (
                                          <video controls {...props} style={{ maxWidth: "100%", borderRadius: 8 }} />
                                      ),
                                      a: (props) => (
                                          <a {...props} target="_blank" rel="noopener noreferrer" />
                                      ),
                                  }}
                              />
                              </div>
                          </div>
                      ))}
                  </div>
  
                  {/* Bottom fixed input bar */}
                  <div className="input-area">
                      <input
                          value={input}
                          onChange={(e) => setInput(e.target.value)}
                          placeholder={isSending ? "Sending..." : "Ask your agent..."}
                          disabled={isSending}
                          onKeyDown={(e) => {
                              if (e.key === "Enter") sendMessage();
                          }}
                      />
                      <button onClick={sendMessage} disabled={isSending}>
                          {isSending ? "Working..." : "Send"}
                      </button>
                  </div>
              </div>
          </div>
      );
}

function escapeHtml(s: string) {
  return s.replace(/[&<>\\"]/g, (c) => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': '&quot;' }[c] ?? c));
}
