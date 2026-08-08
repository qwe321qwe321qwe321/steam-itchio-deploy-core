using System;
using System.Text;
using SteamItchIoDeployerCore;
using Xunit;

namespace SteamItchIoDeployerCore.Tests
{
    public class MachineKeyDerivationTests
    {
        [Fact]
        public void Sha256_Is32Bytes()
        {
            Assert.Equal(32, MachineKeyDerivation.Sha256("material").Length);
        }

        [Fact]
        public void Md5_Is16Bytes()
        {
            Assert.Equal(16, MachineKeyDerivation.Md5("material").Length);
        }

        [Fact]
        public void Sha256Hex_MatchesConvertToHexStringCasing()
        {
            // credentials.cfg files saved by the Godot host before this hashing helper was
            // extracted here were encrypted with Convert.ToHexString's (uppercase) output as the
            // passphrase; a different casing here would silently make those files undecryptable.
            byte[] hash = MachineKeyDerivation.Sha256("some-material");
            Assert.Equal(Convert.ToHexString(hash), MachineKeyDerivation.Sha256Hex("some-material"));
        }
    }
}
