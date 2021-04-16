namespace Monq.Core.Paging
{
    public static class Constants
    {
        public static class Headers
        {
            public const string Link = "Link";

            public const string TotalRecords = "X-Total";
            public const string TotalFilteredRecords = "X-Total-Filtered";
            public const string PerPage = "X-Per-Page";
            public const string Page = "X-Page";
            public const string TotalPages = "X-Total-Pages";
        }
    }
}
