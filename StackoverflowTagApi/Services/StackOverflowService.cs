using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackoverflowTagApi.Interfaces;
using StackoverflowTagApi.Models;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StackoverflowTagApi.Services
{
    public class StackOverflowService : IStackOverflowService
    {
        private readonly HttpClient _client;
        private readonly ILogger<StackOverflowService> _logger;

        public StackOverflowService(HttpClient client, ILogger<StackOverflowService> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _client.BaseAddress = new Uri("https://api.stackexchange.com/2.3/");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            const int pageSize = 100;
            const int minTags = 1000;

            var tags = new List<Tag>();

            try
            {
                for (int page = 1; tags.Count < minTags; page++)
                {
                    var response = await _client.GetAsync($"tags?order=desc&sort=popular&site=stackoverflow&page={page}&pagesize={pageSize}");

                    if (response.IsSuccessStatusCode)
                    {
                        using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                        using (var decompressedStream = new GZipStream(contentStream, CompressionMode.Decompress))
                        using (var streamReader = new StreamReader(decompressedStream))
                        {
                            var content = await streamReader.ReadToEndAsync();

                            if (string.IsNullOrEmpty(content))
                            {
                                _logger.LogInformation("Response content is empty. Returning the retrieved tags.");
                                break;
                            }

                            var result = JsonSerializer.Deserialize<TagResponse>(content);
                            tags.AddRange(result.Items);

                            if (tags.Count >= minTags)
                                break;
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning($"HTTP request returned status code 404: {response.ReasonPhrase}");
                        throw new HttpRequestException($"Failed to fetch tags: {response.StatusCode}");
                    }
                    else
                    {
                        _logger.LogError($"HTTP request failed with status code {response.StatusCode}: {response.ReasonPhrase}");
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP request failed: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON deserialization failed: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred: {ex.Message}");
                throw;
            }

            return tags.ToList();
        }
    }
}