using System.Reflection;

namespace STI_Test_Report
{
    internal static class Program
    {
        static string _companyName = null;
        const string _appDataName = "STI Test Report";
        const string _teamProjectsFileName = "TeamProjects.txt";
        const string _queueSubFolder = "Queue";

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

            //static Configuration.LocalSettings _localSettings = null;
            //static StreamWriter _processFileWriter = null;


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