using System.Net.Http.Json;

namespace SharedLibrary.Utilities
{

    /// Helper utilities for HTTP client operations

    public static class HttpClientHelper
    {

        /// Validates that a user exists by calling UserService

        public static async Task<bool> ValidateUserExistsAsync(HttpClient httpClient, int userId, string userServiceUrl)
        {
            try
            {
                var response = await httpClient.GetAsync($"{userServiceUrl}/api/users/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }


        /// Gets user information from UserService

        public static async Task<DTOs.UserResponse?> GetUserAsync(HttpClient httpClient, int userId, string userServiceUrl)
        {
            try
            {
                var response = await httpClient.GetAsync($"{userServiceUrl}/api/users/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<DTOs.UserResponse>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}

