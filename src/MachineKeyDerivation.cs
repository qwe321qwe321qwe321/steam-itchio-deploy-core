using System;
using System.Security.Cryptography;
using System.Text;

namespace SteamItchIoDeployerCore
{
    /// <summary>
    /// Small hashing helpers used to derive machine-bound key material for encrypting stored
    /// credentials. Callers compose their own material string (typically a device/OS unique id
    /// plus a project path and/or a fixed salt) — this type only wraps the hash algorithms so both
    /// engines don't each hand-roll the same SHA-256/MD5 calls.
    /// </summary>
    public static class MachineKeyDerivation
    {
        /// <summary>32 bytes — suitable as an AES-256 key.</summary>
        public static byte[] Sha256(string material)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(material ?? string.Empty));
            }
        }

        /// <summary>16 bytes — suitable as an AES block-size IV. Not used for its collision resistance.</summary>
        public static byte[] Md5(string material)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                return md5.ComputeHash(Encoding.UTF8.GetBytes(material ?? string.Empty));
            }
        }

        public static string Sha256Hex(string material)
        {
            byte[] hash = Sha256(material);
            // Manual hex formatting instead of Convert.ToHexString: that API is .NET 5+ only and
            // this file is compiled directly (as source) by Unity's older API compatibility profiles too.
            var sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
