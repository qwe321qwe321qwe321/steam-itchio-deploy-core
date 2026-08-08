using SteamItchIoDeployerCore;
using Xunit;

namespace SteamItchIoDeployerCore.Tests
{
    public class MacroResolverTests
    {
        [Fact]
        public void Resolve_SubstitutesAllMacros()
        {
            string result = MacroResolver.Resolve(
                "v{Version} - {Date} {DateTime} - {GitSHA}", "1.0.0", "2026-08-08", "2026-08-08 12:00:00", "abc123");

            Assert.Equal("v1.0.0 - 2026-08-08 2026-08-08 12:00:00 - abc123", result);
        }

        [Fact]
        public void Resolve_EmptyTemplate_ReturnsVersion()
        {
            Assert.Equal("1.0.0", MacroResolver.Resolve("", "1.0.0", "d", "dt", "sha"));
        }
    }

    public class CliArgumentQuotingTests
    {
        [Fact]
        public void Join_QuotesTokensContainingWhitespace()
        {
            string result = CliArgumentQuoting.Join(new[] { "+login", "user", "pass with space" });

            Assert.Equal("+login user \"pass with space\"", result);
        }

        [Fact]
        public void Join_EscapesInnerQuotes()
        {
            string result = CliArgumentQuoting.Join(new[] { "say \"hi\"" });

            Assert.Equal("\"say \\\"hi\\\"\"", result);
        }

        [Fact]
        public void Join_PlainTokens_NoQuoting()
        {
            string result = CliArgumentQuoting.Join(new[] { "+quit" });

            Assert.Equal("+quit", result);
        }
    }
}
