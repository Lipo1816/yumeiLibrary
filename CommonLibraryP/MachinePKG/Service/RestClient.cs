using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
    public class RestClient
    {
        public string EndPoint { get; set; } = "https://portal.temphawk.net/api/v2";
        public EnumHttpVerb Method { get; set; } = EnumHttpVerb.GET;
        public string? PostData { get; set; }
        public string ContentType { get; set; } = "application/json";

        ///https://portal.temphawk.net/api/v2/devices/

        public string token_Header { get; set; } = "?api_token=";  //FQAagh78Z6byQWqx9rYcHvdKWhifMzUFNd6HYqhL5p7PFk8jPkxru3WKXDyx
        public string? token_Content { get; set; } // = "FQAagh78Z6byQWqx9rYcHvdKWhifMzUFNd6HYqhL5p7PFk8jPkxru3WKXDyx"; // 這是範例 token，請替換為實際的 API Token

       // token_Content=ReadTokenFromConfig();
        public RestClient()
        {
            // 先讀取設定檔，若失敗則用預設值
            token_Content = ReadTokenFromConfig() ?? "hqR9JS1Gu8pYrJrancwhy1ImffLnupAYq9HmiTDbU761bc3Z0sUxrq0i1ki4";
        }
        //?api_token=VUQY38XHEGF4WWM3YTlLdRJYBOj26VCOMoSyuzy80pm0fqJbGrTQlT63lfeK     FQAagh78Z6byQWqx9rYcHvdKWhifMzUFNd6HYqhL5p7PFk8jPkxru3WKXDyx
        // //https://portal.temphawk.net/api/v2//devices/00EFD23A?api_token=VUQY38XHEGF4WWM3YTlLdRJYBOj26VCOMoSyuzy80pm0fqJbGrTQlT63lfeK

        public async Task<string> HttpRequestAsync()
        {
            using var client = new HttpClient();
            HttpResponseMessage response;

            switch (Method)
            {
                case EnumHttpVerb.GET:
                    response = await client.GetAsync(EndPoint);
                    break;
                case EnumHttpVerb.POST:
                    response = await client.PostAsync(EndPoint, new StringContent(PostData ?? "", Encoding.UTF8, ContentType));
                    break;
                case EnumHttpVerb.PUT:
                    response = await client.PutAsync(EndPoint, new StringContent(PostData ?? "", Encoding.UTF8, ContentType));
                    break;
                case EnumHttpVerb.DELETE:
                    response = await client.DeleteAsync(EndPoint);
                    break;
                default:
                    throw new NotSupportedException("Method not supported");
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<DeviceDataResponse?> GetDeviceDataAsync(string machineNumber)
        {
            // 假設 API URL 格式如下，請依實際情況調整
            string url = $"https://portal.temphawk.net/api/v2/devices/{machineNumber}"+ token_Header+ token_Content;
            this.Method = EnumHttpVerb.GET;
            this.ContentType = "application/json";
            this.EndPoint = url;

            var json = await this.HttpRequestAsync();
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                var result = JsonSerializer.Deserialize<DeviceDataResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result;
            }
            catch (Exception ex)
            {
                // 可加上 log
                return null;
            }
        }


        private string? ReadTokenFromConfig()
        {
            try
            {
                // 絕對路徑
                var configPath = @"E:\玉美\code\CommonLibraryP\CommonLibraryP\MachinePKG\Service\UserConfig\userConfig.txt";
                if (!File.Exists(configPath))
                    return null;

                var lines = File.ReadAllLines(configPath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("Temp_Token:", StringComparison.OrdinalIgnoreCase))
                    {
                        return line.Substring("Temp_Token:".Length).Trim();
                    }
                }
            }
            catch
            {
                // 可加上 log
            }
            return null;
        }
    }
    public enum EnumHttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    public class DeviceDataResponse
    {
        public bool success { get; set; }
        public DeviceInfo? device { get; set; }
    }

    public class DeviceInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string sensorType { get; set; }
        public string status { get; set; }
        public long activation_date { get; set; }
        public long expiration_date { get; set; }
        public List<DeviceDataItem> device_data { get; set; }
    }

    public class DeviceDataItem
    {
        public long recorded_at { get; set; }
        public List<DataValue> data { get; set; }
    }

    public class DataValue
    {
        public string data_type { get; set; }
        public string value { get; set; }
        public string unit { get; set; }
    }
}
