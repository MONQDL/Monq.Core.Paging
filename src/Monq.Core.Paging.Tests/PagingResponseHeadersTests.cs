using Microsoft.AspNetCore.Http;
using Monq.Core.Paging.Extensions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Monq.Core.Paging.Tests;

public class PagingResponseHeadersTests
{
    class Build
    {
        public int Id { get; set; }
    }

    IEnumerable<Build> GenerateBuilds(int count = 1)
    {
        var builds = new List<Build>();
        for (var i = 0; i < count; i++)
        {
            builds.Add(new Build { Id = i });
        }

        return builds;
    }

    [Fact(DisplayName = "Проверка корректной обработки заголовков HTTP-ответа.")]
    public void ShouldProperlyParsePagingResponseHeaders()
    {
        var paging = new Models.PagingModel { Page = 1, PerPage = 4 };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5005);
        httpContext.Request.Path = "/api/builds/byjobids";
        httpContext.Request.QueryString = new QueryString("?jobs=1,3");

        var builds = GenerateBuilds(8);
        var result = builds
            .AsQueryable()
            .WithPaging(paging, httpContext, x => x.Id);

        Assert.Equal(4, result.Count());

        var pagingData = httpContext.Response.GetPagingData();

        Assert.Equal(8, pagingData.TotalRecords);
        Assert.Equal(8, pagingData.TotalFilteredRecords);
        Assert.Equal(4, pagingData.PerPage);
        Assert.Equal(1, pagingData.Page);
        Assert.Equal(2, pagingData.TotalPages);
    }
}
