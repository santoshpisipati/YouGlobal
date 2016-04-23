namespace YG_MVC.Models
{
    public class SearchViewModel
    {
        public int CurrentPageNumber { get; set; }

        public string Industry { get; set; }

        public string Keywords { get; set; }

        public string Location { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int ResultsCount { get; set; }

        public string Role { get; set; }

        //public IList<JobPosting> SearchResults { get; set; }

        public bool ShowHotJobsOnly { get; set; }

        public bool ShowNextLink { get; set; }

        public bool ShowPreviousLink { get; set; }

        public string WorkArrangement { get; set; }
    }
}