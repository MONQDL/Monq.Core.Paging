namespace Monq.Core.Paging.Models.DataTable
{
    public class DataColumn
    {
        public bool Searchable { get; set; }

        public bool Orderable { get; set; }

        public string? Name { get; set; }

        public string? Data { get; set; }

        public DataSearch? Search { get; set; }
    }
}
