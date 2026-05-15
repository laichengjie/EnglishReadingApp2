using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;


namespace EnglishReadingApp
{
    public class QwenTTSService
    {
        private readonly HttpClient _httpClient;
        private const string API_URL = "https://dashscope.aliyuncs.com/api/v1/services/aigc/multimodal-generation/generation";

        // 官方推荐使用的模型
        // "qwen3-tts-flash" - 快速合成
        // "qwen3-tts-instruct" - 支持指令控制（如控制语速、语调）
        private const string MODEL = "qwen3-tts-flash";

        private readonly string _apiKey = "sk-90da95cc457743fe8b069b38d5e57b7c";

        public QwenTTSService()
        {

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// 将文本转换为语音
        /// </summary>
        /// <param name="text">要转换的文本</param>
        /// <param name="voice">音色: CHERRY, SERENA, ETHAN, CHELSIE</param>
        /// <param name="languageType">语言类型: English, Chinese, etc.</param>
        /// <returns>音频流</returns>
        public async Task<Stream> SpeakAsync(string text, string voice = "CHERRY", string languageType = "Chinese")
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("文本不能为空", nameof(text));
            }

            try
            {
                // 构建请求（参照官方 Java SDK 格式）
                var request = new
                {
                    model = MODEL,
                    input = new
                    {
                        text = text
                    },
                    parameters = new
                    {
                        voice = voice,
                        language_type = languageType,
                        format = "wav"
                    }
                };

                var json = JsonSerializer.Serialize(request);
                System.Diagnostics.Debug.WriteLine($"Qwen TTS 请求: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(API_URL, content);

                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"响应状态: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"响应内容: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"API 请求失败: {responseContent}");
                }

                // 解析响应，获取音频 URL
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;

                if (root.TryGetProperty("output", out var output))
                {
                    if (output.TryGetProperty("audio", out var audio))
                    {
                        if (audio.TryGetProperty("url", out var urlElement))
                        {
                            var audioUrl = urlElement.GetString();
                            System.Diagnostics.Debug.WriteLine($"音频 URL: {audioUrl}");

                            // 下载音频文件
                            if (!string.IsNullOrEmpty(audioUrl))
                            {
                                return await DownloadAudioAsync(audioUrl);
                            }
                        }
                    }
                }

                throw new Exception("无法从响应中获取音频 URL");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TTS 错误: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 将文本转换为语音
        /// </summary>
        /// <param name="text">要转换的文本（支持中文、拼音）</param>
        /// <param name="voice">音色: CHERRY, SERENA, ETHAN, CHELSIE</param>
        /// <param name="languageType">语言类型: Chinese, English, etc.</param>
        /// <returns>音频流</returns>
        public async Task<Stream> SpeakAsync3(string text, string voice = "CHERRY", string languageType = "Chinese")
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("文本不能为空", nameof(text));
            }

            try
            {
                // 构建请求（参照官方 Java SDK 格式）
                var request = new
                {
                    model = MODEL,
                    input = new
                    {
                        text = text
                    },
                    parameters = new
                    {
                        voice = voice,
                        language_type = languageType,  // 设置为 "Chinese" 读中文
                        format = "wav"
                    }
                };

                var json = JsonSerializer.Serialize(request);
                System.Diagnostics.Debug.WriteLine($"Qwen TTS 请求: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(API_URL, content);

                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"响应状态: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"响应内容: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"API 请求失败: {responseContent}");
                }

                // 解析响应，获取音频 URL
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;

                if (root.TryGetProperty("output", out var output))
                {
                    if (output.TryGetProperty("audio", out var audio))
                    {
                        if (audio.TryGetProperty("url", out var urlElement))
                        {
                            var audioUrl = urlElement.GetString();
                            System.Diagnostics.Debug.WriteLine($"音频 URL: {audioUrl}");

                            // 下载音频文件
                            if (!string.IsNullOrEmpty(audioUrl))
                            {
                                return await DownloadAudioAsync(audioUrl);
                            }
                        }
                    }
                }

                throw new Exception("无法从响应中获取音频 URL");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TTS 错误: {ex.Message}");
                throw;
            }
        }

        private async Task<Stream> DownloadAudioAsync(string url)
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var audioBytes = await response.Content.ReadAsByteArrayAsync();
            System.Diagnostics.Debug.WriteLine($"下载音频大小: {audioBytes.Length} 字节");

            return new MemoryStream(audioBytes);
        }

        /// <summary>
        /// 直接获取音频字节数组
        /// </summary>
        public async Task<byte[]> GetAudioBytesAsync(string text, string voice = "CHERRY", string languageType = "Chinese")
        {
            using var stream = await SpeakAsync(text, voice, languageType);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// 测试服务是否正常
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await SpeakAsync("测试", "CHERRY", "Chinese");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"测试失败: {ex.Message}");
                return false;
            }
        }
    }
}