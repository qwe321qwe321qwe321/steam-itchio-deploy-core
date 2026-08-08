using System;
using System.Text;

namespace SteamItchIoDeployerCore
{
    /// <summary>
    /// Renders the text content of steamcmd's app_build/depot_build VDF scripts.
    ///
    /// VDF FORMAT NOTES:
    ///   - VDF uses tab-indented key-value pairs enclosed in braces.
    ///   - String values are double-quoted.
    ///   - Backslashes in path strings must be doubled (\\) as VDF treats \ as an escape character.
    ///   - SteamCMD's VDF parser does NOT support Unicode; all paths must be ASCII-safe.
    ///
    /// File placement (steamcmd/scripts directory layout, ContentRoot resolution, etc.) is left to
    /// each host, since that differs by engine; this type only renders the two script bodies.
    /// </summary>
    public static class VdfContentBuilder
    {
        /// <param name="options">Depot-relevant fields (DepotID, IgnoreFiles).</param>
        public static string BuildDepotVdfContent(SteamVdfOptions options)
        {
            var sb = new StringBuilder();
            sb.AppendLine("\"DepotBuild\"");
            sb.AppendLine("{");
            sb.AppendLine($"\t\"DepotID\"\t\"{EscapeVdfValue(options.DepotId)}\"");
            sb.AppendLine("\t\"FileMapping\"");
            sb.AppendLine("\t{");
            // LocalPath "*" + Recursive "1" = upload every file in ContentRoot recursively.
            sb.AppendLine("\t\t\"LocalPath\"\t\"*\"");
            // DepotPath "." maps all local files to the root of the depot (preserving subdir structure).
            sb.AppendLine("\t\t\"DepotPath\"\t\".\"");
            sb.AppendLine("\t\t\"Recursive\"\t\"1\"");
            sb.AppendLine("\t}");

            foreach (string pattern in SplitIgnorePatterns(options.IgnoreFiles))
            {
                sb.AppendLine($"\t\"FileExclusion\"\t\"{EscapeVdfValue(pattern)}\"");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <param name="options">AppID/branch fields.</param>
        /// <param name="contentRootAbsolutePath">Absolute path to the directory whose contents are uploaded (the build output).</param>
        /// <param name="buildOutputLogDirAbsolutePath">Absolute path to the directory steamcmd writes its own build log chunks to.</param>
        /// <param name="resolvedDescription">Build description with macros already substituted.</param>
        /// <param name="depotVdfAbsolutePath">Absolute path to the sibling depot_build VDF file written by <see cref="BuildDepotVdfContent"/>.</param>
        public static string BuildAppVdfContent(
            SteamVdfOptions options,
            string contentRootAbsolutePath,
            string buildOutputLogDirAbsolutePath,
            string resolvedDescription,
            string depotVdfAbsolutePath)
        {
            string contentRoot = EscapePathForVdf(contentRootAbsolutePath);
            string buildOutput = EscapePathForVdf(buildOutputLogDirAbsolutePath);
            string depotScript = EscapePathForVdf(depotVdfAbsolutePath);
            string setLiveBranch = options.SetLiveEnabled ? options.Branch : string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("\"AppBuild\"");
            sb.AppendLine("{");
            sb.AppendLine($"\t\"AppID\"\t\t\"{EscapeVdfValue(options.AppId)}\"");
            sb.AppendLine($"\t\"Desc\"\t\t\"{EscapeVdfValue(resolvedDescription)}\"");
            // Silent=0 shows progress; Preview=0 means this is a real upload, not a dry-run.
            sb.AppendLine("\t\"Silent\"\t\"0\"");
            sb.AppendLine("\t\"Preview\"\t\"0\"");
            sb.AppendLine($"\t\"ContentRoot\"\t\"{contentRoot}\"");
            sb.AppendLine($"\t\"BuildOutput\"\t\"{buildOutput}\"");
            // SetLive: the branch to promote the build to after upload. Empty string = no promotion.
            sb.AppendLine($"\t\"SetLive\"\t\"{EscapeVdfValue(setLiveBranch)}\"");
            sb.AppendLine("\t\"Depots\"");
            sb.AppendLine("\t{");
            sb.AppendLine($"\t\t\"{EscapeVdfValue(options.DepotId)}\"\t\"{depotScript}\"");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string[] SplitIgnorePatterns(string commaSeparated)
        {
            if (string.IsNullOrWhiteSpace(commaSeparated))
                return Array.Empty<string>();

            string[] raw = commaSeparated.Split(',');
            var result = new System.Collections.Generic.List<string>(raw.Length);
            foreach (string entry in raw)
            {
                string pattern = entry.Trim();
                if (pattern.Length > 0)
                    result.Add(pattern);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Escapes an absolute path for use as a VDF string value. steamcmd runs on the machine
        /// invoking it, so the relevant platform is the current OS, not the game's build target.
        /// </summary>
        public static string EscapePathForVdf(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return absolutePath ?? string.Empty;

            // Checked via the path separator rather than System.OperatingSystem.IsWindows(): that
            // API is .NET 5+ only and this file is compiled directly (as source) by Unity's older
            // API compatibility profiles too. steamcmd runs on the machine invoking it, so the
            // relevant platform is the current OS, not the game's build target.
            if (System.IO.Path.DirectorySeparatorChar == '\\')
            {
                // VDF treats backslash as an escape character, so normalize to backslashes
                // then double-escape each one.
                return absolutePath.Replace("/", "\\").Replace("\\", "\\\\");
            }

            // SteamCMD accepts forward slashes on macOS/Linux; no escaping needed.
            return absolutePath.Replace("\\", "/");
        }

        /// <summary>
        /// Escapes special characters within a generic VDF string value (non-path).
        /// Only double-quotes need escaping in VDF values; backslash is only special in paths.
        /// </summary>
        public static string EscapeVdfValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value ?? string.Empty;

            return value.Replace("\"", "\\\"");
        }
    }
}
