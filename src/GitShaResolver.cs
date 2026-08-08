using System;
using System.Diagnostics;

namespace SteamItchIoDeployerCore
{
    /// <summary>Resolves the current commit SHA via <c>git rev-parse HEAD</c> for the <c>{GitSHA}</c> macro.</summary>
    public static class GitShaResolver
    {
        public const string NoShaValue = "NO_SHA";

        public static string Resolve(string workingDirectory, int timeoutMilliseconds = 3000)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "rev-parse HEAD",
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                        return NoShaValue;

                    if (!process.WaitForExit(timeoutMilliseconds))
                    {
                        TryKill(process);
                        return NoShaValue;
                    }

                    string output = process.StandardOutput.ReadToEnd().Trim();
                    return process.ExitCode == 0 && output.Length > 0 ? output : NoShaValue;
                }
            }
            catch
            {
                return NoShaValue;
            }
        }

        private static void TryKill(Process process)
        {
            try
            {
                // Plain Kill(), not the entireProcessTree overload: that overload is .NET Core 3.0+
                // only and this file is compiled directly (as source) by Unity's older API
                // compatibility profiles too. git rev-parse does not spawn child processes.
                process.Kill();
            }
            catch
            {
                // Best-effort; the process may have exited between the timeout check and here.
            }
        }
    }
}
