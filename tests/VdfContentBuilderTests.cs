using SteamItchIoDeployerCore;
using Xunit;

namespace SteamItchIoDeployerCore.Tests
{
    public class VdfContentBuilderTests
    {
        [Fact]
        public void BuildDepotVdfContent_IncludesDepotIdAndExclusions()
        {
            var options = new SteamVdfOptions
            {
                DepotId = "12345",
                IgnoreFiles = "*.pdb, _BurstDebugInformation_DoNotShip",
            };

            string content = VdfContentBuilder.BuildDepotVdfContent(options);

            Assert.Contains("\"DepotID\"\t\"12345\"", content);
            Assert.Contains("\"FileExclusion\"\t\"*.pdb\"", content);
            Assert.Contains("\"FileExclusion\"\t\"_BurstDebugInformation_DoNotShip\"", content);
        }

        [Fact]
        public void BuildAppVdfContent_SetLiveDisabled_WritesEmptyBranch()
        {
            var options = new SteamVdfOptions
            {
                AppId = "999",
                DepotId = "1000",
                SetLiveEnabled = false,
                Branch = "beta",
            };

            string content = VdfContentBuilder.BuildAppVdfContent(options, "/build", "/logs", "desc", "/depot.vdf");

            Assert.Contains("\"SetLive\"\t\"\"", content);
        }

        [Fact]
        public void BuildAppVdfContent_SetLiveEnabled_WritesBranch()
        {
            var options = new SteamVdfOptions
            {
                AppId = "999",
                DepotId = "1000",
                SetLiveEnabled = true,
                Branch = "beta",
            };

            string content = VdfContentBuilder.BuildAppVdfContent(options, "/build", "/logs", "desc", "/depot.vdf");

            Assert.Contains("\"SetLive\"\t\"beta\"", content);
        }

        [Fact]
        public void SplitIgnorePatterns_TrimsAndDropsEmptyEntries()
        {
            string[] patterns = VdfContentBuilder.SplitIgnorePatterns(" *.pdb ,, foo.log ,");

            Assert.Equal(new[] { "*.pdb", "foo.log" }, patterns);
        }

        [Fact]
        public void EscapeVdfValue_EscapesDoubleQuotes()
        {
            Assert.Equal("say \\\"hi\\\"", VdfContentBuilder.EscapeVdfValue("say \"hi\""));
        }
    }
}
