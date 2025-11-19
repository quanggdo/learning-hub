import React, { useState } from 'react';
import axios from 'axios';
import './App.css';

// !!! QUAN TRỌNG: Thay đổi cổng 7123 thành cổng API .NET của bạn
const API_URL = "https://localhost:7274/api/learning/generate";

function App() {
  const [topic, setTopic] = useState("");
  const [learningPath, setLearningPath] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSubmit = async () => {
    if (isLoading) return;

    setIsLoading(true);
    setError(null);
    setLearningPath(null);

    try {
      const requestData = {
        topic: topic
      };

      const response = await axios.post(API_URL, requestData);
      setLearningPath(response.data);

    } catch (err) {
      console.error("Lỗi khi gọi API:", err);
      let errorMessage = "Không thể kết nối đến máy chủ. Bạn đã chạy Backend (Visual Studio) chưa?";
      
      if (err.response) {
        errorMessage = `Lỗi từ server: ${err.message}`;
      } else if (err.request) {
        errorMessage = "Không nhận được phản hồi từ máy chủ. Kiểm tra lại URL API và cài đặt CORS bên .NET.";
      }
      setError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="container">
      <header className="header">
        <h2>🤖 Trợ lý Học tập Cá nhân hóa</h2>
        <p>Nhập một chủ đề bạn muốn học, AI sẽ tạo lộ trình cho bạn.</p>
      </header>

      <div className="input-form">
        <input
          type="text"
          value={topic}
          onChange={(e) => setTopic(e.target.value)}
          placeholder="Ví dụ: Lập trình React Hooks"
          disabled={isLoading}
        />
        <button onClick={handleSubmit} disabled={isLoading || !topic.trim()}>
          {isLoading ? "Đang xử lý..." : "Tạo Lộ trình"}
        </button>
      </div>

      {error && (
        <div className="error-message">
          <p>{error}</p>
        </div>
      )}

      {learningPath && (
        <div className="results">
          <h3>Lộ trình học cho: {learningPath.topic}</h3>
          <ul>
            {learningPath.steps.map(step => (
              <li key={step.id}>
                <strong>{step.title}:</strong> {step.description}
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}

export default App;