namespace SteamItchIoDeployerCore
{
    /// <summary>
    /// Substitutes the <c>{Version}</c>, <c>{Date}</c>, <c>{DateTime}</c>, and <c>{GitSHA}</c>
    /// macros used in Steam build descriptions and itch.io user-version strings. Formatting of
    /// the date/time values is left to the caller (via <paramref name="dateText"/> /
    /// <paramref name="dateTimeText"/>) since each engine already had its own preferred format
    /// before this was shared, and changing it would be a user-visible behavior change.
    /// </summary>
    public static class MacroResolver
    {
        public static string Resolve(string template, string version, string dateText, string dateTimeText, string gitSha)
        {
            if (string.IsNullOrEmpty(template))
                return version ?? string.Empty;

            return template
                .Replace("{Version}", version ?? string.Empty)
                .Replace("{Date}", dateText ?? string.Empty)
                .Replace("{DateTime}", dateTimeText ?? string.Empty)
                .Replace("{GitSHA}", gitSha ?? string.Empty);
        }
    }
}
