using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHub.Service.Modals
{
    public class Portfolio
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public List<string> Languages { get; set; }
        public DateTimeOffset? LastCommitDate { get; set; }
        public int Stars { get; set; }
        public int PullRequests { get; set; }
    }
}
