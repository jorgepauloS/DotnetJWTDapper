using DotnetJWTDapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetJWTDapper.Repository
{
    public interface IUserRepository
    {
        string Add(ModelUser pUser);
        ModelUserOut GetUser(string pIdUser);
        ModelUserOut Login(string pEmail, string pPassword);
        IEnumerable<ModelUserOut> GetUsers();
        ModelUserOut GetUserByToken(string pToken);
        ModelUserOut GetUserByRefreshToken(string pRefreshToken);
        ModelUserOut RefreshToken(ModelUser pUser);
        bool UpdateUser(ModelUser pUser);
        bool DeleteUser(string pUserId);
    }
}
