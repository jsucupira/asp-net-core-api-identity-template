using System.Configuration;

namespace TemplateProject.Common
{
    public class Settings
    {
        public static string DefaultValue
        {
            get { return ConfigurationManager.AppSettings["DefaultValue"]; }
        }
    }
}