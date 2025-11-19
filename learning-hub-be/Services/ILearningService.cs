namespace learning_hub_fe.Services
{
    public interface ILearningService
    {
        Task<LearningPathResponse> GeneratePathAsync(string topic);
    }
}
