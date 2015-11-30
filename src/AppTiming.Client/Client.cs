using System;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace AppTiming.Client
{
    /// <summary>
    /// The client class that performs all calls.
    /// Either call Start(), do the operation and call End()
    /// OR
    /// call TimeOperation() passing the action to keep time for
    /// </summary>
    public class AppTimingClient
    {
        public const string DefaultApiEndpoint = "http://192.168.0.21:3000/api/v1/";
        public const string UrlRegex = @"(https?:\/\/)?([\da-z\.-]+)\.([0-9a-z\.]{1,6})(:[0-9]*)([\/\w \.-]*)*\/?";

        object _lock = new object();
        protected WebClient _client = null;
        public string ApiKey { get; private set; }
        public readonly string ApiBaseUrl;


        public AppTimingClient(string apiKey)
            : this(apiKey, DefaultApiEndpoint, false)
        { }

        public AppTimingClient(string apiKey, string apiEndpoint)
            : this(apiKey, apiEndpoint, false)
        { }

        public AppTimingClient(string apiKey, bool attachUnobservedExceptionHandler)
            : this(apiKey, DefaultApiEndpoint, attachUnobservedExceptionHandler)
        { }

        public AppTimingClient(string apiKey, string apiBaseUrl, bool attachUnobservedExceptionHandler)
        {
            ApiKey = apiKey;
            if (string.IsNullOrWhiteSpace(apiBaseUrl)) throw new ArgumentNullException("apiEndpoint", "API Endpoint cannot be empty");
            if (!apiBaseUrl.EndsWith("/")) apiBaseUrl += "/";
            if (!apiBaseUrl.EndsWith("api/v1/")) apiBaseUrl += "api/v1/";
            if (!System.Text.RegularExpressions.Regex.IsMatch(apiBaseUrl, UrlRegex)) throw new ArgumentException("apiEndpoint", "Invalid endpoint " + apiBaseUrl);
            _client = new WebClient() { Encoding = Encoding.UTF8 };
            try { new Uri(apiBaseUrl); }
            catch(Exception ex) { throw new ArgumentException("apiEndpoint", string.Format("Invalid endpoint {0}, {1}", apiBaseUrl, ex.Message)); }
            ApiBaseUrl = apiBaseUrl;
            if (attachUnobservedExceptionHandler) TaskScheduler.UnobservedTaskException += GetUnobservedTaskExceptionHandler();
        }

        /// <summary>
        /// Start keeping time for an operation
        /// </summary>
        /// <param name="unitname">A desired identifier for the method keeping track of</param>
        /// <returns></returns>
        public async Task<string> Start(string unitname)
        {
            return await _createTask(unitname, ApiBaseUrl.ToString() + "track/" + unitname);
        }

        /// <summary>
        /// Stop keeping time for an operation
        /// </summary>
        /// <param name="unitname">The identifier used to Start() the time tracking</param>
        /// <returns></returns>
        public async Task<string> End(string unitname)
        {
            return await _createTask(unitname, ApiBaseUrl.ToString() + "time/" + unitname);
        }

        private Task<string> _createTask(string unitname, string url)
        {
            if (string.IsNullOrWhiteSpace(unitname)) throw new ArgumentNullException("unitname", "Unit name cannot be null");
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException("url", "Url cannot be null");
            Uri uri = new Uri(url);
            lock (_lock)
            {
                _client.Headers.Remove("Authorization");
                _client.Headers.Add("Authorization", Convert.ToBase64String(Encoding.UTF8.GetBytes("ApiKey " + ApiKey)));
                return _client.UploadStringTaskAsync(uri, string.Empty);
            }
        }

        /// <summary>
        /// Keep time for an operation
        /// </summary>
        /// <param name="unitname">The identifier to use for the operation</param>
        /// <param name="fn">The operation to run and keep time for</param>
        /// <returns></returns>
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
