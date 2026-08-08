namespace SteamItchIoDeployerCore
{
    /// <summary>Human-readable explanations for steamcmd process exit codes.</summary>
    public static class SteamExitCodeDescriptions
    {
        public static string Describe(int exitCode)
        {
            switch (exitCode)
            {
                case 0: return "Success.";
                case 1: return "Unknown / general error.";
                case 2: return "Steam session error — already logged in elsewhere, or generic login failure.";
                case 3: return "No connection to the Steam network. Check your internet connection.";
                case 4: return "Connection timeout or invalid command-line argument.";
                case 5: return "Steam API / SDK initialisation failed.";
                case 6: return "Build commit failed. Content was uploaded but could not be finalised. Common causes: SetLive branch not eligible yet, invalid branch name, or a transient Valve-side error.";
                case 7: return "Too many failed login attempts. Wait before retrying.";
                case 8: return "Rate limit exceeded — too many steamcmd operations in a short period. Wait and retry.";
                case 42: return "Rate limit exceeded (Valve-side throttle). Wait several minutes before retrying.";
                default: return $"Undocumented exit code {exitCode}. Check the steamcmd log for details.";
            }
        }
    }
}
