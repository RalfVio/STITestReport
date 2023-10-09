using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADORest.BusinessObjects
{
    public class TeamProject
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string OrganizationUrl { get; set; }
        public string TeamProjectName { get; set; }
        public string PAT { get; set; }
       
    }
}
