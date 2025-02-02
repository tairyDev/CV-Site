using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubService.Services.DataEntities
{
    public class Portfolio
    {
        public String UserName {  get; set; }
        public List<Repository> Repositories { get; set; }
    }
}
