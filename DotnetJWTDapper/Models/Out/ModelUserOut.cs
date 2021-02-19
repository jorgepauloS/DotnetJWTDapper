using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetJWTDapper.Models
{
    public class ModelUserOut : ModelUser
    {
        [JsonIgnore]
        public override string Password { get => base.Password; set => base.Password = value; }
        [JsonIgnore]
        public override string Salt { get => base.Salt; set => base.Salt = value; }

        public ModelUserOut(ModelUser pUser)
        {
            Id = pUser.Id;
            Username = pUser.Username;
            Password = pUser.Password;
            Name = pUser.Name;
            Email = pUser.Email;
            Salt = pUser.Salt;
            Created = pUser.Created;
            Modified = pUser.Modified;
            Token = pUser.Token;
            RefreshToken = pUser.RefreshToken;
            TokenExpiration = pUser.TokenExpiration;
            RefreshTokenExpiration = pUser.RefreshTokenExpiration;
        }
    }
}
