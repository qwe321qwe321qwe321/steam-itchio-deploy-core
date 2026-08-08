using SteamItchIoDeployerCore;
using Xunit;

namespace SteamItchIoDeployerCore.Tests
{
    public class ButlerCommandBuilderTests
    {
        [Fact]
        public void BuildPushArguments_MinimalOptions()
        {
            string[] args = ButlerCommandBuilder.BuildPushArguments(
                "/build", "user/game", "windows", userVersion: "", ifChanged: false, ignorePatterns: null);

            Assert.Equal(new[] { "push", "/build", "user/game:windows" }, args);
        }

        [Fact]
        public void BuildPushArguments_AllOptions()
        {
            string[] args = ButlerCommandBuilder.BuildPushArguments(
                "/build",
                "user/game",
                "windows",
                userVersion: "1.2.3",
                ifChanged: true,
                ignorePatterns: new[] { "*.pdb", " foo.log " });

            Assert.Equal(
                new[]
                {
                    "push", "/build", "user/game:windows",
                    "--userversion", "1.2.3",
                    "--if-changed",
                    "--ignore", "*.pdb",
                    "--ignore", "foo.log",
                },
                args);
        }
    }
}
