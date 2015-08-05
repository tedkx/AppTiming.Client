using System;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace AppTiming.Client
{
    public class AppTimingClient
    {
        public const string DefaultApiEndpoint = "http://192.168.0.21:3000/api/v1/";
        public const string UrlRegex = @"(https?:\/\/)?([\da-z\.-]+)\.([0-9a-z\.]{1,6})(:[0-9]*)([\/\w \.-]*)*\/?";

        object _lock = new object();
        private Uri _apiEndpoint;
        protected WebClient _client = null;
        public string ApiKey { get; private set; }


        public AppTimingClient(string apiKey)
            : this(apiKey, DefaultApiEndpoint, false)
        { }

        public AppTimingClient(string apiKey, string apiEndpoint)
            : this(apiKey, apiEndpoint, false)
        { }

        public AppTimingClient(string apiKey, bool attachUnobservedExceptionHandler)
            : this(apiKey, DefaultApiEndpoint, attachUnobservedExceptionHandler)
        { }

        public AppTimingClient(string apiKey, string apiEndpoint, bool attachUnobservedExceptionHandler)
        {
            ApiKey = apiKey;
            if (string.IsNullOrWhiteSpace(apiEndpoint)) throw new ArgumentNullException("apiEndpoint", "API Endpoint cannot be empty");
            if (!System.Text.RegularExpressions.Regex.IsMatch(apiEndpoint, UrlRegex)) throw new ArgumentException("apiEndpoint", "Invalid endpoint " + apiEndpoint);
            _client = new WebClient() { Encoding = Encoding.UTF8 };
            if (attachUnobservedExceptionHandler) TaskScheduler.UnobservedTaskException += GetUnobservedTaskExceptionHandler();
        }

        public async Task<string> Start(string unitname)
        {
            return await _createTask(unitname, "http://192.168.0.21:3000/api/v1/track/");
        }

        public async Task<string> End(string unitname)
        {
            return await _createTask(unitname, "http://192.168.0.21:3000/api/v1/time/");
        }

        private Task<string> _createTask(string unitname, string url)
        {
            if (string.IsNullOrWhiteSpace(unitname)) throw new ArgumentNullException("unitname", "Unit name cannot be null");
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException("url", "Url cannot be null");
            if (!url.EndsWith("/")) url += "/";
            lock (_lock)
            {
                _client.Headers.Remove("Authorization");
                _client.Headers.Add("Authorization", Convert.ToBase64String(Encoding.UTF8.GetBytes("ApiKey " + ApiKey)));
                return _client.UploadStringTaskAsync(new Uri("http://192.168.0.21:3000/api/v1/track/" + unitname), string.Empty);
            }
        }

        public async Task<string> TimeOperation(string unitname, Action fn)
        {
            await Start(unitname);
            fn();
            return await End(unitname);
        }

        public static System.EventHandler<UnobservedTaskExceptionEventArgs> GetUnobservedTaskExceptionHandler()
        {
            return ((sender, e) => e.SetObserved());
        }
    }
}
