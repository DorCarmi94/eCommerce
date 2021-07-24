using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce.Auth
{
    public class AuthUser
    {
        
        private string _username;
        private byte[] _hashedPassword;

        public AuthUser()
        {
        }
        
        public AuthUser(string username, byte[] hashedPassword)
        {
            _username = username;
            _hashedPassword = hashedPassword;
        }

        // ========== Properties ========== //

        [Key]
        public string Username
        {
            get => _username;
            set => _username = value;
        }
        
        public byte[] Password
        {
            get => _hashedPassword;
            set => _hashedPassword = value;
        }

        [NotMapped]
        public byte[] HashedPassword
        {
            get => _hashedPassword; 
        }
    }
}