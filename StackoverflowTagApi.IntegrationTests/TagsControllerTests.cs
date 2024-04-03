using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using StackoverflowTagApi.Controllers;
using StackoverflowTagApi.Data;
using StackoverflowTagApi.Interfaces;
using StackoverflowTagApi.Models;
using StackoverflowTagApi.Repository;
using StackoverflowTagApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StackoverflowTagApi.IntegrationTests
{
    public class TagsControllerTests
    {
        public class TagsControllerIntegrationTests : IDisposable
        {
            private CustomWebApplicationFactory _factory;
            private HttpClient _client;

            public void Dispose()
            {
                _client.Dispose();
                _factory.Dispose();
            }

            public TagsControllerIntegrationTests()
            {
                _factory = new CustomWebApplicationFactory();
                _client = _factory.CreateClient();
            }

            [Fact]
            public async Task GetTags_WithSortingAndPagination_ReturnsOk()
            {
                var response = await _client.GetAsync("/Tags?sortBy=name&ascending=true&page=1&pageSize=10");

                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var responseString = await response.Content.ReadAsStringAsync();
                var tags = JsonSerializer.Deserialize<List<Tag>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Assert.NotNull(tags);
                Assert.NotEmpty(tags);

                Assert.True(IsSortedByName(tags));

                Assert.True(tags.Count <= 10);
            }

            [Fact]
            public async Task GetTags_WithDefaultSorting_ReturnsOk()
            {
                var response = await _client.GetAsync("/Tags");

                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var responseString = await response.Content.ReadAsStringAsync();
                var tags = JsonSerializer.Deserialize<List<Tag>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Assert.NotNull(tags);
                Assert.NotEmpty(tags);
            }

            [Fact]
            public async Task RefreshTags_ReturnsOk()
            {
                var response = await _client.PostAsync("/Tags/refresh", null);

                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            private bool IsSortedByName(List<Tag> tags)
            {
                if (tags.Count < 2)
                    return true;
                return string.Compare(tags.First().Name, tags.Last().Name) <= 0;
            }
        }
    }
}
