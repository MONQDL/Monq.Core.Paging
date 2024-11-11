using Microsoft.AspNetCore.Http;
using Monq.Core.Paging.Extensions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Monq.Core.Paging.Tests;

public class MultiLevelPagingSortingTests
{
    static IEnumerable<FirstLevel> SeedData()
    {
        var data = new List<FirstLevel>
        {
            new FirstLevel
            {
                Id = 1,
                Name = "FirstLevel1",
                SecondLevel = new SecondLevel
                {
                    ThirdLevel = new ThirdLevel
                    {
                        Id = 3,
                        Name = "ThirdLevel3"
                    }
                }
            },
            new FirstLevel
            {
                Id = 2,
                Name = "FirstLevel2",
                SecondLevel = new SecondLevel
                {
                    Name = "SecondLeve3",
                    ThirdLevel = new ThirdLevel
                    {
                        Id = 1,
                        Name = "ThirdLevel1",
                        FourthLevel = new FourthLevel
                        {
                            Enabled = false,
                            Name = "FourthLevel1",
                            Id = 1
                        }
                    }
                }
            },
            new FirstLevel
            {
                Id = 3,
                Name = "FirstLevel3",
                SecondLevel = new SecondLevel
                {
                    Name = "SecondLeve2",
                    ThirdLevel = new ThirdLevel
                    {
                        Id = 5,
                        Name = "ThirdLevel5",
                        FourthLevel = new FourthLevel
                        {
                            Enabled = true,
                            Name = "FourthLevel3",
                            Id = 2
                        }
                    }
                }
            },
            new FirstLevel
            {
                Id = 4,
                Name = "FirstLevel4",
                SecondLevel = new SecondLevel
                {
                    Id = 125,
                    Name = "SecondLevel",
                    ThirdLevel = new ThirdLevel
                    {
                        Id = 5,
                        Name = "ThirdLevel4",
                        FourthLevel = new FourthLevel
                        {
                            Enabled = true,
                            Name = "FourthLevel2",
                            Id = 3
                        }
                    }
                }
            }
        };
        return data;
    }

    [Fact(DisplayName = "Проверка многоуровневой сортировки записей. Проверка первого уровня, сортировка по наименованию (DESC).")]
    public void ShouldProperlySortFirstLevelByName()
    {
        var paging = new Models.PagingModel
        {
            Page = 1,
            PerPage = 10,
            SortCol = "Name",
            SortDir = "desc"
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5005);
        httpContext.Request.Path = "/api/levels";

        var levels = SeedData();
        var result = levels
            .AsQueryable()
            .WithPaging(paging, httpContext, x => x.Id);

        Assert.Equal(4, result.Count());

        var firstObject = result.FirstOrDefault();
        var secondObject = result.Skip(1).FirstOrDefault();
        var thirdObject = result.Skip(2).FirstOrDefault();
        var fourthObject = result.Skip(3).FirstOrDefault();

        Assert.Equal(4, firstObject.Id);
        Assert.Equal(3, secondObject.Id);
        Assert.Equal(2, thirdObject.Id);
        Assert.Equal(1, fourthObject.Id);
    }

    [Fact(DisplayName = "Проверка многоуровневой сортировки записей. Проверка второго уровня, сортировка по наименованию (DESC).")]
    public void ShouldProperlySortSecondLevelByName()
    {
        var paging = new Models.PagingModel
        {
            Page = 1,
            PerPage = 10,
            SortCol = "SecondLevel.Name",
            SortDir = "desc"
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5005);
        httpContext.Request.Path = "/api/levels";

        var levels = SeedData();
        var result = levels
            .AsQueryable()
            .WithPaging(paging, httpContext, x => x.Id);

        Assert.Equal(4, result.Count());

        var firstObject = result.FirstOrDefault();
        var secondObject = result.Skip(1).FirstOrDefault();
        var thirdObject = result.Skip(2).FirstOrDefault();
        var fourthObject = result.Skip(3).FirstOrDefault();

        Assert.Equal(4, firstObject.Id);
        Assert.Equal(2, secondObject.Id);
        Assert.Equal(3, thirdObject.Id);
        Assert.Equal(1, fourthObject.Id);
    }

