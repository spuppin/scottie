using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Scottie.Results;
using Scottie.Server;

namespace Scottie
{
    public enum CreateMode
    {
        Persistent,
        PersitentSequential,
        Ephemeral,
        EphemeralSequential
    }

    public interface IScottie : IDisposable
    {
        void Connect();
        void Disconnect();

        string Create(string path, string content, CreateMode mode);
        string Get(string path);
        IReadOnlyList<string> GetChildren(string path);

        void Update(string path, string content, long version);
        void Delete(string path, long version);

        void Multi(IList<MultiOpParams> operations);
    }

    public class ScottieClient : IScottie
    {
        public ScottieClient(ILogger logger = null)
        {
            _logger = logger ?? LogManager.GetCurrentClassLogger();
        }

        private readonly ILogger _logger;
        private readonly Uri _baseUri = new Uri("http://localhost:2323");

        // All of these look like the should be in a separate class.
        private HttpClient _client;
        private long _sessionId;
        private CancellationTokenSource _heartbeatCancellation;
        private Task _heartbeat;

        public void Connect()
        {
            _client = new HttpClient();

            var result = Post<SessionResult>("session", null);

            _sessionId = result.SessionId;
            _heartbeatCancellation = new CancellationTokenSource();
            _heartbeat = StartHeartbeatTask(TimeSpan.FromMilliseconds(2000), _heartbeatCancellation.Token);
        }

        public void Disconnect()
        {
            try
            {
                Delete<SessionResult>($"session/{_sessionId}");
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Disconnect failed");
            }

            StopHeartbeatTask();

            DisposeClient();
        }

        public string Create(string path, string data, CreateMode mode)
        {
            var content = GetHttpContent(new CreateParams {CreateMode = mode.ToString(), Data = data});
            var result = Post<CreateResult>($"znode/{_sessionId}/{path}", content);
            return result.Path;
        }

        public string Get(string path)
        {
            var result = Get<GetResult>($"znode/{_sessionId}/{path}");
            return result.Node.Path;
        }

        public IReadOnlyList<string> GetChildren(string path)
        {
            var result = Get<ChildrenResult>($"znode/{_sessionId}/children/{path}");
            return result.Children;
        }

        public void Update(string path, string data, long version)
        {
            var content = GetHttpContent(new UpdateParams {Data = data, Version = version});
            Put<UpdateResult>($"znode/{_sessionId}/{path}", content);
        }

        public void Delete(string path, long version)
        {
            Delete<DeleteResult>($"znode/{_sessionId}/{version}/{path}");
        }

        public void Multi(IList<MultiOpParams> operations)
        {
            HttpContent content = GetHttpContent(operations);
            Post<MultiResult>($"znode/{_sessionId}/multi", content);
        }

        private static HttpContent GetHttpContent(object value)
        {
            var json = JsonConvert.SerializeObject(value);
            var stringContent = new StringContent(json);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return stringContent;
        }

        private void StopHeartbeatTask()
        {
            _heartbeatCancellation?.Cancel();

            if (_heartbeat != null)
            {
                try
                {
                    Task.WaitAll(_heartbeat);
                }
                catch (AggregateException ae) when (ae.InnerException is TaskCanceledException)
                {
                    _logger.Debug("Heartbeat stopped because the task was cancelled.");
                }

                _heartbeat = null;
            }
        }

        public async Task StartHeartbeatTask(TimeSpan period, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(period, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    Put<SessionResult>($"session/{_sessionId}", null);
                }
            }
        }

        private T Post<T>(string relativeUri, HttpContent content) where T : class
        {
            return Execute<T>(relativeUri, requestUri =>
                    _client.PostAsync(requestUri, content).Result);
        }

        private T Put<T>(string relativeUri, HttpContent content) where T : class
        {
            return Execute<T>(relativeUri, requestUri =>
                    _client.PutAsync(requestUri, content).Result);
        }

        private T Delete<T>(string relativeUri) where T : class
        {
            return Execute<T>(relativeUri, requestUri =>
                    _client.DeleteAsync(requestUri).Result);
        }


        private T Get<T>(string relativeUri) where T : class
        {
            return Execute<T>(relativeUri, requestUri =>
                    _client.GetAsync(requestUri).Result);
        }


        private T Execute<T>(string relativeUri, Func<Uri, HttpResponseMessage> action) where T : class
        {
            var requestUri = new Uri(_baseUri, relativeUri);

            HttpResponseMessage response = action(requestUri);

            return GetFromResponse<T>(response);
        }

        private static T GetFromResponse<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            Stream stream = response.Content.ReadAsStreamAsync().Result;
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return (T) serializer.Deserialize(jsonTextReader, typeof(T));
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ScottieClient()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
            }
        }

        private void DisposeClient()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }
    }
}