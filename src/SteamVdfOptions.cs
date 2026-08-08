namespace SteamItchIoDeployerCore
{
    /// <summary>Plain data needed to render steamcmd's app_build/depot_build VDF scripts.</summary>
    public sealed class SteamVdfOptions
    {
        public string AppId { get; set; } = string.Empty;
        public string DepotId { get; set; } = string.Empty;

        /// <summary>When false, "SetLive" is written as an empty string (no branch promotion).</summary>
        public bool SetLiveEnabled { get; set; }
        public string Branch { get; set; } = "default";

        /// <summary>Comma-separated glob patterns, e.g. "*.pdb, _BurstDebugInformation_DoNotShip".</summary>
        public string IgnoreFiles { get; set; } = string.Empty;
    }
}
