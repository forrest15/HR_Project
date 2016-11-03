using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;

namespace Domain.Entities
{
    public class Password
    {
        private static int saltSize = 32;
        private string _encryptedPassword { get; set; }
        private string _salt { get; set; }

        [NotMapped]
        public string Value
        {
            get
            {
                return _encryptedPassword;
            }
            set
            {
                _salt = CreateSalt();
                _encryptedPassword = CreatePasswordHash(value, _salt);
            }
        }

        [NotMapped]
        public string Salt
        {
            get
            {
                return _salt;
            }
        }

        public override bool Equals(object obj)
        {
            var pass = obj as Password;
            if (pass == null)
            {
                return false;
            }

            return Value == pass.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator Password(string password)
        {
            return new Password
            {
                Value = password
            };
        }

        public static implicit operator string(Password password)
        {
            return password.Value;
        }

        public static bool operator ==(Password thisPassword, Password comparedPassword)
        {
            return thisPassword.Equals(comparedPassword);
        }

        public static bool operator !=(Password thisPassword, Password comparedPassword)
        {
            return !thisPassword.Equals(comparedPassword);
        }


        private static string CreateSalt()
        {
            return CreateSalt(saltSize);
        }

        private static string CreateSalt(int size)
        {
            byte[] buff = new byte[size];
            new RNGCryptoServiceProvider().GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        private static string CreatePasswordHash(string pwd, string salt)
        {
            var pwdBytes = GetBytes(pwd);
            var saltBytes = GetBytes(salt);
            var hashedPassword = CreatePasswordHash(pwdBytes, saltBytes);
            return Convert.ToBase64String(hashedPassword);
        }

        private static byte[] CreatePasswordHash(byte[] pwd, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] fullText = pwd.Union(salt).ToArray();
            return algorithm.ComputeHash(fullText);
        }

        static byte[] GetBytes(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
    }
}
