using Monq.Core.Paging.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Monq.Core.Paging.Models;

/// <summary>
/// Параметры поиска.
/// </summary>
public class Searching
{
    public Searching()
    {
    }

    public Searching(SearchType searchType, IEnumerable<string>? propNames = null)
    {
        SearchType = searchType;
        Properties = propNames?.Select(x => x.ToLower());
    }

    public Searching(SearchType searchType, params Expression<Func<object, object>>[] propNames)
    {
        SearchType = searchType;
        Properties = propNames?.Select(x => x.GetFullPropertyName().ToLower());
    }

    public static Searching CreateSearching<T>(SearchType searchType, int depth, params Expression<Func<T, object>>[] propNames) =>
        new Searching(searchType, propNames.Select(x => x.GetFullPropertyName())) { Depth = depth };

    public bool InSearch(string propName) =>
        SearchType switch
        {
            SearchType.Include => Properties?.Any() == true && Properties.Contains(propName.ToLower()),
            SearchType.Exclude => Properties?.Any() != true || !Properties.Contains(propName.ToLower()),
            _ => true
        };

    /// <summary>
    /// Глубина поиска.
    /// </summary>
    public int Depth { get; set; } = 1;

    /// <summary>
    /// Свойства, которые необходимо включить/исключить в поиск.
    /// </summary>
    public IEnumerable<string>? Properties { get; protected set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    public SearchType SearchType { get; protected set; } = SearchType.None;
}

/// <summary>
/// Тип поиска
/// </summary>
public enum SearchType
{
    /// <summary>
    /// Поиск по всем полям.
    /// </summary>
    None,

    /// <summary>
    /// Поиск только по полям указанных в Properties.
    /// </summary>
    Include,

    /// <summary>
    /// Поиск по всем полям кроме тех что указаны в Properties.
    /// </summary>
    Exclude
}