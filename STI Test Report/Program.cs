using System.Reflection;

namespace STI_Test_Report
{
    internal static class Program
    {
        static string _companyName = null;
        const string _appDataName = "STI Test Report";
        const string _teamProjectsFileName = "TeamProjects.txt";
        const string _userSettingsFileName = "Settings.txt";
        const string _logSubFolder = "Logs";
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
            if (!Directory.Exists(GetApplicationDataFolder(_logSubFolder)))
                Directory.CreateDirectory(GetApplicationDataFolder(_logSubFolder));
            UserSettings= UserSettings.ReadUserSettings(GetApplicationDataFolder(_userSettingsFileName));
            if (UserSettings == null)
                UserSettings = new UserSettings();

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
        public static string GetApplicationFolder(string subObject)
        {
            if (string.IsNullOrEmpty(subObject))
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            else
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), subObject);
        }

        public static string GetTeamProjectsFilePath() => GetApplicationDataFolder(_teamProjectsFileName);
        public static string GetTemplateDBFilePath() => GetApplicationFolder(_templateDB);
        public static string GetLogFolder() => GetApplicationDataFolder(_logSubFolder);

        public static UserSettings UserSettings { get; set; }

        public static void UserSettingsSave() => UserSettings.WriteUserSettings(GetApplicationDataFolder(_userSettingsFileName));
    }
}