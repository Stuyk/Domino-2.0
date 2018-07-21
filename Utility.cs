using LiteDB;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Domino
{
    internal class Utility
    {
        /// <summary>
        /// Returns a SHA256 hash from supplied arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string ComputeHash(params object[] args)
        {
            var ctrHash = string.Empty;
            foreach (var arg in args)
            {
                if (arg == null || (arg is string && string.IsNullOrWhiteSpace(arg.ToString()))) continue;
                ctrHash += arg;
            }

            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(ctrHash));
                var strbuilder = new StringBuilder();
                for (var i = 0; i < bytes.Length; i++) strbuilder.Append(bytes[i].ToString("x2"));
                return strbuilder.ToString();
            }
        }
    }
}
