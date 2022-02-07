using Microsoft.AspNetCore.Http;
using Monq.Core.Paging.Extensions;
using Monq.Core.Paging.Models;
using Monq.Core.Paging.Models.DataTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Monq.Core.Paging.Tests
{
    public class PagingExtensionsTests
    {
        class Build
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Build SubBuild { get; set; }
        }

        static IEnumerable<Build> GenerateBuilds(int count = 1)
        {
            var builds = new List<Build>();
            for (var i = 0; i < count; i++)
            {
                builds.Add(new Build { Id = i, Name = $"Сборка test_{i}" });
            }

            return builds;
        }

        [Fact(DisplayName = "Проверка постраничной навигации для 4/8 записей, страница 1.")]
        public void ShouldProperlyReturnDataWith4of8Page1()
        {
            var paging = new PagingModel { Page = 1, PerPage = 4 };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = GenerateBuilds(8);
            var result = builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToList();

            Assert.Equal(4, result.Count);

            var linksHeader = (string)httpContext.Response.Headers["Link"];

            Assert.False(string.IsNullOrEmpty(linksHeader));

            var pageLinks = linksHeader.Replace("\",", "\"|").Split('|');

            Assert.Equal(2, pageLinks.Length);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=2>; rel=\"next\"", pageLinks[0]);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=2>; rel=\"last\"", pageLinks[1]);

            Assert.Equal("8", httpContext.Response.Headers["X-Total"]);
            Assert.Equal("8", httpContext.Response.Headers["X-Total-Filtered"]);
            Assert.Equal("4", httpContext.Response.Headers["X-Per-Page"]);
            Assert.Equal("1", httpContext.Response.Headers["X-Page"]);
            Assert.Equal("2", httpContext.Response.Headers["X-Total-Pages"]);
        }

        [Fact(DisplayName = "Проверка постраничной навигации c ответом DataTables для 4/8 записей, страница 1.")]
        public async Task ShouldProperlyReturnDataTablesWith4of8Page1()
        {
            var paging = new Models.PagingModel { Page = 1, PerPage = 4 };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = new TestAsyncEnumerable<Build>(
                GenerateBuilds(8));

            var result = await builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToDataTablesResponseAsync(httpContext);

            Assert.Equal(4, result.Data.Count());

            var linksHeader = (string)httpContext.Response.Headers["Link"];

            Assert.False(string.IsNullOrEmpty(linksHeader));

            var pageLinks = linksHeader.Replace("\",", "\"|").Split('|');

            Assert.Equal(2, pageLinks.Length);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=2>; rel=\"next\"", pageLinks[0]);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=2>; rel=\"last\"", pageLinks[1]);

            Assert.Equal("8", httpContext.Response.Headers["X-Total"]);
            Assert.Equal("8", httpContext.Response.Headers["X-Total-Filtered"]);
            Assert.Equal("4", httpContext.Response.Headers["X-Per-Page"]);
            Assert.Equal("1", httpContext.Response.Headers["X-Page"]);
            Assert.Equal("2", httpContext.Response.Headers["X-Total-Pages"]);
        }

        [Fact(DisplayName = "Проверка постраничной навигации для 4/12 записей, страница 2.")]
        public void ShouldProperlyReturnDataWith4of12Page2()
        {
            var paging = new Models.PagingModel { Page = 2, PerPage = 4 };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = GenerateBuilds(12);
            var result = builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToList();

            Assert.Equal(4, result.Count);

            var linksHeader = (string)httpContext.Response.Headers["Link"];

            Assert.False(string.IsNullOrEmpty(linksHeader));

            var pageLinks = linksHeader.Replace("\",", "\"|").Split('|');

            Assert.Equal(4, pageLinks.Length);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=1>; rel=\"first\"", pageLinks[0]);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=1>; rel=\"prev\"", pageLinks[1]);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=3>; rel=\"next\"", pageLinks[2]);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=3>; rel=\"last\"", pageLinks[3]);

            Assert.Equal("12", httpContext.Response.Headers["X-Total"]);
            Assert.Equal("12", httpContext.Response.Headers["X-Total-Filtered"]);
            Assert.Equal("4", httpContext.Response.Headers["X-Per-Page"]);
            Assert.Equal("2", httpContext.Response.Headers["X-Page"]);
            Assert.Equal("3", httpContext.Response.Headers["X-Total-Pages"]);
        }

        [Fact(DisplayName = "Проверка постраничной навигации с ответом DataTables для 4/12 записей, страница 2.")]
        public async Task ShouldProperlyReturnDataTablesWith4of12Page2()
        {
            var paging = new Models.PagingModel { Page = 2, PerPage = 4 };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = new TestAsyncEnumerable<Build>(
                GenerateBuilds(12));
            var result = await builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToDataTablesResponseAsync(httpContext);

            Assert.Equal(4, result.Data.Count());

            var linksHeader = (string)httpContext.Response.Headers["Link"];

            Assert.False(string.IsNullOrEmpty(linksHeader));

            var pageLinks = linksHeader.Replace("\",", "\"|").Split('|');

            Assert.Equal(4, pageLinks.Length);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=1>; rel=\"first\"", pageLinks[0]);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=1>; rel=\"prev\"", pageLinks[1]);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=3>; rel=\"next\"", pageLinks[2]);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=3>; rel=\"last\"", pageLinks[3]);

            Assert.Equal("12", httpContext.Response.Headers["X-Total"]);
            Assert.Equal("12", httpContext.Response.Headers["X-Total-Filtered"]);
            Assert.Equal("4", httpContext.Response.Headers["X-Per-Page"]);
            Assert.Equal("2", httpContext.Response.Headers["X-Page"]);
            Assert.Equal("3", httpContext.Response.Headers["X-Total-Pages"]);
        }

        [Fact(DisplayName = "Проверка постраничной навигации для Page > TotalPages. 4/8 записей, страница 1.")]
        public void ShouldProperlyReturnDataWithPageMoreThanTotalPages()
        {
            var paging = new Models.PagingModel { Page = 10, PerPage = 4 };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = GenerateBuilds(8);
            var result = builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToList();

            Assert.Empty(result);

            var linksHeader = (string)httpContext.Response.Headers["Link"];

            Assert.False(string.IsNullOrEmpty(linksHeader));

            var pageLinks = linksHeader.Replace("\",", "\"|").Split('|');

            Assert.Equal(2, pageLinks.Length);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=1>; rel=\"first\"", pageLinks[0]);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=9>; rel=\"prev\"", pageLinks[1]);

            Assert.Equal("8", httpContext.Response.Headers["X-Total"]);
            Assert.Equal("8", httpContext.Response.Headers["X-Total-Filtered"]);
            Assert.Equal("4", httpContext.Response.Headers["X-Per-Page"]);
            Assert.Equal("10", httpContext.Response.Headers["X-Page"]);
            Assert.Equal("2", httpContext.Response.Headers["X-Total-Pages"]);
        }

        [Fact(DisplayName = "Проверка постраничной навигации с ответом DataTables для Page > TotalPages. 4/8 записей, страница 1.")]
        public async Task ShouldProperlyReturnDataTablesWithPageMoreThanTotalPages()
        {
            var paging = new Models.PagingModel { Page = 10, PerPage = 4 };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = new TestAsyncEnumerable<Build>(
                GenerateBuilds(8));
            var result = await builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToDataTablesResponseAsync(httpContext);

            Assert.Empty(result.Data);

            var linksHeader = (string)httpContext.Response.Headers["Link"];

            Assert.False(string.IsNullOrEmpty(linksHeader));

            var pageLinks = linksHeader.Replace("\",", "\"|").Split('|');

            Assert.Equal(2, pageLinks.Length);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=1>; rel=\"first\"", pageLinks[0]);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=9>; rel=\"prev\"", pageLinks[1]);

            Assert.Equal("8", httpContext.Response.Headers["X-Total"]);
            Assert.Equal("8", httpContext.Response.Headers["X-Total-Filtered"]);
            Assert.Equal("4", httpContext.Response.Headers["X-Per-Page"]);
            Assert.Equal("10", httpContext.Response.Headers["X-Page"]);
            Assert.Equal("2", httpContext.Response.Headers["X-Total-Pages"]);
        }

        [Fact(DisplayName = "Проверка постраничной навигации для Page = -1. 4/8 записей, страница 1.")]
        public void ShouldProperlyReturnDataWithPageMinusOne()
        {
            var paging = new Models.PagingModel { Page = 2, PerPage = -1 };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = GenerateBuilds(8);
            var result = builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToList();

            Assert.Equal(8, result.Count);

            var linksHeader = (string)httpContext.Response.Headers["Link"];

            Assert.True(string.IsNullOrEmpty(linksHeader));

            var pageLinks = linksHeader.Replace("\",", "\"|").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            Assert.Empty(pageLinks);

            Assert.Equal("8", httpContext.Response.Headers["X-Total"]);
            Assert.Equal("8", httpContext.Response.Headers["X-Total-Filtered"]);
            Assert.Equal("-1", httpContext.Response.Headers["X-Per-Page"]);
            Assert.Equal("1", httpContext.Response.Headers["X-Page"]);
            Assert.Equal("1", httpContext.Response.Headers["X-Total-Pages"]);
        }

        [Fact(DisplayName = "Проверка постраничной навигации c ответом DataTables для Page = -1. 4/8 записей, страница 1.")]
        public async Task ShouldProperlyReturnDataTablesWithPageMinusOne()
        {
            var paging = new Models.PagingModel { Page = 2, PerPage = -1 };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = new TestAsyncEnumerable<Build>(
                GenerateBuilds(8));
            var result = await builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToDataTablesResponseAsync(httpContext);

            Assert.Equal(8, result.Data.Count());

            var linksHeader = (string)httpContext.Response.Headers["Link"];

            Assert.True(string.IsNullOrEmpty(linksHeader));

            var pageLinks = linksHeader.Replace("\",", "\"|").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            Assert.Empty(pageLinks);

            Assert.Equal("8", httpContext.Response.Headers["X-Total"]);
            Assert.Equal("8", httpContext.Response.Headers["X-Total-Filtered"]);
            Assert.Equal("-1", httpContext.Response.Headers["X-Per-Page"]);
            Assert.Equal("1", httpContext.Response.Headers["X-Page"]);
            Assert.Equal("1", httpContext.Response.Headers["X-Total-Pages"]);
        }

        [Fact(DisplayName = "Проверка постраничной навигации для Page <= 0. 4/8 записей, страница 1.")]
        public void ShouldProperlyReturnDataWithPageLessThanZero()
        {
            var paging = new Models.PagingModel { Page = -2, PerPage = 4 };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = GenerateBuilds(8);
            var result = builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToList();

            Assert.Empty(result);

            var linksHeader = (string)httpContext.Response.Headers["Link"];

            Assert.False(string.IsNullOrEmpty(linksHeader));

            var pageLinks = linksHeader.Replace("\",", "\"|").Split('|');

            Assert.Equal(2, pageLinks.Length);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=-1>; rel=\"next\"", pageLinks[0]);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=2>; rel=\"last\"", pageLinks[1]);

            Assert.Equal("8", httpContext.Response.Headers["X-Total"]);
            Assert.Equal("8", httpContext.Response.Headers["X-Total-Filtered"]);
            Assert.Equal("4", httpContext.Response.Headers["X-Per-Page"]);
            Assert.Equal("-2", httpContext.Response.Headers["X-Page"]);
            Assert.Equal("2", httpContext.Response.Headers["X-Total-Pages"]);
        }

        [Fact(DisplayName = "Проверка постраничной навигации c ответом DataTables для Page <= 0. 4/8 записей, страница 1.")]
        public async Task ShouldProperlyReturnDataTablesWithPageLessThanZero()
        {
            var paging = new Models.PagingModel { Page = -2, PerPage = 4 };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = new TestAsyncEnumerable<Build>(
                GenerateBuilds(8));
            var result = await builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToDataTablesResponseAsync(httpContext);

            Assert.Empty(result.Data);

            var linksHeader = (string)httpContext.Response.Headers["Link"];

            Assert.False(string.IsNullOrEmpty(linksHeader));

            var pageLinks = linksHeader.Replace("\",", "\"|").Split('|');

            Assert.Equal(2, pageLinks.Length);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=-1>; rel=\"next\"", pageLinks[0]);
            Assert.Equal("<http://localhost:5005/api/builds/byjobids?jobs=1,3&perPage=4&page=2>; rel=\"last\"", pageLinks[1]);

            Assert.Equal("8", httpContext.Response.Headers["X-Total"]);
            Assert.Equal("8", httpContext.Response.Headers["X-Total-Filtered"]);
            Assert.Equal("4", httpContext.Response.Headers["X-Per-Page"]);
            Assert.Equal("-2", httpContext.Response.Headers["X-Page"]);
            Assert.Equal("2", httpContext.Response.Headers["X-Total-Pages"]);
        }

        [Fact(DisplayName = "Проверка фильтрации элементов по полю paging.Search по полю типа string")]
        public void ShouldProperlySearchRecordsInListByStringField()
        {
            var paging = new Models.PagingModel { Page = 1, PerPage = 10, Search = "3" };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = GenerateBuilds(8);
            var result = builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToList();

            Assert.Single(result);
            Assert.Equal(3, result[0].Id);
        }

        [Fact(DisplayName = "Проверка фильтрации c ответом DataTables элементов по полю paging.Search по полю типа string")]
        public async Task ShouldProperlySearchDataTablesRecordsInListByStringField()
        {
            var paging = new Models.PagingModel { Page = 1, PerPage = 10, Search = "3" };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = new TestAsyncEnumerable<Build>(
                GenerateBuilds(8));
            var result = await builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToDataTablesResponseAsync(httpContext);

            Assert.Single(result.Data);
            Assert.Equal(3, result.Data.First().Id);
        }

        [Fact(DisplayName = "Проверка фильтрации элементов по полю paging.Search по полю типа int")]
        public void ShouldProperlySearchRecordsInListByIntField()
        {
            var paging = new Models.PagingModel { Page = 1, PerPage = 10, Search = "3" };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = GenerateBuilds(8);
            foreach (var build in builds)
                build.Name = "build";

            var result = builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToList();

            Assert.Single(result);
            Assert.Equal(3, result[0].Id);
        }

        [Fact(DisplayName = "Проверка фильтрации c ответом DataTables элементов по полю paging.Search по полю типа int")]
        public async Task ShouldProperlySearchDataTablesRecordsInListByIntField()
        {
            var paging = new Models.PagingModel { Page = 1, PerPage = 10, Search = "3" };

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");

            var builds = GenerateBuilds(8);
            foreach (var build in builds)
                build.Name = "build";
            var asyncBuilds = new TestAsyncEnumerable<Build>(builds);

            var result = await asyncBuilds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToDataTablesResponseAsync(httpContext);

            Assert.Single(result.Data);
            Assert.Equal(3, result.Data.First().Id);
        }

        [Fact(DisplayName = "Проверка фильтрации элементов по полю paging.Search с полями исключенными из поиска")]
        public void ShouldProperlySearchRecordsInListWithExcludeSearchPropertiesDataTable()
        {
            var dataTable = new DataTable
            {
                Page = 1,
                Length = 10,
                Search = new DataSearch { Value = "3" }
            };

            var httpContext = CreateDefaultHttpContext();

            var builds = Enumerable.Range(0, 10).Select(x => new Build { Id = x, Name = $"TestName{x + 1}" });
            var result = builds
                .AsQueryable()
                .WithPaging(dataTable, httpContext, x => x.Id)
                .ToList();

            Assert.Equal(2, result.Count);

            dataTable.Columns = new List<DataColumn>
            {
                new DataColumn { Data="id",Searchable=false}
            };

            httpContext = CreateDefaultHttpContext();
            result = builds
                .AsQueryable()
                .WithPaging(dataTable, httpContext, x => x.Id)
                .ToList();

            Assert.Single(result);
            Assert.Contains('3', result[0].Name);
        }

        [Fact(DisplayName = "Проверка фильтрации элементов по полю paging.Search с полями исключенными из поиска")]
        public void ShouldProperlySearchRecordsInListWithExcludeSearchProperties()
        {
            var paging = new PagingModel { Page = 1, PerPage = 10, Search = "3" };

            var httpContext = CreateDefaultHttpContext();

            var builds = Enumerable.Range(0, 10).Select(x => new Build { Id = x, Name = $"TestName{x + 1}" }).Union(new[] { new Build { Id = 11, Name = null } });
            var result = builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id)
                .ToList();

            Assert.Equal(2, result.Count);

            httpContext = CreateDefaultHttpContext();

            result = builds
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id, null, SearchType.Exclude, 3, x => x.Id, x => x.SubBuild.SubBuild.Name)
                .ToList();

            Assert.Single(result);
            Assert.Contains('3', result[0].Name);
        }

        static DefaultHttpContext CreateDefaultHttpContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost", 5005);
            httpContext.Request.Path = "/api/builds/byjobids";
            httpContext.Request.QueryString = new QueryString("?jobs=1,3");
            return httpContext;
        }
    }
}