using System.Collections.Generic;

namespace SteamItchIoDeployerCore
{
    /// <summary>Builds butler <c>push</c> argument token arrays.</summary>
    public static class ButlerCommandBuilder
    {
        public static string[] BuildPushArguments(
            string buildOutputPath,
            string target,
            string channel,
            string userVersion,
            bool ifChanged,
            IEnumerable<string> ignorePatterns)
        {
            var args = new List<string>
            {
                "push",
                buildOutputPath ?? string.Empty,
                $"{target}:{channel}",
            };

            if (!string.IsNullOrWhiteSpace(userVersion))
            {
                args.Add("--userversion");
                args.Add(userVersion);
            }

            if (ifChanged)
                args.Add("--if-changed");

            if (ignorePatterns != null)
            {
                foreach (string pattern in ignorePatterns)
                {
                    if (!string.IsNullOrWhiteSpace(pattern))
                    {
                        args.Add("--ignore");
                        args.Add(pattern.Trim());
                    }
                }
            }

            return args.ToArray();
        }
    }
}
