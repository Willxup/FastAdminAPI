using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FastAdminAPI.Common.Logs;
using Newtonsoft.Json;

namespace FastAdminAPI.Network.Utilities
{
    /// <summary>
    /// HttpClient帮助类
    /// 需要依赖注入
    /// </summary>
    public class HttpClientTool
    {
        /// <summary>
        /// HttpClient工厂
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientTool(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        #region 内部方法
        /// <summary>
        /// 构建HttpClient
        /// </summary>
        /// <param name="headers">请求头</param>
        /// <param name="timeoutSecond">超时时间</param>
        /// <returns></returns>
        private HttpClient BuildHttpClient(Dictionary<string, string> headers, int? timeoutSecond)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();

            // 清空默认请求头
            httpClient.DefaultRequestHeaders.Clear();

            // 设置content-type
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // 设置编码格式
            httpClient.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));

            // 设置自定义请求头
            if (headers != null && headers?.Count > 0)
            {
                foreach (var item in headers)
                {
                    if (!httpClient.DefaultRequestHeaders.Contains(item.Key))
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
            }

            // 设置超时
            if (timeoutSecond != null && timeoutSecond > 0)
            {
                httpClient.Timeout = TimeSpan.FromSeconds(timeoutSecond.Value);
            }

            return httpClient;
        }
        /// <summary>
        /// 生成HttpRequestMessage
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="requestBody">请求体</param>
        /// <param name="method">http请求方式</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        private static HttpRequestMessage GenerateHttpRequestMessage(string url, string requestBody, HttpMethod method, Dictionary<string, string> headers)
        {
            var request = new HttpRequestMessage(method, url);
            
            if (!string.IsNullOrEmpty(requestBody))
            {
                request.Content = new StringContent(requestBody);
            }

            if (headers != null && headers?.Count > 0)
            {
                foreach (var item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            return request;
        }
        /// <summary>
        ///  生成请求体(包含请求头)
        /// </summary>
        /// <param name="requestBody">请求体</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        private static StringContent GenerateStringContent(string requestBody, Dictionary<string, string> headers)
        {
            var content = new StringContent(requestBody);

            if (headers != null && headers?.Count > 0)
            {
                foreach (var item in headers)
                {
                    content.Headers.Add(item.Key, item.Value);
                }
            }

            return content;
        }
        /// <summary>
        /// 转换响应内容Json格式
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="responseContent"></param>
        /// <returns></returns>
        private static TResult ConvertResponseContentToResult<TResult>(string responseContent)
        {
            try
            {
                // 空校验
                if (string.IsNullOrEmpty(responseContent))
                {
                    return default;
                }
                // 返回结果类型为字符串
                if (typeof(TResult) == typeof(string))
                {
                    return (TResult)Convert.ChangeType(responseContent, typeof(TResult));
                }
                // 其他
                else
                {
                    var result = JsonConvert.DeserializeObject<TResult>(responseContent);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("返回结果无法转换为目标实体", ex);
            }
        }
        #endregion

        /// <summary>
        /// 获取HTTPClient
        /// </summary>
        /// <param name="headers">请求头</param>
        /// <param name="timeoutSecond">超时时间</param>
        /// <returns></returns>
        public HttpClient GetHttpClient(Dictionary<string, string> headers = null, int timeoutSecond = 60)
        {
            return BuildHttpClient(headers, timeoutSecond);
        }
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="headers">请求头</param>
        /// <param name="timeoutSecond">超时时间</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> headers = null, int timeoutSecond = 60)
        {
            try
            {
                var client = BuildHttpClient(headers, timeoutSecond);

                var response = await client.GetAsync(url);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return ConvertResponseContentToResult<T>(responseContent);
                }
                else
                {
                    throw new Exception("Http Code:" + response.StatusCode + ", Response:" + responseContent);
                }
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"HTTP请求失败，HTTP-Get:{url} Error:{ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Post
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="requestBody">请求体</param>
        /// <param name="headers">请求头</param>
        /// <param name="timeoutSecond">超时时间</param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string url, string requestBody, Dictionary<string, string> headers = null, int timeoutSecond = 60)
        {
            try
            {
                var client = BuildHttpClient(null, timeoutSecond);

                var requestContent = GenerateStringContent(requestBody, headers);

                var response = await client.PostAsync(url, requestContent);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return ConvertResponseContentToResult<T>(responseContent);
                }
                else
                {
                    throw new Exception("Http Code:" + response.StatusCode + ", Response:" + responseContent);
                }
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"HTTP请求失败，HTTP-Post:{url} Error:{ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="requestBody">请求体</param>
        /// <param name="headers">请求头</param>
        /// <param name="timeoutSecond">超时时间</param>
        /// <returns></returns>
        public async Task<T> PutAsync<T>(string url, string requestBody, Dictionary<string, string> headers = null, int timeoutSecond = 60)
        {
            try
            {
                var client = BuildHttpClient(null, timeoutSecond);

                var requestContent = GenerateStringContent(requestBody, headers);

                var response = await client.PutAsync(url, requestContent);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return ConvertResponseContentToResult<T>(responseContent);
                }
                else
                {
                    throw new Exception("Http Code:" + response.StatusCode + ", Response:" + responseContent);
                }
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"HTTP请求失败，HTTP-Put:{url} Error:{ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Patch
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="requestBody">请求体</param>
        /// <param name="headers">请求头</param>
        /// <param name="timeoutSecond">超时时间</param>
        /// <returns></returns>
        public async Task<T> PatchAsync<T>(string url, string requestBody, Dictionary<string, string> headers = null, int timeoutSecond = 60)
        {
            try
            {
                var client = BuildHttpClient(null, timeoutSecond);

                var requestContent = GenerateStringContent(requestBody, headers);

                var response = await client.PatchAsync(url, requestContent);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return ConvertResponseContentToResult<T>(responseContent);
                }
                else
                {
                    throw new Exception("Http Code:" + response.StatusCode + ", Response:" + responseContent);
                }
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"HTTP请求失败，HTTP-Patch:{url} Error:{ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="headers">请求头</param>
        /// <param name="timeoutSecond">超时时间</param>
        /// <returns></returns>
        public async Task<T> DeleteAsync<T>(string url, Dictionary<string, string> headers = null, int timeoutSecond = 60)
        {
            try
            {
                var client = BuildHttpClient(headers, timeoutSecond);

                var response = await client.DeleteAsync(url);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return ConvertResponseContentToResult<T>(responseContent);
                }
                else
                {
                    throw new Exception("Http Code:" + response.StatusCode + ", Response:" + responseContent);
                }
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"HTTP请求失败，HTTP-Delete:{url} Error:{ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// common request
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="method">http请求方式</param>
        /// <param name="requestBody">请求体</param>
        /// <param name="headers">请求头</param>
        /// <param name="timeoutSecond">超时时间</param>
        /// <returns></returns>
        public async Task<T> ExecuteAsync<T>(string url, HttpMethod method, string requestBody, Dictionary<string, string> headers, int timeoutSecond = 60)
        {
            try
            {
                var client = BuildHttpClient(null, timeoutSecond);

                var request = GenerateHttpRequestMessage(url, requestBody, method, headers);

                var response = await client.SendAsync(request);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return ConvertResponseContentToResult<T>(responseContent);
                }
                else
                {
                    throw new Exception("Http Code:" + response.StatusCode + ", Response:" + responseContent);
                }
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"HTTP请求失败，HTTP-Execute:{url} Error:{ex.Message}", ex);
                throw new Exception(ex.Message);
            }

        }
    }
}
