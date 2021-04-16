namespace Monq.Core.Paging.Models.DataTable
{
    public class OrderColumn
    {
        /// <summary>
        /// Номер колонки, по которой проходит сортировка.
        /// </summary>
        public int Column { get; set; } = 0;

        /// <summary>
        /// Направление сортировки.
        /// </summary>
        public string Dir { get; set; } = "asc";
    }
}
