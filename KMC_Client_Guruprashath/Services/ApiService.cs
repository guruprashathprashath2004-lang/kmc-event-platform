using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using KMC_Client_Guruprashath.Models;

namespace KMC_Client_Guruprashath.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient client;
        private static readonly JsonSerializerOptions jsonOpts = new(JsonSerializerDefaults.Web);

        public ApiService(HttpClient _client)
        {
            client = _client;
        }

        private HttpRequestMessage Authed(HttpMethod method, string url, string token)
        {
            var req = new HttpRequestMessage(method, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return req;
        }

        public async Task<List<CategoryVM>> GetCategoriesAsync()
        {
            var result = await client.GetFromJsonAsync<List<CategoryVM>>("api/Category", jsonOpts);
            return result ?? new List<CategoryVM>();
        }

        public async Task<List<CategoryVM>> GetCategoriesAdminAsync(string token)
        {
            using var req = Authed(HttpMethod.Get, "api/Category", token);
            var res = await client.SendAsync(req);
            if (!res.IsSuccessStatusCode) return new List<CategoryVM>();
            return await res.Content.ReadFromJsonAsync<List<CategoryVM>>(jsonOpts) ?? new List<CategoryVM>();
        }

        public async Task<List<EventVM>> GetUpcomingEventsAsync()
        {
            var result = await client.GetFromJsonAsync<List<EventVM>>("api/Event/upcoming", jsonOpts);
            return result ?? new List<EventVM>();
        }

        public async Task<List<EventVM>> GetFeaturedEventsAsync()
        {
            var result = await client.GetFromJsonAsync<List<EventVM>>("api/Event/featured", jsonOpts);
            return result ?? new List<EventVM>();
        }

        public async Task<List<EventVM>> GetEventsByCategoryAsync(int categoryId)
        {
            var result = await client.GetFromJsonAsync<List<EventVM>>($"api/Event/category/{categoryId}", jsonOpts);
            return result ?? new List<EventVM>();
        }

        public async Task<List<EventVM>> GetAllEventsAsync()
        {
            var result = await client.GetFromJsonAsync<List<EventVM>>("api/Event", jsonOpts);
            return result ?? new List<EventVM>();
        }

        public async Task<List<EventVM>> SearchEventsAsync(string keyword)
        {
            var res = await client.GetAsync($"api/Event/search/{Uri.EscapeDataString(keyword)}");
            if (!res.IsSuccessStatusCode) return new List<EventVM>();
            return await res.Content.ReadFromJsonAsync<List<EventVM>>(jsonOpts) ?? new List<EventVM>();
        }

        public async Task<EventVM?> GetEventByIdAsync(int id)
        {
            var res = await client.GetAsync($"api/Event/{id}");
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<EventVM>(jsonOpts);
        }

        private async Task<(bool Success, string? Token, string? FullName, string? Role, string Message)> ReadAuthResponse(HttpResponseMessage res)
        {
            if (!res.IsSuccessStatusCode)
                return (false, null, null, null, "Unable to reach the KMC API.");

            var json = await res.Content.ReadFromJsonAsync<JsonElement>();
            bool success = json.GetProperty("success").GetBoolean();
            string message = json.GetProperty("message").GetString() ?? "";
            string? token = success && json.TryGetProperty("token", out var t) ? t.GetString() : null;
            string? fullName = success && json.TryGetProperty("fullName", out var f) ? f.GetString() : null;
            string? role = success && json.TryGetProperty("role", out var r) ? r.GetString() : null;

            return (success, token, fullName, role, message);
        }

        public async Task<(bool Success, string? Token, string? FullName, string? Role, string Message)> LoginAsync(string username, string password)
        {
            var res = await client.PostAsJsonAsync("api/Auth/login", new { Username = username, Password = password }, jsonOpts);
            return await ReadAuthResponse(res);
        }

        public async Task<(bool Success, string? Token, string? FullName, string? Role, string Message)> RegisterAccountAsync(RegisterVM vm)
        {
            var payload = new
            {
                Username = vm.Username,
                Email = vm.Email,
                Password = vm.Password,
                FullName = vm.FullName,
                Phone = vm.Phone,
                Role = vm.Role
            };
            var res = await client.PostAsJsonAsync("api/Auth/register", payload, jsonOpts);
            return await ReadAuthResponse(res);
        }

        public async Task<(bool Success, string Message)> RegisterForEventAsync(RegistrationVM vm, string token)
        {
            var payload = new
            {
                FullName = vm.FullName,
                Email = vm.Email,
                Phone = vm.Phone,
                NIC = vm.NIC,
                PhotoUrl = vm.PhotoUrl,
                EventId = vm.EventId
            };

            using var req = Authed(HttpMethod.Post, "api/Registration", token);
            req.Content = JsonContent.Create(payload, options: jsonOpts);
            var res = await client.SendAsync(req);

            var body = await res.Content.ReadFromJsonAsync<JsonElement?>();
            string message = body != null && body.Value.TryGetProperty("message", out var m)
                ? m.GetString() ?? ""
                : (res.IsSuccessStatusCode ? "Registration successful." : "Registration failed.");

            return (res.IsSuccessStatusCode, message);
        }

        public async Task<List<RegistrationVM>> GetMyRegistrationsAsync(string token)
        {
            using var req = Authed(HttpMethod.Get, "api/Registration/mine", token);
            var res = await client.SendAsync(req);
            if (!res.IsSuccessStatusCode) return new List<RegistrationVM>();

            var raw = await res.Content.ReadFromJsonAsync<List<JsonElement>>(jsonOpts) ?? new List<JsonElement>();
            var list = new List<RegistrationVM>();
            foreach (var item in raw)
            {
                list.Add(new RegistrationVM
                {
                    RegistrationId = item.GetProperty("registrationId").GetInt32(),
                    FullName = item.GetProperty("fullName").GetString() ?? "",
                    Email = item.GetProperty("email").GetString() ?? "",
                    Phone = item.GetProperty("phone").GetString() ?? "",
                    NIC = item.GetProperty("nic").GetString() ?? "",
                    PhotoUrl = item.TryGetProperty("photoUrl", out var p) ? p.GetString() : null,
                    RegisteredOn = item.GetProperty("registeredOn").GetDateTime(),
                    EventId = item.GetProperty("eventId").GetInt32(),
                    Event = new EventVM { Title = item.GetProperty("eventTitle").GetString() ?? "" }
                });
            }
            return list;
        }

        public async Task<(string? Url, string Message)> UploadEventImageAsync(Stream fileStream, string fileName, string contentType, string token)
        {
            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(fileContent, "file", fileName);

            using var req = Authed(HttpMethod.Post, "api/Event/upload-image", token);
            req.Content = content;

            var res = await client.SendAsync(req);
            if (!res.IsSuccessStatusCode)
            {
                var body = await res.Content.ReadAsStringAsync();
                return (null, string.IsNullOrWhiteSpace(body) ? $"Image upload failed ({(int)res.StatusCode} {res.StatusCode})." : body);
            }
            var json = await res.Content.ReadFromJsonAsync<JsonElement>();
            var url = json.TryGetProperty("url", out var u) ? u.GetString() : null;
            return (url, "");
        }

        public async Task<List<EventVM>> GetMyEventsAsync(string token)
        {
            using var req = Authed(HttpMethod.Get, "api/Event/mine", token);
            var res = await client.SendAsync(req);
            if (!res.IsSuccessStatusCode) return new List<EventVM>();
            return await res.Content.ReadFromJsonAsync<List<EventVM>>(jsonOpts) ?? new List<EventVM>();
        }

        public async Task<List<EventVM>> GetPendingEventsAsync(string token)
        {
            using var req = Authed(HttpMethod.Get, "api/Event/pending", token);
            var res = await client.SendAsync(req);
            if (!res.IsSuccessStatusCode) return new List<EventVM>();
            return await res.Content.ReadFromJsonAsync<List<EventVM>>(jsonOpts) ?? new List<EventVM>();
        }

        public async Task<bool> ApproveEventAsync(int id, string token)
        {
            using var req = Authed(HttpMethod.Post, $"api/Event/{id}/approve", token);
            var res = await client.SendAsync(req);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> RejectEventAsync(int id, string token)
        {
            using var req = Authed(HttpMethod.Post, $"api/Event/{id}/reject", token);
            var res = await client.SendAsync(req);
            return res.IsSuccessStatusCode;
        }

        public async Task<(bool Success, string Message)> AddEventAsync(EventWriteVM vm, string token)
        {
            using var req = Authed(HttpMethod.Post, "api/Event", token);
            req.Content = JsonContent.Create(vm, options: jsonOpts);
            var res = await client.SendAsync(req);
            if (res.IsSuccessStatusCode) return (true, "");
            var body = await res.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(body) ? $"Request failed ({(int)res.StatusCode} {res.StatusCode})." : body);
        }

        public async Task<(bool Success, string Message)> UpdateEventAsync(EventWriteVM vm, string token)
        {
            using var req = Authed(HttpMethod.Put, $"api/Event/{vm.EventId}", token);
            req.Content = JsonContent.Create(vm, options: jsonOpts);
            var res = await client.SendAsync(req);
            if (res.IsSuccessStatusCode) return (true, "");
            var body = await res.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(body) ? $"Request failed ({(int)res.StatusCode} {res.StatusCode})." : body);
        }

        public async Task<bool> DeleteEventAsync(int id, string token)
        {
            using var req = Authed(HttpMethod.Delete, $"api/Event/{id}", token);
            var res = await client.SendAsync(req);
            return res.IsSuccessStatusCode;
        }

        public async Task<List<UserVM>> GetUsersAsync(string token)
        {
            using var req = Authed(HttpMethod.Get, "api/User", token);
            var res = await client.SendAsync(req);
            if (!res.IsSuccessStatusCode) return new List<UserVM>();
            return await res.Content.ReadFromJsonAsync<List<UserVM>>(jsonOpts) ?? new List<UserVM>();
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(int userId, string token)
        {
            using var req = Authed(HttpMethod.Delete, $"api/User/{userId}", token);
            var res = await client.SendAsync(req);

            if (res.IsSuccessStatusCode)
                return (true, "User removed.");

            var body = await res.Content.ReadFromJsonAsync<JsonElement?>();
            string message = body != null && body.Value.TryGetProperty("message", out var m)
                ? m.GetString() ?? "Could not remove this user."
                : "Could not remove this user.";
            return (false, message);
        }
    }
}
