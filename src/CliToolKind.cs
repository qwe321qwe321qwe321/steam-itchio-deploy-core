namespace SteamItchIoDeployerCore
{
    /// <summary>Identifies which CLI tool produced a line of output, for classification purposes.</summary>
    public enum CliToolKind
    {
        Generic,
        SteamCmd,
        Butler,
    }
}
