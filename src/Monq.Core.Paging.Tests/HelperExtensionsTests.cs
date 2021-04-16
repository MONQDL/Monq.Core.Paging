using Monq.Core.Paging.Extensions;
using Monq.Core.Paging.Models;
using Monq.Core.Paging.Models.DataTable;
using Xunit;

namespace Monq.Core.Paging.Tests
{
    public class HelperExtensionsTests
    {
        [Fact(DisplayName = "Проверка экранирования \"+\" в uri.")]
        public void ShouldEscapePlusSymbolInSearch()
        {
            const string defaultUrl = "http://localhost:5005/api/systems/filter";
            const string expectedUrl = "http://localhost:5005/api/systems/filter?&page=1&perPage=1&search=%2B5";

            var pagingModel = new PagingModel { PerPage = 1, Search = "+5" };

            var url = pagingModel.GetUri(defaultUrl);

            Assert.Equal(expectedUrl, url);
        }

        [Fact(DisplayName = "Проверка возвращения Url без ? от PagingModel.")]
        public void ShouldReturnUrlWithoutQuestionMarkFromPagingModel()
        {
            // TODO: Выяснить почему тут испольузется ?&
            const string defaultUrl = "http://localhost:5005/api/systems/filter";
            const string expectedUrl = "http://localhost:5005/api/systems/filter?&page=1&perPage=2&search=1";
            var pagingModel = new PagingModel { PerPage = 2, Search = "1" };

            var url = pagingModel.GetUri(defaultUrl);

            Assert.Equal(expectedUrl, url);
        }

        [Fact(DisplayName = "Проверка возвращения Url без ? от DataTable.")]
        public void ShouldReturnUrlWithoutQuestionMarkFromDataTable()
        {
            const string defaultUrl = "http://localhost:5005/api/systems/filter";
            const string expectedUrl = "http://localhost:5005/api/systems/filter?&page=1&sortCol=test&sortDir=asc&search=1";
            var pagingModel = new DataTable
            {
                Order = new[] { new OrderColumn { Column = 0, Dir = "asc" } },
                Columns = new[] { new DataColumn { Name = "test" } },
                Search = new DataSearch { Value = "1" }
            };

            var url = pagingModel.GetUri(defaultUrl);

            Assert.Equal(expectedUrl, url);
        }

        [Fact(DisplayName = "Проверка возвращения Url c ? от PagingModel.")]
        public void ShouldReturnUrlWithQuestionMarkFromPagingModel()
        {
            const string defaultUrl = "http://localhost:5005/api/systems/filter?response=json";
            const string expectedUrl = "http://localhost:5005/api/systems/filter?response=json&page=1&perPage=2&search=1";
            var pagingModel = new PagingModel { PerPage = 2, Search = "1" };

            var url = pagingModel.GetUri(defaultUrl);

            Assert.Equal(expectedUrl, url);
        }

        [Fact(DisplayName = "Проверка возвращения Url c ? от DataTable.")]
        public void ShouldReturnUrlWithQuestionMarkFromDataTable()
        {
            const string defaultUrl = "http://localhost:5005/api/systems/filter?response=json";
            const string expectedUrl = "http://localhost:5005/api/systems/filter?response=json&page=1&sortCol=test&sortDir=asc&search=1";
            var pagingModel = new DataTable
            {
                Order = new[] { new OrderColumn { Column = 0, Dir = "asc" } },
                Columns = new[] { new DataColumn { Name = "test" } },
                Search = new DataSearch { Value = "1" }
            };

            var url = pagingModel.GetUri(defaultUrl);

            Assert.Equal(expectedUrl, url);
        }
    }
}
