using System.ComponentModel.DataAnnotations;

namespace Monq.Core.Paging.Models
{
    /// <summary>
    /// Модель данных постраничной навигации, применяемая в СМ.
    /// </summary>
    public class PagingModel
    {
        /// <summary>
        /// Количество записей на 1 страницу. По умолчанию
        /// запрашиваются все доступные записи.
        /// </summary>
        public const int DEFAULT_ITEMS_PER_PAGE = 1000;
        public const int ALL_ITEMS_PER_PAGE = -1;

        /// <summary>
        /// Название поля, по которому выполняется сортировка.
        /// </summary>
        public string? SortCol { get; set; }

        /// <summary>
        /// Направление сортировки. Возможные значения asc, desc.
        /// </summary>
        public string? SortDir { get; set; }

        /// <summary>
        /// Номер страницы.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Количество записей, которое требуется пропустить.
        /// </summary>
        public int Skip { get; set; } = 0;

        /// <summary>
        /// Строка поиска.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Количество записей на 1 страницу. Если -1 - получить все.
        /// </summary>
        [Range(-1, int.MaxValue)]
        public int PerPage { get; set; } = DEFAULT_ITEMS_PER_PAGE;
    }
}