    [Fact(DisplayName = "Проверка многоуровневой сортировки записей. Проверка третьего уровня, сортировка по наименованию (ASC).")]
    public void ShouldProperlySortThirdLevelByName()
    {
        var paging = new Models.PagingModel
        {
            Page = 1,
            PerPage = 10,
            SortCol = "SecondLevel.ThirdLevel.Name ",
            SortDir = "asc"
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5005);
        httpContext.Request.Path = "/api/levels";

        var levels = SeedData();
        var result = levels
            .AsQueryable()
            .WithPaging(paging, httpContext, x => x.Id);

        Assert.Equal(4, result.Count());

        var firstObject = result.FirstOrDefault();
        var secondObject = result.Skip(1).FirstOrDefault();
        var thirdObject = result.Skip(2).FirstOrDefault();
        var fourthObject = result.Skip(3).FirstOrDefault();

        Assert.Equal(2, firstObject.Id);
        Assert.Equal(1, secondObject.Id);
        Assert.Equal(4, thirdObject.Id);
        Assert.Equal(3, fourthObject.Id);
    }

    [Fact(DisplayName = "Проверка многоуровневой сортировки записей. Проверка четвертого уровня, сортировка по наименованию (ASC).")]
    public void ShouldProperlySortFourthLevelByName()
    {
        var paging = new Models.PagingModel
        {
            Page = 1,
            PerPage = 10,
            SortCol = "SecondLevel.ThirdLevel.FourthLevel.Name",
            SortDir = "asc"
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5005);
        httpContext.Request.Path = "/api/levels";

        var levels = SeedData();
        var result = levels
            .AsQueryable()
            .WithPaging(paging, httpContext, x => x.Id);

        Assert.Equal(4, result.Count());

        var firstObject = result.FirstOrDefault();
        var secondObject = result.Skip(1).FirstOrDefault();
        var thirdObject = result.Skip(2).FirstOrDefault();
        var fourthObject = result.Skip(3).FirstOrDefault();

        Assert.Equal(1, firstObject.Id);
        Assert.Equal(2, secondObject.Id);
        Assert.Equal(4, thirdObject.Id);
        Assert.Equal(3, fourthObject.Id);
    }

    [Fact(DisplayName = "Проверка многоуровневой сортировки записей. Проверка четвертого уровня, сортировка по enabled (ASC).")]
    public void ShouldProperlySortFourthLevelByEnabled()
    {
        var paging = new Models.PagingModel
        {
            Page = 1,
            PerPage = 10,
            SortCol = "SecondLevel.ThirdLevel.FourthLevel.Enabled",
            SortDir = "asc"
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5005);
        httpContext.Request.Path = "/api/levels";

        var levels = SeedData();
        var result = levels
            .AsQueryable()
            .WithPaging(paging, httpContext, x => x.Id);

        Assert.Equal(4, result.Count());

        var firstObject = result.FirstOrDefault();
        var secondObject = result.Skip(1).FirstOrDefault();
        var thirdObject = result.Skip(2).FirstOrDefault();
        var fourthObject = result.Skip(3).FirstOrDefault();

        Assert.Equal(1, firstObject.Id);
        Assert.Equal(2, secondObject.Id);
        Assert.Equal(3, thirdObject.Id);
        Assert.Equal(4, fourthObject.Id);
    }
}

public class FirstLevel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public SecondLevel SecondLevel { get; set; }
}

public class SecondLevel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ThirdLevel ThirdLevel { get; set; }
}

public class ThirdLevel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public FourthLevel FourthLevel { get; set; }
}

public class FourthLevel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Enabled { get; set; }
}