using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetJWTDapper.Models
{
    public class ModelUser
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string Username { get; set; }
        public virtual string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public virtual string Salt { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpiration { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

        internal ModelUserOut ToOut()
        {
            return new ModelUserOut(this);
        }
    }
}
