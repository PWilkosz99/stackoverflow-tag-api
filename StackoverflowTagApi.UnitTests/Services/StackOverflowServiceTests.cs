//using Microsoft.AspNetCore.Routing;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Moq.Protected;
//using StackoverflowTagApi.Models;
//using StackoverflowTagApi.Services;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO.Compression;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace StackoverflowTagApi.UnitTests.Services
//{
//    public class StackOverflowServiceTests
//    {
//        [Fact]
//        public async Task GetTagsAsync_Success()
//        {
//            var httpClient = new HttpClient();
//            var logger = new Mock<ILogger<StackOverflowService>>().Object;
//            var stackOverflowService = new StackOverflowService(httpClient, logger);

//            await stackOverflowService.GetTagsAsync();

//            Assert.True(true);
//        }

//        [Fact]
//        public async Task GetTagsAsync_SuccessfulResponse_ReturnsTags()
//        {
//            var expectedTags = new List<Tag>
//            {
//                new Tag { Name = "tag1", Count = 10 },
//                new Tag { Name = "tag2", Count = 5 }
//            };
//            var responseContent = new TagResponse { Items = expectedTags };
//            var serializedContent = JsonSerializer.Serialize(responseContent);
//            var compressedContent = Compress(serializedContent);
//            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, compressedContent);
//            var logger = new Mock<ILogger<StackOverflowService>>().Object;
//            var service = new StackOverflowService(httpClient, logger);

//            var tags = await service.GetTagsAsync();

//            Assert.Equal(expectedTags.Count, tags.Count);
//            for (int i = 0; i < expectedTags.Count; i++)
//            {
//                Assert.Equal(expectedTags[i].Name, tags[i].Name);
//                Assert.Equal(expectedTags[i].Count, tags[i].Count);
//            }
//        }

//        [Fact]
//        public async Task GetTagsAsync_EmptyResponse_ReturnsEmptyList()
//        {
//            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, Compress(""));
//            var logger = new Mock<ILogger<StackOverflowService>>().Object;
//            var service = new StackOverflowService(httpClient, logger);

//            var tags = await service.GetTagsAsync();

//            Assert.Empty(tags);
//        }

//        [Fact]
//        public async Task GetTagsAsync_InvalidJsonResponse_ThrowsException()
//        {
//            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, Compress("invalid json content"));
//            var logger = new Mock<ILogger<StackOverflowService>>().Object;
//            var service = new StackOverflowService(httpClient, logger);

//            await Assert.ThrowsAsync<JsonException>(async () => await service.GetTagsAsync());
//        }

//        private static HttpClient CreateMockHttpClient(HttpStatusCode statusCode, byte[] content)
//        {
//            var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//            mockHttpMessageHandler
//                .Protected()
//                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
//                .ReturnsAsync(new HttpResponseMessage
//                {
//                    StatusCode = statusCode,
//                    Content = new ByteArrayContent(content),
//                });

//            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
//            httpClient.BaseAddress = new Uri("https://api.stackexchange.com/2.3/");
//            return httpClient;
//        }

//        private static byte[] Compress(string content)
//        {
//            using (var outputStream = new MemoryStream())
//            {
//                using (var gzipStream = new GZipStream(outputStream, System.IO.Compression.CompressionMode.Compress))
//                {
//                    using (var writer = new StreamWriter(gzipStream))
//                    {
//                        writer.Write(content);
//                    }
//                }
//                return outputStream.ToArray();
//            }
//        }
//    }
//}
