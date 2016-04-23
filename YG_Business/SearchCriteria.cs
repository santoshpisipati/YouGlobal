using System.Collections.Generic;

namespace YG_Business
{
    public class SearchCriteria
    {
        // Methods
        public SearchCriteria()
        {
        }

        public SearchCriteria(string industry, string location, string keywords)
        {
            this.Industry = industry;
            this.Location = location;
            this.Keywords = keywords;
        }

        public SearchCriteria(string industry, string role, string location, string workArrangement, string keywords)
        {
            this.Industry = industry;
            this.Role = role;
            this.Location = location;
            this.WorkArrangement = workArrangement;
            this.Keywords = keywords;
        }

        public override string ToString()
        {
            return string.Format("SearchCriteria => [Industry: {0}, Role: {1}, Location: {2}, WorkArrangement: {3}, Keywords: {4}, ShowHotJobsOnly: {5}]", new object[] { this.Industry, this.Role, this.Location, this.WorkArrangement, this.Keywords, this.ShowHotJobsOnly });
        }

        // Properties
        public string Industry { get; set; }

        public string Keywords { get; set; }

        public string Location { get; set; }

        public string Role { get; set; }

        public bool ShowHotJobsOnly { get; set; }

        public string WorkArrangement { get; set; }

        public string Occupation { get; set; }

        public List<string> OccupationSelect { get; set; }

        public List<string> IndustrySelect { get; set; }
    }
}