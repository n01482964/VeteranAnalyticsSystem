namespace VeteranAnalyticsSystem.Extensions;


public static class ConfigurationExtensions
{
    /// <summary>
    /// Retrieves a required configuration value by key. Throws an exception if the key is missing or the value is null/empty.
    /// </summary>
    /// <param name="configuration">The IConfiguration instance.</param>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the key is not found or the value is null/empty.</exception>
    public static string GetRequiredValue<T>(this IConfiguration configuration, string key)
    {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Configuration value for key '{key}' is required but was not found or is empty.");
        }

        return value;
    }
}
