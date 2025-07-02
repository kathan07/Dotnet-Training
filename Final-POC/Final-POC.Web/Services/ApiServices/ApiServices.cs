using Flurl.Http;
using Flurl;
using Final_POC.Core.DTOs;

namespace Final_POC.Web.Services.ApiServices
{
    public class ApiServices: IApiServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _apiBaseUrl;
        private readonly string _accessToken;

        public ApiServices(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"]!;
            _accessToken = configuration["ApiSettings:AccessToken"]!;
        }

        private string? GetToken()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies[_accessToken];
        }

        private void SetToken(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            var context = _httpContextAccessor.HttpContext;
            context?.Response.Cookies.Delete(_accessToken);
            context?.Response.Cookies.Append(_accessToken, token, cookieOptions);
        }

        private IFlurlRequest ConfigureRequest(string endpoint)
        {
            var request = _apiBaseUrl
                .AppendPathSegment(endpoint)
                .WithHeader("Accept", "application/json");

            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                request = request.WithOAuthBearerToken(token);
            }

            return request;
        }

        public async Task<bool> LoginAsync(LoginUserDto user)
        {
            try
            {
                var response = await _apiBaseUrl
                    .AppendPathSegment("auth/login")
                    .PostJsonAsync(user)
                    .ReceiveJson<ApiResponseDto<LoginReponseDto>>();

                if (response.Success)
                {
                    SetToken(response.Data.Token);
                    return true;
                }

                return false;
            }
            catch (FlurlHttpException)
            {
                return false;
            }
        }

        public async Task<T> GetAsync<T>(string endpoint, object? queryParams = null)
        {
            try
            {
                var request = ConfigureRequest(endpoint);

                if (queryParams != null)
                {
                    request = request.SetQueryParams(queryParams);
                }

                return await request.GetJsonAsync<T>();
            }
            catch (FlurlHttpException ex)
            {
                HandleApiException(ex);
                throw;
            }
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                return await ConfigureRequest(endpoint)
                    .PostJsonAsync(data)
                    .ReceiveJson<T>();
            }
            catch (FlurlHttpException ex)
            {
                HandleApiException(ex);
                throw;
            }
        }

        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                return await ConfigureRequest(endpoint)
                    .PutJsonAsync(data)
                    .ReceiveJson<T>();
            }
            catch (FlurlHttpException ex)
            {
                HandleApiException(ex);
                throw;
            }
        }

        public async Task<T> PatchAsync<T>(string endpoint, object data)
        {
            try
            {
                return await ConfigureRequest(endpoint)
                    .PatchJsonAsync(data)
                    .ReceiveJson<T>();
            }
            catch (FlurlHttpException ex)
            {
                HandleApiException(ex);
                throw;
            }
        }

        public async Task<T> DeleteAsync<T>(string endpoint)
        {
            try
            {
                return await ConfigureRequest(endpoint)
                    .DeleteAsync()
                    .ReceiveJson<T>();
            }
            catch (FlurlHttpException ex)
            {
                HandleApiException(ex);
                throw;
            }
        }

        private void HandleApiException(FlurlHttpException ex)
        {
            if (ex.StatusCode == 401)
            {
                _httpContextAccessor.HttpContext?.Response.Cookies.Delete(_accessToken);
                throw new UnauthorizedAccessException("Unauthorized. Token is invalid or expired.", ex);
            }

            throw new Exception("API exception", ex);
        }

    }
}
