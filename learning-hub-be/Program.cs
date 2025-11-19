using Google.GenAI;
using Google.GenAI.Types;
using learning_hub_fe.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.Json;
var builder = WebApplication.CreateBuilder(args);

// === 1. ĐĂNG KÝ DỊCH VỤ ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddScoped<ILearningService, LearningService>();
// Đọc API Key từ file secrets.json
var geminiApiKey = builder.Configuration["Gemini:api-key"];

// Đăng ký dịch vụ Gemini (SDK mới)
// Chúng ta sẽ đăng ký nó dưới dạng Singleton
builder.Services.AddSingleton(provider =>
{
    return new Client(apiKey: geminiApiKey);
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(policyBuilder => policyBuilder
    .WithOrigins("http://localhost:5173") 
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapPost("/api/learning/generate",
    async (LearningPathRequest request, ILearningService learningService) =>
    {
        try
        {
            String topic = request.Topic;
            var response = await learningService.GeneratePathAsync(topic);
            return Results.Ok(response);
        }
        catch (Exception ex) { 
            Console.WriteLine(ex);
            return Results.Problem("Lỗi phía máy chủ " + ex);
        }
    });


app.Run();

public record LearningStep(int Id, string Title, string Description);
public record LearningPathResponse(string Topic, List<LearningStep> Steps);
public record LearningPathRequest(string Topic);

