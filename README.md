# Monq.Core.Paging

## Change Log

See CHANGELOG.md

*English*

The library provides a model and a set of methods for working with paged requests through EntityFramework Core. It allows to generate headers for HttpContext.Response with paginated request metadata. The model supports the [DataTables format](https://datatables.net/).

## Installation

```powershell
Install-Package Monq.Core.Paging
```

## Using

The library implements the approach of using HTTP Headers to deliver meta information about a piece of data contained for a page. This approach was chosen for the sake of the "purity" of the requested HTTP response data. Thus, when designing REST interfaces, the client receives exactly the data that he requested - a list of entities in the response body. Meta-information on entities, if necessary, is taken from the HTTP response headers.

The main set of extension methods is presented in `Monq.Core.Paging.Extensions`.

### WithPaging
The main extension method for paginating requests is the `WithPaging` method.

```csharp
var result = context
    .AsQueryable()
    .WithPaging(paging, httpContext, x => x.Id) // <-- method of pagination of requests.
    .ToList();
```

The `WithPaging ()` method takes an object of the [`PagingModel` class](https://github.com/MONQDL/Monq.Core.Paging.Models/blob/master/src/Monq.Core.Paging.Models/Paging.cs ) a page-by-page request and generates a request in the database, based on the filled fields of the object. In addition, the method accepts `HttpContext` as an argument to form the response headers and adds [#http-response-headers] to the context response headers (metadata).

The method allows sorting by field, as well as searching by fields of the model with a specified depth of nesting of links.

Example of use with arrays (EntityFramework emulation)
```csharp
var paging = new Models.PagingModel
{
    Page = 1,
    PerPage = 2,
    SortCol = "Name",
    SortDir = "desc"
};

var httpContext = new DefaultHttpContext();
httpContext.Request.Scheme = "http";
httpContext.Request.Host = new HostString("localhost", 5005);
httpContext.Request.Path = "/api/levels";

var levels = new List<FirstLevel>
{
    new FirstLevel { Id = 1, Name = "FirstLevel1" },
    new FirstLevel { Id = 2, Name = "FirstLevel2" },
    new FirstLevel { Id = 3, Name = "FirstLevel3" },
    new FirstLevel { Id = 4, Name = "FirstLevel4" },
};

var result = levels
    .AsQueryable()
    .WithPaging(paging, httpContext, x => x.Id)
    .ToList();
```

### ToDataTablesResponse
Analogous to the `WithPaging` method, but used to generate responses in DataTables format.

### Methods for working with PagingModel

*`string GetUri(this PagingModel? paging, string url)`* - the method forms a query string based on the values of the `PagingModel` model object.

Пример:

```csharp
const string defaultUrl = "http://localhost:5005/api/systems/filter";
const string expectedUrl = "http://localhost:5005/api/systems/filter?&page=1&perPage=1&search=%2B5";

var pagingModel = new PagingModel { PerPage = 1, Search = "+5" };

var url = pagingModel.GetUri(defaultUrl);

Assert.Equal(expectedUrl, url);
```

*`string GetUri(this DataTable paging, string url)`* - the method forms a query string based on the values of the `DataTable` model object.

Пример:

```csharp
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
```

*`PagingModel GetPagingModel(this DataTable model)`* - method converts the `DataTable` model to` PagingModel`.

Пример:

```csharp
var dataTableModel = new DataTable
{
    Order = new[] { new OrderColumn { Column = 0, Dir = "asc" } },
    Columns = new[] { new DataColumn { Name = "test" } },
    Search = new DataSearch { Value = "1" }
};

var pagingModel = pagingModel.GetPagingModel(dataTableModel);
```

### Additional Features of the PagingModel

- When using the [`PagingModel`](https://github.com/MONQDL/Monq.Core.Paging.Models/blob/master/src/Monq.Core.Paging.Models/Paging.cs) model, keep in mind that `Skip` takes precedence over Page. If `Skip > 0`, then the operation of calculating pages taking into account `Skip` will be performed.

### Http Response Headers

When generating a response containing part of the data for the page, such a list of headers is generated

- Link is the standard [Link header](https://www.w3.org/wiki/LinkHeader).
- X-Total - total number of records in the selection.
- X-Total-Filtered - the number of records filtered in the query through the `Search` field of the `PagingModel` model.
- X-Per-Page - number of records per page. Duplicate the value of the `PerPage` field of the `PagingModel`.
- X-Page - current page number. Duplicate the value of the `Page` field of the `PagingModel` model.
- X-Total-Pages - the number of pages available for request, taking into account the search specified in the `Search` field.

---

*Русский*

Библиотека предоставляет модель и набор методов для работы с постраничными запросами через EntityFramework Core. Позволяет формировать заголовки для HttpContext.Response с метаданными постраничных запросов. Модель поддерживает работу с форматом [DataTables](https://datatables.net/).

## Установка

```powershell
Install-Package Monq.Core.Paging
```

## Использование

В библиотеке реализован подход использования HTTP Headers для доставки метаинформации по части данных, содержащейся для страницы. Такой подход выбран в угоду "чистоты" запрашиваемых данных HTTP ответа. Таким образом, при проектировании REST интерфейсов, клиент получает именно те данные, которые он запросил - список сущностей в теле ответа. Метаинформация по сущностям, при необходимости забирается из HTTP заголовков ответа.

Основной набор методов расширения представлен в `Monq.Core.Paging.Extensions`.

### WithPaging
Основным методом расширения для разбивки запросов на страницы является метод `WithPaging`.

```csharp
var result = context
    .AsQueryable()
    .WithPaging(paging, httpContext, x => x.Id) // <-- метод разбивки запросов на страницы.
    .ToList();
```

Метод `WithPaging()` принимает объект класса [`PagingModel`](https://github.com/MONQDL/Monq.Core.Paging.Models/blob/master/src/Monq.Core.Paging.Models/Paging.cs) постраничного запроса и формирует запрос в БД, на основании заполненных полей объекта. Кроме того, метод принимает `HttpContext` в качестве аргумента, для формирования заголовков ответа и формирует длбавляет в заголовки ответа контекста (метаданные)[#заголовки-http-ответа].

Метод позволяет выполнять сортировку по полю, а также поиск по полям модели с заданной глубиной вложенности связей.

Пример использования с массивами данных (эмуляция EntityFramework)
```csharp
var paging = new Models.PagingModel
{
    Page = 1,
    PerPage = 2,
    SortCol = "Name",
    SortDir = "desc"
};

var httpContext = new DefaultHttpContext();
httpContext.Request.Scheme = "http";
httpContext.Request.Host = new HostString("localhost", 5005);
httpContext.Request.Path = "/api/levels";

var levels = new List<FirstLevel>
{
    new FirstLevel { Id = 1, Name = "FirstLevel1" },
    new FirstLevel { Id = 2, Name = "FirstLevel2" },
    new FirstLevel { Id = 3, Name = "FirstLevel3" },
    new FirstLevel { Id = 4, Name = "FirstLevel4" },
};

var result = levels
    .AsQueryable()
    .WithPaging(paging, httpContext, x => x.Id)
    .ToList();
```

### ToDataTablesResponse
Аналог метода `WithPaging`, но используется для формирования ответов в формате DataTables.

### Методы работы с PagingModel

*`string GetUri(this PagingModel? paging, string url)`* - метод формирует строку запроса по значениям объекта модели `PagingModel`.

Пример:

```csharp
const string defaultUrl = "http://localhost:5005/api/systems/filter";
const string expectedUrl = "http://localhost:5005/api/systems/filter?&page=1&perPage=1&search=%2B5";

var pagingModel = new PagingModel { PerPage = 1, Search = "+5" };

var url = pagingModel.GetUri(defaultUrl);

Assert.Equal(expectedUrl, url);
```

*`string GetUri(this DataTable paging, string url)`* - метод формирует строку запроса по значениям объекта модели `DataTable`.

Пример:

```csharp
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
```

*`PagingModel GetPagingModel(this DataTable model)`* - метод преобразовывает модель `DataTable` в `PagingModel`.

Пример:

```csharp
var dataTableModel = new DataTable
{
    Order = new[] { new OrderColumn { Column = 0, Dir = "asc" } },
    Columns = new[] { new DataColumn { Name = "test" } },
    Search = new DataSearch { Value = "1" }
};

var pagingModel = pagingModel.GetPagingModel(dataTableModel);
```

### Особенности

- При использовании модели [`PagingModel`](https://github.com/MONQDL/Monq.Core.Paging.Models/blob/master/src/Monq.Core.Paging.Models/Paging.cs) следует учитывать, что `Skip` имеет больший приоритет, чем `Page`. Если `Skip > 0`, то будет выполнена операция расчета страниц с учетом `Skip`.

### Заголовки HTTP ответа

При формировании ответа, содержащего часть данных для страницы формируется такой список заголовков

- Link - стандартный заголовок [Link](https://www.w3.org/wiki/LinkHeader).
- X-Total - полное количество записей в выборке.
- X-Total-Filtered - количество записей, отфильтрованных в запросе через поле `Search` модели `PagingModel`.
- X-Per-Page - количество записей на странице. Дубль значения поля `PerPage` модели `PagingModel`.
- X-Page - номер текущей страницы. Дубль значения поля `Page` модели `PagingModel`.
- X-Total-Pages - количество станиц, доступное для запроса с учетом поиска, указанного в поле `Search`.
