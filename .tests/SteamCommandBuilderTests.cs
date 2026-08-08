using SteamItchIoDeployerCore;
using Xunit;

namespace SteamItchIoDeployerCore.Tests
{
    public class SteamCommandBuilderTests
    {
        [Fact]
        public void BuildLoginAndRunAppBuildArguments_WithoutGuardCode_OmitsGuardTokens()
        {
            string[] args = SteamCommandBuilder.BuildLoginAndRunAppBuildArguments("user", "pass", "", "/app.vdf");

            Assert.Equal(new[] { "+login", "user", "pass", "+run_app_build", "/app.vdf", "+quit" }, args);
        }

        [Fact]
        public void BuildLoginAndRunAppBuildArguments_WithGuardCode_PrependsGuardTokens()
        {
            string[] args = SteamCommandBuilder.BuildLoginAndRunAppBuildArguments("user", "pass", " 12345 ", "/app.vdf");

            Assert.Equal(
                new[] { "+set_steam_guard_code", "12345", "+login", "user", "pass", "+run_app_build", "/app.vdf", "+quit" },
                args);
        }

        [Fact]
        public void BuildTestLoginArguments_DefaultGuardCode_OmitsGuardTokens()
        {
            string[] args = SteamCommandBuilder.BuildTestLoginArguments("user", "pass");

            Assert.Equal(new[] { "+login", "user", "pass", "+quit" }, args);
        }

        [Theory]
        [InlineData(0, "Success.")]
        [InlineData(6, "Build commit failed. Content was uploaded but could not be finalised. Common causes: SetLive branch not eligible yet, invalid branch name, or a transient Valve-side error.")]
        [InlineData(42, "Rate limit exceeded (Valve-side throttle). Wait several minutes before retrying.")]
        public void SteamExitCodeDescriptions_KnownCodes(int exitCode, string expected)
        {
            Assert.Equal(expected, SteamExitCodeDescriptions.Describe(exitCode));
        }

        [Fact]
        public void SteamExitCodeDescriptions_UnknownCode_MentionsCode()
        {
            Assert.Contains("1234", SteamExitCodeDescriptions.Describe(1234));
        }
    }
}
