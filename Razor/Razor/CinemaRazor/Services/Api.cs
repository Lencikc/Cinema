using CinemaRazor.Models;
using CinemaRazor.Requests;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CinemaRazor.Services
{
    public class Api
    {
        private readonly HttpClient _http;
        private readonly AuthState _auth;
        private readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public Api(HttpClient http, AuthState auth)
        {
            _http = http;
            _auth = auth;
        }

        private void Bearer()
        {
            _http.DefaultRequestHeaders.Authorization =
                _auth.Token == null ? null : new AuthenticationHeaderValue("Bearer", _auth.Token);
        }

        public async Task<JsonElement?> Post(string url, object body)
        {
            try
            {
                Bearer();
                var r = await _http.PostAsJsonAsync(url, body);
                return await r.Content.ReadFromJsonAsync<JsonElement>(_opts);
            }
            catch { return null; }
        }

        public async Task<JsonElement?> Put(string url, object? body = null)
        {
            try
            {
                Bearer();
                var r = body == null
                    ? await _http.PutAsync(url, null)
                    : await _http.PutAsJsonAsync(url, body);
                return await r.Content.ReadFromJsonAsync<JsonElement>(_opts);
            }
            catch { return null; }
        }

        public async Task<JsonElement?> Get(string url)
        {
            try
            {
                Bearer();
                return await _http.GetFromJsonAsync<JsonElement>(url, _opts);
            }
            catch { return null; }
        }

        public async Task<JsonElement?> Delete(string url)
        {
            try
            {
                Bearer();
                var r = await _http.DeleteAsync(url);
                return await r.Content.ReadFromJsonAsync<JsonElement>(_opts);
            }
            catch { return null; }
        }
    }
}
