// ReSharper disable UnusedMember.Global

using System;

namespace LoginModule.Model
{
    public class User
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public DateTime RegisteredAt { get; set; }

        #region Security Properties
        public string HashedPassword { get; set; }

        public byte[] Salt { get; set; }
        #endregion
    }
}

