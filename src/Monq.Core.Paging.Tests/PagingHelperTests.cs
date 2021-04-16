using Monq.Core.Paging.Helpers;
using System;
using Xunit;

namespace Monq.Core.Paging.Tests
{
    public class PagingHelperTests
    {
        [Fact]
        public void ShouldProperlyReturnLinksPage2Of4()
        {
            var links = PagingHelper.GetLinks(new Uri("http://localhost.com/api/events"), 32, 2, 10);

            var result = links.Split(',');
            Assert.Equal(4, result.Length);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=1>; rel=\"first\"", result[0]);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=1>; rel=\"prev\"", result[1]);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=3>; rel=\"next\"", result[2]);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=4>; rel=\"last\"", result[3]);
        }

        [Fact]
        public void ShouldProperlyReturnLinksPage1Of4()
        {
            var links = PagingHelper.GetLinks(new Uri("http://localhost.com/api/events"), 32, 1, 10);

            var result = links.Split(',');
            Assert.Equal(2, result.Length);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=2>; rel=\"next\"", result[0]);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=4>; rel=\"last\"", result[1]);
        }

        [Fact]
        public void ShouldProperlyReturnLinksPage4Of4()
        {
            var links = PagingHelper.GetLinks(new Uri("http://localhost.com/api/events"), 32, 4, 10);

            var result = links.Split(',');
            Assert.Equal(2, result.Length);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=1>; rel=\"first\"", result[0]);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=3>; rel=\"prev\"", result[1]);
        }

        [Fact]
        public void ShouldProperlyReturnLinksPage3Of4()
        {
            var links = PagingHelper.GetLinks(new Uri("http://localhost.com/api/events"), 32, 3, 10);

            var result = links.Split(',');
            Assert.Equal(4, result.Length);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=1>; rel=\"first\"", result[0]);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=2>; rel=\"prev\"", result[1]);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=4>; rel=\"next\"", result[2]);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=4>; rel=\"last\"", result[3]);
        }

        [Fact]
        public void ShouldProperlyReturnLinksPage1Of1()
        {
            var links = PagingHelper.GetLinks(new Uri("http://localhost.com/api/events"), 30, 1, 30);

            var result = links.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Assert.Empty(result);
        }

        [Fact]
        public void ShouldProperlyReturnLinksPage1Of4WithCurrentPage0()
        {
            var links = PagingHelper.GetLinks(new Uri("http://localhost.com/api/events"), 32, 0, 10);

            var result = links.Split(',');
            Assert.Equal(2, result.Length);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=2>; rel=\"next\"", result[0]);
            Assert.Equal("<http://localhost.com/api/events?perPage=10&page=4>; rel=\"last\"", result[1]);
        }

        [Fact]
        public void ShouldProperlyReturnLinksWithExistingQuery()
        {
            var links = PagingHelper.GetLinks(new Uri("http://localhost.com/api/events?sort=Name&search=тест"), 32, 3, 10);

            var result = links.Split(',');
            Assert.Equal(4, result.Length);
            Assert.Equal("<http://localhost.com/api/events?sort=Name&search=тест&perPage=10&page=1>; rel=\"first\"", result[0]);
            Assert.Equal("<http://localhost.com/api/events?sort=Name&search=тест&perPage=10&page=2>; rel=\"prev\"", result[1]);
            Assert.Equal("<http://localhost.com/api/events?sort=Name&search=тест&perPage=10&page=4>; rel=\"next\"", result[2]);
            Assert.Equal("<http://localhost.com/api/events?sort=Name&search=тест&perPage=10&page=4>; rel=\"last\"", result[3]);
        }
    }
}
