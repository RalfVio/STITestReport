using ADORest.BusinessObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STI_Test_Report
{
    internal class TeamProjects
    {
        public TeamProjects() { Projects = new List<TeamProject>(); }
        public int CurrentProject { get; set; }
        public List<TeamProject> Projects { get; set; }
        public static TeamProjects CreateExample()
        {
            var result = new TeamProjects() { CurrentProject = 1 };
            result.Projects.Add(new TeamProject()
            {
                Title = "Sample",
                OrganizationUrl = "https://dev.azure.com/<your org>",
                TeamProjectName = "<your team project>",
                PAT = "<your personal access token>"
            });

            return result;
        }
        public TeamProject GetProject()
        {
            if (CurrentProject > 0 && CurrentProject <= Projects.Count)
                return Projects[CurrentProject - 1];
            else
            {
                this.CurrentProject = 1;
                return this.Projects.FirstOrDefault();
            }
        }
        public  static TeamProjects GetTeamProjects()
        {
            string filePath = Program.GetTeamProjectsFilePath();
            if (!File.Exists(filePath))
                return null;

            var result = JsonConvert.DeserializeObject<TeamProjects>(File.ReadAllText(filePath));
            for (int i = 0; i < result.Projects.Count; i++)
                result.Projects[i].Id = i + 1;

            return result;
        }
    }
}
