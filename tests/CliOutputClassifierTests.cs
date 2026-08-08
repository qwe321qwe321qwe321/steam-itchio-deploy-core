using SteamItchIoDeployerCore;
using Xunit;

namespace SteamItchIoDeployerCore.Tests
{
    public class CliOutputClassifierTests
    {
        [Fact]
        public void Classify_SteamGuardLine_ReturnsSteamGuardRequired()
        {
            CliLogLevel level = CliOutputClassifier.Classify(
                CliToolKind.SteamCmd, "FAILED login with result code RequireTwoFactorCode", fromStdErr: false);

            Assert.Equal(CliLogLevel.SteamGuardRequired, level);
        }

        [Fact]
        public void Classify_SteamAuthFailure_ReturnsAuthFailure()
        {
            CliLogLevel level = CliOutputClassifier.Classify(
                CliToolKind.SteamCmd, "FAILED login with result code InvalidPassword", fromStdErr: false);

            Assert.Equal(CliLogLevel.AuthFailure, level);
        }

        [Fact]
        public void Classify_ButlerAuthFailure_ReturnsAuthFailure()
        {
            CliLogLevel level = CliOutputClassifier.Classify(
                CliToolKind.Butler, "error: invalid api key supplied", fromStdErr: false);

            Assert.Equal(CliLogLevel.AuthFailure, level);
        }

        [Fact]
        public void Classify_StdErrLine_ReturnsError()
        {
            CliLogLevel level = CliOutputClassifier.Classify(CliToolKind.Generic, "some line", fromStdErr: true);

            Assert.Equal(CliLogLevel.Error, level);
        }

        [Fact]
        public void Classify_PlainStdoutLine_ReturnsInfo()
        {
            CliLogLevel level = CliOutputClassifier.Classify(CliToolKind.Generic, "Uploading chunk 3/10", fromStdErr: false);

            Assert.Equal(CliLogLevel.Info, level);
        }

        [Fact]
        public void IsSteamGuardRequired_MatchesKnownPhrase()
        {
            Assert.True(CliOutputClassifier.IsSteamGuardRequired("Steam Guard code required"));
            Assert.False(CliOutputClassifier.IsSteamGuardRequired("Upload complete"));
        }
    }
}
