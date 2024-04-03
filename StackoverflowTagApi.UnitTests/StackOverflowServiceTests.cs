using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using StackoverflowTagApi.Models;
using StackoverflowTagApi.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StackoverflowTagApi.UnitTests
{
    public class StackOverflowServiceTests
    {
        [Fact]
        public async Task GetTagsAsync_SuccessfulResponse_ReturnsTags()
        {
            var expectedTags = new List<Tag>();

            for (int i = 0; i < 1100; i++)
            {
                expectedTags.Add(new Tag { Name = $"tag{i}", Count = i });
            }

            var responseContent = new TagResponse { Items = expectedTags };
            var serializedContent = JsonSerializer.Serialize(responseContent);
            var compressedContent = Compress(serializedContent);
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, compressedContent);
            var loggerMock = new Mock<ILogger<StackOverflowService>>();
            var logger = loggerMock.Object;
            var service = new StackOverflowService(httpClient, logger);

            var tags = (await service.GetTagsAsync()).ToList();

            Assert.Equal(expectedTags.Count, tags.Count);
            for (int i = 0; i < expectedTags.Count; i++)
            {
                Assert.Equal(expectedTags[i].Name, tags[i].Name);
                Assert.Equal(expectedTags[i].Count, tags[i].Count);
            }

            loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.Never
            );
        }

        [Fact]
        public async Task GetTagsAsync_EmptyResponse_ReturnsEmptyList()
        {
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, Compress(""));
            var loggerMock = new Mock<ILogger<StackOverflowService>>();
            var logger = loggerMock.Object;
            var service = new StackOverflowService(httpClient, logger);

            var tags = await service.GetTagsAsync();

            Assert.Empty(tags);

            loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetTagsAsync_InvalidJsonResponse_ThrowsException()
        {
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, Compress("invalid json content"));
            var loggerMock = new Mock<ILogger<StackOverflowService>>();
            var logger = loggerMock.Object;
            var service = new StackOverflowService(httpClient, logger);

            await Assert.ThrowsAsync<JsonException>(async () => await service.GetTagsAsync());

            loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetTagsAsync_NotFound_HandlesException()
        {
            var httpClient = CreateMockHttpClient(HttpStatusCode.NotFound, Compress(""));
            var loggerMock = new Mock<ILogger<StackOverflowService>>();
            var logger = loggerMock.Object;
            var stackOverflowService = new StackOverflowService(httpClient, logger);

            await Assert.ThrowsAsync<HttpRequestException>(async () => await stackOverflowService.GetTagsAsync());

            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetTagsAsync_BadRequest_HandlesException()
        {
            var httpClient = CreateMockHttpClient(HttpStatusCode.BadRequest, Compress(""));
            var loggerMock = new Mock<ILogger<StackOverflowService>>();
            var logger = loggerMock.Object;
            var stackOverflowService = new StackOverflowService(httpClient, logger);

            await Assert.ThrowsAsync<HttpRequestException>(async () => await stackOverflowService.GetTagsAsync());

            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.Exactly(2)
            );
        }

        private static HttpClient CreateMockHttpClient(HttpStatusCode statusCode, byte[] content)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new ByteArrayContent(content),
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://api.stackexchange.com/2.3/");
            return httpClient;
        }

        private static byte[] Compress(string content)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    using (var writer = new StreamWriter(gzipStream))
                    {
                        writer.Write(content);
                    }
                }
                return outputStream.ToArray();
            }
        }
    }
}
