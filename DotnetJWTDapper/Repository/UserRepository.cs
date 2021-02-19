using Dapper;
using DotnetJWTDapper.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DotnetJWTDapper.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration) { }

        public string Add(ModelUser pUser)
        {
            var connectionString = this.GetConnection();
            ModelUser user = null;
            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = $"SELECT * FROM USERS WHERE Email = '{pUser.Email}'";
                    user = con.Query<ModelUser>(query).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }

                if (user is object)
                    throw new Exception("Email already registered");
            }

            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = $"SELECT * FROM USERS WHERE Username = '{pUser.Username}'";
                    user = con.Query<ModelUser>(query).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }

                if (user is object)
                    throw new Exception("Username already registered");
            }

            pUser.Id = Guid.NewGuid().ToString();

            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            pUser.Salt = Convert.ToBase64String(salt);

            //Hash Password
            pUser.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: pUser.Password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 100,
            numBytesRequested: 256 / 8));

            //RefreshToken and RefreshToken Expiration 
            using (SHA256 mySHA256 = SHA256.Create())
            {
                DateTime now = DateTime.UtcNow;
                byte[] ticks = Encoding.UTF8.GetBytes(now.Ticks.ToString());

                List<byte> bytesSalt = Convert.FromBase64String(pUser.Salt).ToList();
                ticks.ToList().ForEach(m => bytesSalt.Add(m));

                byte[] hash = mySHA256.ComputeHash(bytesSalt.ToArray());
                pUser.RefreshToken = Convert.ToBase64String(hash);
                pUser.RefreshTokenExpiration = now.AddYears(1);
            }

            //Token and Token Expiration 
            using (SHA256 mySHA256 = SHA256.Create())
            {
                DateTime now = DateTime.UtcNow;
                byte[] ticks = Encoding.UTF8.GetBytes(now.Ticks.ToString());

                List<byte> bytesSalt = Convert.FromBase64String(pUser.Salt).ToList();
                ticks.ToList().ForEach(m => bytesSalt.Add(m));

                byte[] hash = mySHA256.ComputeHash(bytesSalt.ToArray());
                pUser.Token = Convert.ToBase64String(hash);
                pUser.TokenExpiration = now.AddDays(1);
            }

            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query =
                        "INSERT INTO USERS (Id, Username, Password, Name, Email, Salt, Token, " +
                        "RefreshToken, TokenExpiration, RefreshTokenExpiration) " +
                        "VALUES" +
                        "(@Id, @Username, @Password, @Name, @Email, @Salt, @Token, " +
                        "@RefreshToken, @TokenExpiration, @RefreshTokenExpiration) ";

                    int count = con.Execute(query, pUser);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }

                return pUser.Id;
            }
        }

        public ModelUserOut GetUser(string pIdUser)
        {
            var connectionString = this.GetConnection();
            ModelUser user = null;
            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = $"SELECT * FROM USERS WHERE Id = '{pIdUser}'";
                    user = con.Query<ModelUser>(query).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }

                if (user is object)
                    return user.ToOut();
                else
                    return null;
            }
        }

        public ModelUserOut GetUserByRefreshToken(string pRefreshToken)
        {
            var connectionString = this.GetConnection();
            ModelUser user = null;
            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = $"SELECT * FROM USERS WHERE RefreshToken = '{pRefreshToken}'";
                    user = con.Query<ModelUser>(query).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
                if (user is object)
                    return user.ToOut();
                else
                    return null;
            }
        }

        public ModelUserOut GetUserByToken(string pToken)
        {
            var connectionString = this.GetConnection();
            ModelUser user = null;
            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = $"SELECT * FROM USERS WHERE Token = '{pToken}'";
                    user = con.Query<ModelUser>(query).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }

                if (user is object)
                    return user.ToOut();
                else
                    return null;
            }
        }

        public IEnumerable<ModelUserOut> GetUsers()
        {
            var connectionString = this.GetConnection();
            List<ModelUser> users;
            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = "SELECT * FROM USERS";
                    users = con.Query<ModelUser>(query).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }

            IEnumerable<ModelUserOut> usersOut = new List<ModelUserOut>();
            users.ForEach(m => usersOut = usersOut.Append(m.ToOut()));

            return usersOut;
        }

        public ModelUserOut Login(string pEmail, string pPassword)
        {
            if (string.IsNullOrEmpty(pEmail))
                throw new Exception("Invalid Email");
            if (string.IsNullOrEmpty(pPassword))
                throw new Exception("Invalid Password");

            ModelUser user = null;

            var connectionString = this.GetConnection();
            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    var query = $"SELECT * FROM USERS WHERE Email = '{pEmail}'";
                    user = con.Query<ModelUser>(query).FirstOrDefault();

                    if (user is object)
                    {
                        string pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: pPassword,
                            salt: Convert.FromBase64String(user.Salt),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 100,
                            numBytesRequested: 256 / 8));

                        if (pass.Equals(user.Password))
                        {
                            //RefreshToken and RefreshToken Expiration 
                            if (user.RefreshTokenExpiration < DateTime.UtcNow)
                            {
                                using (SHA256 mySHA256 = SHA256.Create())
                                {
                                    DateTime now = DateTime.UtcNow;
                                    byte[] ticks = Encoding.UTF8.GetBytes(now.Ticks.ToString());

                                    List<byte> bytesSalt = Convert.FromBase64String(user.Salt).ToList();
                                    ticks.ToList().ForEach(m => bytesSalt.Add(m));

                                    byte[] hash = mySHA256.ComputeHash(bytesSalt.ToArray());
                                    user.RefreshToken = Convert.ToBase64String(hash);
                                    user.RefreshTokenExpiration = now.AddYears(1);
                                }
                            }

                            //Token and Token Expiration 
                            if (user.TokenExpiration < DateTime.UtcNow)
                            {
                                using (SHA256 mySHA256 = SHA256.Create())
                                {
                                    DateTime now = DateTime.UtcNow;
                                    byte[] ticks = Encoding.UTF8.GetBytes(now.Ticks.ToString());

                                    List<byte> bytesSalt = Convert.FromBase64String(user.Salt).ToList();
                                    ticks.ToList().ForEach(m => bytesSalt.Add(m));

                                    byte[] hash = mySHA256.ComputeHash(bytesSalt.ToArray());
                                    user.Token = Convert.ToBase64String(hash);
                                    user.TokenExpiration = now.AddDays(1);
                                }
                            }

                            string queryToken =
                                "UPDATE USERS SET Token = @Token, RefreshToken = @RefreshToken," +
                                " TokenExpiration = @TokenExpiration, RefreshTokenExpiration = @RefreshTokenExpiration" +
                                " WHERE Id = @Id";

                            con.Execute(queryToken, user);

                            if (user is object)
                                return user.ToOut();
                            else
                                return null;
                        }
                        else
                            throw new Exception("Invalid Password");
                    }
                    else
                    {
                        throw new Exception("Invalid Email");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public ModelUserOut RefreshToken(ModelUser pUser)
        {
            if (pUser.TokenExpiration < DateTime.UtcNow)
            {
                using (SHA256 mySHA256 = SHA256.Create())
                {
                    DateTime now = DateTime.UtcNow;
                    byte[] ticks = Encoding.UTF8.GetBytes(now.Ticks.ToString());

                    List<byte> bytesSalt = Convert.FromBase64String(pUser.Salt).ToList();
                    ticks.ToList().ForEach(m => bytesSalt.Add(m));

                    byte[] hash = mySHA256.ComputeHash(bytesSalt.ToArray());
                    pUser.Token = Convert.ToBase64String(hash);

                    if ((pUser.RefreshTokenExpiration - now) > TimeSpan.FromDays(1))
                        pUser.TokenExpiration = now.AddDays(1);
                    else
                        pUser.TokenExpiration = pUser.RefreshTokenExpiration;
                }
            }

            var connectionString = this.GetConnection();

            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "UPDATE USERS SET Token = @Token, TokenExpiration = @TokenExpiration WHERE Id = @Id";

                    int count = con.Execute(query, pUser);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }

                if (pUser is object)
                    return pUser.ToOut();
                else
                    return null;
            }
        }

        public bool DeleteUser(string pUserId)
        {
            var connectionString = this.GetConnection();

            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = $"DELETE FROM USERS WHERE Id = '{pUserId}'";

                    int count = con.Execute(query);
                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public bool UpdateUser(ModelUser pUser)
        {
            pUser.Modified = DateTime.UtcNow;

            ModelUser baseUser = GetUser(pUser.Id);

            pUser.Token = baseUser.Token;
            pUser.TokenExpiration = baseUser.TokenExpiration;
            pUser.RefreshToken = baseUser.RefreshToken;
            pUser.RefreshTokenExpiration = baseUser.RefreshTokenExpiration;

            if (!string.IsNullOrEmpty(pUser.Password))
            {
                //Hash Password
                pUser.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: pUser.Password,
                salt: Convert.FromBase64String(baseUser.Salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 100,
                numBytesRequested: 256 / 8));
            }

            string password = string.IsNullOrEmpty(pUser.Password) ? string.Empty : "Password = @Password, ";

            //RefreshToken and RefreshToken Expiration 
            if (baseUser.RefreshTokenExpiration < DateTime.UtcNow)
            {
                using (SHA256 mySHA256 = SHA256.Create())
                {
                    DateTime now = DateTime.UtcNow;
                    byte[] ticks = Encoding.UTF8.GetBytes(now.Ticks.ToString());

                    List<byte> bytesSalt = Convert.FromBase64String(baseUser.Salt).ToList();
                    ticks.ToList().ForEach(m => bytesSalt.Add(m));

                    byte[] hash = mySHA256.ComputeHash(bytesSalt.ToArray());
                    pUser.RefreshToken = Convert.ToBase64String(hash);
                    pUser.RefreshTokenExpiration = now.AddYears(1);
                }
            }

            //Token and Token Expiration 
            if (baseUser.TokenExpiration < DateTime.UtcNow)
            {
                using (SHA256 mySHA256 = SHA256.Create())
                {
                    DateTime now = DateTime.UtcNow;
                    byte[] ticks = Encoding.UTF8.GetBytes(now.Ticks.ToString());

                    List<byte> bytesSalt = Convert.FromBase64String(baseUser.Salt).ToList();
                    ticks.ToList().ForEach(m => bytesSalt.Add(m));

                    byte[] hash = mySHA256.ComputeHash(bytesSalt.ToArray());
                    pUser.Token = Convert.ToBase64String(hash);
                    pUser.TokenExpiration = now.AddDays(1);
                }
            }

            var connectionString = this.GetConnection();

            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "UPDATE USERS SET " +
                        " Username = @Username, Name = @Name, Email = @Email," +
                        password +
                        " RefreshToken = @RefreshToken, RefreshTokenExpiration = @RefreshTokenExpiration," +
                        " Token = @Token, TokenExpiration = @TokenExpiration," +
                        " Modified = @Modified" +
                        " WHERE Id = @Id";

                    int count = con.Execute(query, pUser);
                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}
