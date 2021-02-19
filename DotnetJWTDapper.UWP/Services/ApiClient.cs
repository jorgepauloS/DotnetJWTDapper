using DotnetJWTDapper.UWP.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DotnetJWTDapper.UWP.Services
{
    public class ApiClient : HttpClient
    {
        public ApiClient()
        {
            BaseAddress = new Uri("http://localhost/SEChallengeAPI/");
        }

        private async Task<string> GetBearer(string pToken, string pRefreshToken)
        {
            ModelBearer model = new ModelBearer() { Token = pToken, RefreshToken = pRefreshToken };

            var bearer = await this.PostAsJsonAsync<ModelBearer>("api/auth/token", model);
            if (bearer.IsSuccessStatusCode)
            {
                var jsonString = await bearer.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ModelBearer>(jsonString).Bearer;
            }
            else
            {
                var refreshtoken = await this.PostAsJsonAsync<ModelBearer>("api/auth/refreshtoken", model);
                if (refreshtoken.IsSuccessStatusCode)
                {
                    var jsonString = await refreshtoken.Content.ReadAsStringAsync();
                    ModelBearer modelbearer = JsonConvert.DeserializeObject<ModelBearer>(jsonString);
                    App.UserLogged.Token = modelbearer.Token;
                    App.UserLogged.TokenExpiration = modelbearer.TokenExpiration;
                    return modelbearer.Bearer;
                }
                else
                {
                    throw new Exception("You need to signin again!");
                }
            }
        }

        public async Task<ModelUser> GetUser(ModelUser pUserLogged, string pIdUser)
        {
            string bearer = await GetBearer(pUserLogged.Token, pUserLogged.RefreshToken);
            this.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer);

            var user = await this.GetAsync($"api/users/{pIdUser}");
            if (user.IsSuccessStatusCode)
            {
                var jsonString = await user.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ModelUser>(jsonString);
            }
            else
            {
                throw new Exception(await user.Content.ReadAsStringAsync());
            }
        }

        public async Task<ModelUser> SignUp(ModelUser pUser)
        {
            var user = await this.PostAsJsonAsync<ModelUser>("api/users/signup", pUser);
            if (user.IsSuccessStatusCode)
            {
                var jsonString = await user.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ModelUser>(jsonString);
            }
            else
            {
                throw new Exception(await user.Content.ReadAsStringAsync());
            }
        }

        public async Task<ModelUser> Login(string pEmail, string pPassword)
        {
            ModelUser modelUser = new ModelUser() { Email = pEmail, Password = pPassword };
            var user = await this.PostAsJsonAsync<ModelUser>("api/users/login", modelUser);
            if (user.IsSuccessStatusCode)
            {
                var jsonString = await user.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ModelUser>(jsonString);
            }
            else
            {
                throw new Exception(await user.Content.ReadAsStringAsync());
            }
        }

        public async Task<bool> UpdateUser(ModelUser pUserLogged, ModelUser pUserUpdated)
        {
            string bearer = await GetBearer(pUserLogged.Token, pUserLogged.RefreshToken);
            this.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer);

            var user = await this.PutAsJsonAsync($"api/users/update", pUserUpdated);
            if (user.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new Exception(await user.Content.ReadAsStringAsync());
            }
        }

        public async Task<bool> DeleteUser(ModelUser pUserLogged, string pIdUser)
        {
            string bearer = await GetBearer(pUserLogged.Token, pUserLogged.RefreshToken);
            this.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer);

            var user = await this.DeleteAsync($"api/users/delete/{pIdUser}");
            if (user.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new Exception(await user.Content.ReadAsStringAsync());
            }
        }

        public async Task<List<ModelUser>> GetUsers(ModelUser pUser)
        {
            string bearer = await GetBearer(pUser.Token, pUser.RefreshToken);
            this.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer);

            var users = await this.GetAsync("api/users");
            if (users.IsSuccessStatusCode)
            {
                var jsonString = await users.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ModelUser>>(jsonString);
            }
            else
            {
                throw new Exception(await users.Content.ReadAsStringAsync());
            }
        }

    }
}
