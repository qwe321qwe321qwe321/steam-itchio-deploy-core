using System.Text.RegularExpressions;

namespace SteamItchIoDeployerCore
{
    public enum CliLogLevel
    {
        Info,
        Error,
        SteamGuardRequired,
        AuthFailure,
    }

    /// <summary>
    /// Classifies a single line of steamcmd/butler output so a host UI can react to Steam Guard
    /// prompts, authentication failures, and generic errors without re-implementing the patterns
    /// steamcmd and butler are known to print.
    /// </summary>
    public static class CliOutputClassifier
    {
        public static readonly Regex SteamGuardRequiredPattern = new Regex(
            @"(not been authenticated for your account using Steam Guard|" +
            @"Steam Guard code:|" +
            @"Steam Guard code required|" +
            @"FAILED login with result code RequireTwoFactorCode|" +
            @"FAILED login with result code RequirePasswordEntry|" +
            @"Enter the current code from your Steam Guard)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Regex SteamAuthFailurePattern = new Regex(
            @"(Invalid Password|Two-factor code mismatch|" +
            @"Login Failure|Logging in user.*Failed|FAILED login with result code InvalidPassword)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Regex ButlerAuthFailurePattern = new Regex(
            @"(authentication not complete|api key|unauthorized|forbidden|invalid api key|not logged in)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Regex GenericErrorPattern = new Regex(
            @"(ERROR!|error:|FAILED|Build Failed|Upload Failed|rate limit exceeded)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsSteamGuardRequired(string text) =>
            !string.IsNullOrEmpty(text) && SteamGuardRequiredPattern.IsMatch(text);

        /// <summary>
        /// Classifies a single output line. <paramref name="fromStdErr"/> unconditionally
        /// promotes the line to <see cref="CliLogLevel.Error"/> unless it already matches a more
        /// specific pattern, mirroring how steamcmd/butler route real failures to stderr.
        /// </summary>
        public static CliLogLevel Classify(CliToolKind toolKind, string line, bool fromStdErr)
        {
            if (string.IsNullOrEmpty(line))
                return CliLogLevel.Info;

            if (toolKind == CliToolKind.SteamCmd)
            {
                if (SteamGuardRequiredPattern.IsMatch(line))
                    return CliLogLevel.SteamGuardRequired;

                if (SteamAuthFailurePattern.IsMatch(line))
                    return CliLogLevel.AuthFailure;
            }

            if (toolKind == CliToolKind.Butler && ButlerAuthFailurePattern.IsMatch(line))
                return CliLogLevel.AuthFailure;

            if (fromStdErr || GenericErrorPattern.IsMatch(line))
                return CliLogLevel.Error;

            return CliLogLevel.Info;
        }
    }
}
