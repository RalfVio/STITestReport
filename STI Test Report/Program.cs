using System.Reflection;

namespace STI_Test_Report
{
    internal static class Program
    {
        static string _companyName = null;
        const string _appDataName = "STI Test Report";
        const string _teamProjectsFileName = "TeamProjects.txt";
        const string _queueSubFolder = "Queue";
        const string _templateDB = "Template.db";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes();
            foreach (var attribute in attributes)
            {
                if (attribute is AssemblyCompanyAttribute companyAttribute)
                    _companyName = companyAttribute.Company;
            }

            string appDatafolder = GetApplicationDataFolder(null);
            if (!Directory.Exists(appDatafolder))
                Directory.CreateDirectory(appDatafolder);
            if (!Directory.Exists(GetApplicationDataFolder(_queueSubFolder)))
                Directory.CreateDirectory(GetApplicationDataFolder(_queueSubFolder));
            if (!File.Exists(GetApplicationDataFolder(_templateDB)))
            File.Copy(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _templateDB),
                GetApplicationDataFolder(_templateDB));

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new StartForm());
        }

        public static string GetApplicationDataFolder()=> GetApplicationDataFolder(null);
        public static string GetApplicationDataFolder(string subObject)
        {
            if (string.IsNullOrEmpty(subObject))
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), _companyName, _appDataName);
            else
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), _companyName, _appDataName, subObject);
        }

        public static string GetTeamProjectsFilePath() => GetApplicationDataFolder(_teamProjectsFileName);
    }
}