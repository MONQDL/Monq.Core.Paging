namespace Monq.Core.Paging.Models;

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
