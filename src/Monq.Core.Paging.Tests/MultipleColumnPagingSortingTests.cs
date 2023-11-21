using Microsoft.AspNetCore.Http;
using Monq.Core.Paging.Extensions;
using Monq.Core.Paging.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Monq.Core.Paging.Tests;

public class MultipleColumnPagingSortingTests
{
    [Fact(DisplayName = "Checking a sort by multiple columns. Checking the first column only (ASC).")]
    public void ShouldProperlySortFirstPropertyAsc()
    {
        var paging = new Models.PagingModel
        {
            Page = 1,
            PerPage = 10,
            SortCols = new[] { new SortColParameter { ColName = "FirstCol", Dir = "asc" } },
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5005);
        httpContext.Request.Path = "/api/levels";

        var data = SeedData();
        var result = data
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id);

        Assert.Equal(8, result.Count());

        var entity1 = result.FirstOrDefault();
        var entity2 = result.Skip(1).FirstOrDefault();
        var entity3 = result.Skip(2).FirstOrDefault();
        var entity4 = result.Skip(3).FirstOrDefault();
        var entity5 = result.Skip(4).FirstOrDefault();
        var entity6 = result.Skip(5).FirstOrDefault();
        var entity7 = result.Skip(6).FirstOrDefault();
        var entity8 = result.Skip(7).FirstOrDefault();

        Assert.Equal("first-01", entity1.FirstCol);
        Assert.Equal("first-01", entity2.FirstCol);
        Assert.Equal("first-01", entity3.FirstCol);
        Assert.Equal("first-01", entity4.FirstCol);

