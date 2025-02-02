using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubService.Services
{
    public class RepositoryDetails
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Languages { get; set; }
        public DateTime? LastCommit { get; set; }
        public int Stars { get; set; }
        public int OpenPullRequests { get; set; }
        public string RepoUrl { get; set; }
    }
}
