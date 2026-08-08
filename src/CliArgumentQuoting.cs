using System.Collections.Generic;
using System.Text;

namespace SteamItchIoDeployerCore
{
    /// <summary>
    /// Joins an argument token array into a single command-line string for hosts that must pass
    /// <c>ProcessStartInfo.Arguments</c> as one string instead of using <c>ArgumentList</c>
    /// (e.g. Unity, whose minimum-supported .NET Standard 2.0 API profile predates ArgumentList).
    /// Any token containing whitespace or a double quote is wrapped in quotes with inner quotes
    /// backslash-escaped.
    /// </summary>
    public static class CliArgumentQuoting
    {
        public static string Join(IEnumerable<string> arguments)
        {
            var sb = new StringBuilder();
            foreach (string argument in arguments)
            {
                if (sb.Length > 0)
                    sb.Append(' ');
                AppendQuotedIfNeeded(sb, argument ?? string.Empty);
            }

            return sb.ToString();
        }

        private static void AppendQuotedIfNeeded(StringBuilder sb, string argument)
        {
            bool needsQuotes = argument.Length == 0 || ContainsWhitespaceOrQuote(argument);
            if (!needsQuotes)
            {
                sb.Append(argument);
                return;
            }

            sb.Append('"');
            foreach (char c in argument)
            {
                if (c == '"')
                    sb.Append('\\');
                sb.Append(c);
            }

            sb.Append('"');
        }

        private static bool ContainsWhitespaceOrQuote(string value)
        {
            foreach (char c in value)
            {
                if (c == ' ' || c == '\t' || c == '"')
                    return true;
            }

            return false;
        }
    }
}
