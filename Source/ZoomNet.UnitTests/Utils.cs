using Microsoft.Extensions.Logging;
using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using Pathoschild.Http.Client.Retry;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
using ZoomNet.Utilities;

namespace ZoomNet.UnitTests
{
	public static class Utils
	{
		private const string ZOOM_V2_BASE_URI = "https://api.zoom.us/v2";

		public static Pathoschild.Http.Client.IClient GetFluentClient(MockHttpMessageHandler httpMessageHandler)
		{
			var httpClient = httpMessageHandler.ToHttpClient();
			var client = new FluentClient(new Uri(ZOOM_V2_BASE_URI), httpClient);
			client.SetRequestCoordinator(new IRetryConfig[] { new ExpiredOAuthTokenRetryStrategy(null), new Http429RetryStrategy() });
			client.Filters.Remove<DefaultErrorFilter>();
			client.Filters.Add(new DiagnosticHandler(LogLevel.Debug, LogLevel.Error));
			client.Filters.Add(new ZoomErrorHandler());

			return client;
		}

		public static string GetZoomApiUri(params object[] resources)
		{
			return resources.Aggregate(ZOOM_V2_BASE_URI, (current, path) => $"{current.TrimEnd('/')}/{path.ToString().TrimStart('/')}");
		}
	}
}
