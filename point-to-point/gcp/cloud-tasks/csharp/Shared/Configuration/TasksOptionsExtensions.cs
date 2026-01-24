using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shared.Configuration;

public static class TasksOptionsExtensions
{
    public static OptionsBuilder<TasksSettings> AddTasksSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddOptions<TasksSettings>()
            .Bind(configuration.GetSection(TasksSettings.SectionName))
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.ProjectId), "Tasks:ProjectId is required.")
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.LocationId), "Tasks:LocationId is required.")
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.QueueId), "Tasks:QueueId is required.")
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.TargetUrl), "Tasks:TargetUrl is required.")
            .ValidateOnStart();
    }
}
