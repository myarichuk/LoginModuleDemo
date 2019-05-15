// ReSharper disable UnusedMember.Global
namespace LoginModule.Model
{
    public class User
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public bool IsValidated { get; set; }

        public string HashedPassword { get; set; }
    }
}

