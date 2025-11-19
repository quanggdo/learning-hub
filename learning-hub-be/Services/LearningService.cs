
using System.Text.Json;
using Google.GenAI;
using Google.GenAI.Types;

namespace learning_hub_fe.Services
{
    public class LearningService : ILearningService
    {
        private readonly Client geminiClient;

        public LearningService(Client geminiClient)
        {
            this.geminiClient = geminiClient;
        }
        public async Task<LearningPathResponse> GeneratePathAsync(string topic)
        {
                string jsonSchemaExample = """
              "topic": "Tên chủ đề",
              "steps": [
                { "id": 1, "title": "Tiêu đề bước 1", "description": "Mô tả bước 1" },
                { "id": 2, "title": "Tiêu đề bước 2", "description": "Mô tả bước 2" }
              ]
            }
            """;

                string prompt = $"""
            Bạn là một trợ lý lập kế hoạch học tập.
            Tạo một lộ trình học tập 5 bước cho chủ đề: "{topic}".
            
            QUAN TRỌNG: Chỉ trả về MỘT đối tượng JSON. KHÔNG dùng markdown (```json).
            JSON phải tuân thủ nghiêm ngặt theo cấu trúc sau:
            {jsonSchemaExample}
            """;
                var config = new GenerateContentConfig
                {
                    ResponseMimeType = "application/json"
                };
                var aiResponse = await geminiClient.Models.GenerateContentAsync(
                    model: "gemini-2.5-pro",
                    contents: prompt,
                    config: config
                );

                string? jsonResponse = aiResponse.Candidates?.FirstOrDefault()
                                           ?.Content.Parts.FirstOrDefault()
                                           ?.Text;

                if (string.IsNullOrEmpty(jsonResponse))
                {
                    throw new Exception("AI đã trả về 1 phản hồi rỗng");
                }

                Console.WriteLine("--- AI Response (Raw) ---");
                Console.WriteLine(jsonResponse);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                LearningPathResponse? learningPath = JsonSerializer.Deserialize<LearningPathResponse>(jsonResponse, options);
                return learningPath;
            }
            
        }
    }

