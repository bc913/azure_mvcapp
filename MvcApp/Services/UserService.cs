using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using MvcApp.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace MvcApp.Services
{

    public static class UserServiceExtensions
    {
        public static void AddUserService(this IServiceCollection services, IConfiguration configuration)
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient<IUserService, UserService>();
        }
    }
    
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpClient _httpClient;
        private readonly string _scope = string.Empty;
        private readonly string _baseAddress = string.Empty;
        private readonly ITokenAcquisition _tokenAcquisition;

        public UserService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, 
        IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _contextAccessor = contextAccessor;
            _scope = configuration["WebApi:Scope"];
            _baseAddress = configuration["WebApi:BaseAddress"];
        }

        public async Task<IEnumerable<UserViewModel>> GetAsync()
        {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync($"{ _baseAddress}/api/users");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                IEnumerable<UserViewModel> users = JsonConvert.DeserializeObject<IEnumerable<UserViewModel>>(content);
                return users;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { _scope });
            Debug.WriteLine($"access token-{accessToken}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}