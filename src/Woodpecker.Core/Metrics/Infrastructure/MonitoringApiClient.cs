﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Woodpecker.Core.Metrics.Infrastructure
{
    public class MonitoringApiClient : IMonitoringApiClient
    {
        private readonly IHttpClient httpClient;
        private readonly ISecurityTokenProvider securityTokenProvider;

        public MonitoringApiClient(ISecurityTokenProvider securityTokenProvider, IHttpClient httpClient)
        {
            this.securityTokenProvider = securityTokenProvider;
            this.httpClient = httpClient;
        }

        public async Task<MetricsResponse> FetchMetrics(IMetricsRequest metricsRequest)
        {
            var token = await this.securityTokenProvider.GetSecurityTokenAsync().ConfigureAwait(false);

            var request = new HttpRequestMessage(HttpMethod.Get, UriFactory.CreateMonitoringUriWithMetricFilter(metricsRequest));

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await this.httpClient.SendAsync(request).ConfigureAwait(false);

            return await Deserialise(response).ConfigureAwait(false);
        }

        private static async Task<MetricsResponse> Deserialise(HttpResponseMessage response)
        {
            var contentAsstring = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<MetricsResponse>(contentAsstring);
        }
    }
}