        Assert.Equal("first-02", entity5.FirstCol);
        Assert.Equal("first-02", entity6.FirstCol);
        Assert.Equal("first-02", entity7.FirstCol);
        Assert.Equal("first-02", entity8.FirstCol);
    }

    [Fact(DisplayName = "Checking a sort by multiple columns. Checking the first column only (DESC).")]
    public void ShouldProperlySortFirstPropertyDesc()
    {
        var paging = new Models.PagingModel
        {
            Page = 1,
            PerPage = 10,
            SortCols = new[] { new SortColParameter { ColName = "FirstCol", Dir = "desc" } },
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5005);
        httpContext.Request.Path = "/api/levels";

        var data = SeedData();
        var result = data
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id);

        Assert.Equal(8, result.Count());

        var entity1 = result.FirstOrDefault();
        var entity2 = result.Skip(1).FirstOrDefault();
        var entity3 = result.Skip(2).FirstOrDefault();
        var entity4 = result.Skip(3).FirstOrDefault();
        var entity5 = result.Skip(4).FirstOrDefault();
        var entity6 = result.Skip(5).FirstOrDefault();
        var entity7 = result.Skip(6).FirstOrDefault();
        var entity8 = result.Skip(7).FirstOrDefault();

        Assert.Equal("first-02", entity1.FirstCol);
        Assert.Equal("first-02", entity2.FirstCol);
        Assert.Equal("first-02", entity3.FirstCol);
        Assert.Equal("first-02", entity4.FirstCol);

        Assert.Equal("first-01", entity5.FirstCol);
        Assert.Equal("first-01", entity6.FirstCol);
        Assert.Equal("first-01", entity7.FirstCol);
        Assert.Equal("first-01", entity8.FirstCol);
    }

    [Fact(DisplayName = "Checking a sort by multiple columns. Checking three columns (ASC).")]
    public void ShouldProperlySortThreePropertiesAsc()
    {
        var paging = new Models.PagingModel
        {
            Page = 1,
            PerPage = 10,
            SortCols = new[]
            {
                new SortColParameter { ColName = "FirstCol", Dir = "asc"},
                new SortColParameter { ColName = "SecondCol", Dir = "asc"},
                new SortColParameter { ColName = "ThirdCol", Dir = "asc"},
            },
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5005);
        httpContext.Request.Path = "/api/levels";

        var data = SeedData();
        var result = data
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id);

        Assert.Equal(8, result.Count());

        var entity1 = result.FirstOrDefault();
        var entity2 = result.Skip(1).FirstOrDefault();
        var entity3 = result.Skip(2).FirstOrDefault();
        var entity4 = result.Skip(3).FirstOrDefault();
        var entity5 = result.Skip(4).FirstOrDefault();
        var entity6 = result.Skip(5).FirstOrDefault();
        var entity7 = result.Skip(6).FirstOrDefault();
        var entity8 = result.Skip(7).FirstOrDefault();

        Assert.Equal(1, entity1.Id);
        Assert.Equal(2, entity2.Id);
        Assert.Equal(3, entity3.Id);
        Assert.Equal(4, entity4.Id);
        Assert.Equal(5, entity5.Id);
        Assert.Equal(6, entity6.Id);
        Assert.Equal(7, entity7.Id);
        Assert.Equal(8, entity8.Id);
    }

    [Fact(DisplayName = "Checking a sort by multiple columns. Checking three columns (DESC).")]
    public void ShouldProperlySortThreePropertiesDesc()
    {
        var paging = new Models.PagingModel
        {
            Page = 1,
            PerPage = 10,
            SortCols = new[]
            {
                new SortColParameter { ColName = "FirstCol", Dir = "desc"},
                new SortColParameter { ColName = "SecondCol", Dir = "desc"},
                new SortColParameter { ColName = "ThirdCol", Dir = "desc"},
            },
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5005);
        httpContext.Request.Path = "/api/levels";

        var data = SeedData();
        var result = data
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id);

        Assert.Equal(8, result.Count());

        var entity1 = result.FirstOrDefault();
        var entity2 = result.Skip(1).FirstOrDefault();
        var entity3 = result.Skip(2).FirstOrDefault();
        var entity4 = result.Skip(3).FirstOrDefault();
        var entity5 = result.Skip(4).FirstOrDefault();
        var entity6 = result.Skip(5).FirstOrDefault();
        var entity7 = result.Skip(6).FirstOrDefault();
        var entity8 = result.Skip(7).FirstOrDefault();

        Assert.Equal(8, entity1.Id);
        Assert.Equal(7, entity2.Id);
        Assert.Equal(6, entity3.Id);
        Assert.Equal(5, entity4.Id);
        Assert.Equal(4, entity5.Id);
        Assert.Equal(3, entity6.Id);
        Assert.Equal(2, entity7.Id);
        Assert.Equal(1, entity8.Id);
    }

    [Fact(DisplayName = "Checking a sort by multiple columns. Checking first and second - (ASC), third - (DESC).")]
    public void ShouldProperlySortThreePropertiesAscDesc()
    {
        var paging = new Models.PagingModel
        {
            Page = 1,
            PerPage = 10,
            SortCols = new[]
            {
                new SortColParameter { ColName = "FirstCol", Dir = "asc"},
                new SortColParameter { ColName = "SecondCol", Dir = "asc"},
                new SortColParameter { ColName = "ThirdCol", Dir = "desc"},
            },
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5005);
        httpContext.Request.Path = "/api/levels";

        var data = SeedData();
        var result = data
                .AsQueryable()
                .WithPaging(paging, httpContext, x => x.Id);

        Assert.Equal(8, result.Count());

        var entity1 = result.FirstOrDefault();
        var entity2 = result.Skip(1).FirstOrDefault();
        var entity3 = result.Skip(2).FirstOrDefault();
        var entity4 = result.Skip(3).FirstOrDefault();
        var entity5 = result.Skip(4).FirstOrDefault();
        var entity6 = result.Skip(5).FirstOrDefault();
        var entity7 = result.Skip(6).FirstOrDefault();
        var entity8 = result.Skip(7).FirstOrDefault();

        Assert.Equal(2, entity1.Id);
        Assert.Equal(1, entity2.Id);
        Assert.Equal(4, entity3.Id);
        Assert.Equal(3, entity4.Id);
        Assert.Equal(6, entity5.Id);
        Assert.Equal(5, entity6.Id);
        Assert.Equal(8, entity7.Id);
        Assert.Equal(7, entity8.Id);
    }

    static IEnumerable<TestEntity> SeedData()
    {
        var data = new List<TestEntity>
        {
            new () { Id = 7, FirstCol = "first-02", SecondCol = "second-02", ThirdCol = "third-01" },
            new () { Id = 4, FirstCol = "first-01", SecondCol = "second-02", ThirdCol = "third-02" },
            new () { Id = 2, FirstCol = "first-01", SecondCol = "second-01", ThirdCol = "third-02" },
            new () { Id = 3, FirstCol = "first-01", SecondCol = "second-02", ThirdCol = "third-01" },
            new () { Id = 8, FirstCol = "first-02", SecondCol = "second-02", ThirdCol = "third-02" },
            new () { Id = 5, FirstCol = "first-02", SecondCol = "second-01", ThirdCol = "third-01" },
            new () { Id = 1, FirstCol = "first-01", SecondCol = "second-01", ThirdCol = "third-01" },
            new () { Id = 6, FirstCol = "first-02", SecondCol = "second-01", ThirdCol = "third-02" },
        };
        return data;
    }
}

public class TestEntity
{
    public int Id { get; set; }
    public string FirstCol { get; set; }
    public string SecondCol { get; set; }
    public string ThirdCol { get; set; }
}
