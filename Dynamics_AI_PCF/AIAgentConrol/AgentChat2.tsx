import * as React from "react";
import  { useState, useRef, useEffect } from "react";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";

interface Message {
    role: "user" | "assistant";
    content: string;
    type?: "text" | "image" | "video" | "audio" | "file";
    url?: string;
}

export default function AgentChat(props: { 
    onSend: (msg: string) => Promise<string>;
    height?: string;
}) {
    const [messages, setMessages] = useState<Message[]>([]);
    const [input, setInput] = useState("");
    const fileInputRef = useRef<HTMLInputElement>(null);
    const chatRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        chatRef.current?.scrollTo(0, chatRef.current.scrollHeight);
    }, [messages]);

    const sendMessage = async () => {
        if (!input.trim()) return;

        const userMsg: Message = { role: "user", content: input, type: "text" };
        setMessages((prev) => [...prev, userMsg]);

        const reply = await props.onSend(input);
        const botMsg: Message = { role: "assistant", content: reply, type: "text" };
        
        setMessages((prev) => [...prev, botMsg]);
        setInput("");
    };

    const handleFileUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (!file) return;

        const url = URL.createObjectURL(file);
        const ext = file.name.split(".").pop()?.toLowerCase();

        let type: Message["type"] = "file";
        if (["jpg", "jpeg", "png", "gif", "webp"].includes(ext!)) type = "image";
        if (["mp4", "webm"].includes(ext!)) type = "video";
        if (["mp3", "wav", "ogg"].includes(ext!)) type = "audio";

        setMessages((prev) => [
            ...prev,
            { role: "user", content: file.name, type, url }
        ]);
    };

    const handleKeyDown = (e: React.KeyboardEvent) => {
            if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                sendMessage();
            }
        };
    
    return (
        <div style={{ height: props.height || "100%", display: "flex", flexDirection: "column" }}>
            {/* Chat Box */}
            <div 
                ref={chatRef}
                className="chat-box"
                style={{ flex: 1, overflowY: "auto", padding: "10px" }}
            >
                {messages.map((m, i) => (
                    <div
                        key={i}
                        className={m.role === "user" ? "msg-user" : "msg-bot"}
                    >
                        {m.type === "text" && (
                            <ReactMarkdown remarkPlugins={[remarkGfm]}>
                                {m.content}
                            </ReactMarkdown>
                        )}

                        {m.type === "image" && (
                            <img src={m.url} style={{ maxWidth: "250px", borderRadius: "10px" }} />
                        )}

                        {m.type === "video" && (
                            <video src={m.url} controls style={{ maxWidth: "250px" }} />
                        )}

                        {m.type === "audio" && (
                            <audio src={m.url} controls />
                        )}

                        {m.type === "file" && (
                            <a href={m.url} download>{m.content}</a>
                        )}
                    </div>
                ))}
            </div>

            {/* Input Bar */}
            <div className="input-bar">
                <button
                    className="file-btn"
                    onClick={() => fileInputRef.current?.click()}
                >
                    📎
                </button>

                <input
                    ref={fileInputRef}
                    type="file"
                    style={{ display: "none" }}
                    onChange={handleFileUpload}
                />

                <input
                    className="text-input"
                    value={input}
                    onChange={(e) => setInput(e.target.value)}
                    placeholder="Type your message…"
                    onKeyDown={handleKeyDown}
                />

                <button className="send-btn" onClick={sendMessage}>
                    ➤
                </button>
            </div>
        </div>
    );
}
