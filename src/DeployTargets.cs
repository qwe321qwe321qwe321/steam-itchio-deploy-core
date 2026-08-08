using System;

namespace SteamItchIoDeployerCore
{
    /// <summary>
    /// Upload targets shared by every engine-specific deployer. A build can be pushed to
    /// any combination of these in a single run.
    /// </summary>
    [Flags]
    public enum DeployTargets
    {
        None = 0,
        Steam = 1 << 0,
        ItchIo = 1 << 1,
    }
}
