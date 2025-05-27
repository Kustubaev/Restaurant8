using Microsoft.AspNetCore.Hosting;

public static class IWebHostEnvironmentExtensions
{
    public static string WebRootUrl(this IWebHostEnvironment env)
    {
        // Для разработки может работать так:
        return env.IsDevelopment() ? "https://localhost:7256" : $"https://{env.ApplicationName}";
    }
}