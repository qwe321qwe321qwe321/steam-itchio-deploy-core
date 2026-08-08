using System.Collections.Generic;

namespace SteamItchIoDeployerCore
{
    /// <summary>
    /// Builds steamcmd argument token arrays. Returned as token arrays (not a pre-joined string)
    /// so each host can hand them to whichever process API it uses (ArgumentList, or
    /// <see cref="CliArgumentQuoting"/> for hosts limited to a single Arguments string).
    /// </summary>
    public static class SteamCommandBuilder
    {
        public static string[] BuildLoginAndRunAppBuildArguments(
            string username, string password, string steamGuardCode, string appVdfPath)
        {
            var args = new List<string>();
            AppendGuardCode(args, steamGuardCode);
            args.Add("+login");
            args.Add(username ?? string.Empty);
            args.Add(password ?? string.Empty);
            args.Add("+run_app_build");
            args.Add(appVdfPath ?? string.Empty);
            args.Add("+quit");
            return args.ToArray();
        }

        public static string[] BuildTestLoginArguments(
            string username, string password, string steamGuardCode = "")
        {
            var args = new List<string>();
            AppendGuardCode(args, steamGuardCode);
            args.Add("+login");
            args.Add(username ?? string.Empty);
            args.Add(password ?? string.Empty);
            args.Add("+quit");
            return args.ToArray();
        }

        private static void AppendGuardCode(List<string> args, string steamGuardCode)
        {
            if (string.IsNullOrWhiteSpace(steamGuardCode))
                return;

            args.Add("+set_steam_guard_code");
            args.Add(steamGuardCode.Trim());
        }
    }
}
