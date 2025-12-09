import  { useState, useRef, useEffect } from "react";
import "./AI_Agent_UI.css"; // Use the previous CSS
import * as React from "react";

export type ChatMessage = {
    id: string;
    role: "user" | "assistant";
    content: string;
};

export interface AgentChatProps {
    onSend: (message: string,entity_name?:string, record_id?:string) => Promise<string>; 
    onClear: () => Promise<string>;
    height?: string; 
    record_id?:string,
    entity_name?:string
}

const AgentChat: React.FC<AgentChatProps> = ({ onSend,onClear,entity_name,record_id, height = "100%" }) => {

    const [messages, setMessages] = useState<ChatMessage[]>([]);
    const [input, setInput] = useState("");
    const [loading, setLoading] = useState(false);
    const bodyRef = useRef<HTMLDivElement | null>(null);

    /* Auto-scroll when messages update */
    useEffect(() => {
        if (bodyRef.current) {
            bodyRef.current.scrollTop = bodyRef.current.scrollHeight;
        }
    }, [messages]);

     useEffect(() => {
        sendMessage();
    }, [entity_name,record_id]);

    const sendMessage = async () => {
       // if (!input.trim() || loading) return;
        if((!input.trim() && !entity_name?.trim() && !record_id?.trim())|| loading) return;

        const userMsg: ChatMessage = {
            id: crypto.randomUUID(),
            role: "user",
            content: input
        };

        setMessages((prev) => [...prev, userMsg]);
        setInput("");
        setLoading(true);

        try {
            const reply = await onSend(userMsg.content,entity_name,record_id);

            const agentMsg: ChatMessage = {
                id: crypto.randomUUID(),
                role: "assistant",
                content: reply
            };

            setMessages((prev) => [...prev, agentMsg]);

        } catch (e) {
            const errMsg: ChatMessage = {
                id: crypto.randomUUID(),
                role: "assistant",
                content: "⚠️ Error: Failed to send message."
            };
            setMessages((prev) => [...prev, errMsg]);
        }

        setLoading(false);
    };

    const clearMessage=async ()=>{
        setMessages([]);
        await onClear();
    };
    const handleKeyDown = (e: React.KeyboardEvent) => {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            sendMessage();
        }
    };

    return (
        <div className="pcf-devui" style={{ height }}>
            {/* Header */}
            <header className="devui-header">
                <div className="title">
                     <button className="send-btn" onClick={clearMessage}>
                    Clear
                    </button>
                </div>
            </header>

            {/* Chat Body */}
            <main className="devui-body" ref={bodyRef}>
                <div className="messages">
                    {messages.map((msg) => (
                        <div
                            key={msg.id}
                            className={`msg msg-${msg.role}`}>

                        {/* <div className="bubble"> */}
                            <div className="bubble" dangerouslySetInnerHTML={{__html:convertToHtmlTable(msg.content)}}>
                            {/* <div className="bubble" dangerouslySetInnerHTML={{__html:msg.content}}> */}
                                 {/* {msg.content}  */}
                            </div>
                        </div>
                    ))}

                    {loading && (
                        <div className="msg msg-assistant">
                            <div className="bubble">...</div>
                        </div>
                    )}
                </div>
            </main>

            {/* Input Footer */}
            <footer className="devui-footer">
                <div className="input-wrap">
                    <textarea
                        className="devui-input"
                        value={input}
                        placeholder="Ask something..."
                        onChange={(e) => setInput(e.target.value)}
                        onKeyDown={handleKeyDown}
                    />
                </div>

                <button className="send-btn" onClick={sendMessage}>
                    Send
                </button>
            </footer>
        </div>
    );
};

function convertToHtmlTable(text: string): string {
    // Check if content looks like a table
    if (!text.includes("|")) return text;

    const rows = text
        .trim()
        .split("\n")
        .filter((line) => line.trim().startsWith("|"));

    if (rows.length < 2) return text;

    const html =
        "<table>" +
        rows
            .map((row) => {
                const cells = row
                    .split("|")
                    .filter((c) => c.trim() !== "")
                    .map((c) => `<td>${c.trim()}</td>`)
                    .join("");

                return `<tr>${cells}</tr>`;
            })
            .join("") +
        "</table>";

    return html;
}


export default AgentChat;
