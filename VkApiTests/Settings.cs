using System.Xml.Linq;


namespace VkApiTests
{
    static class Settings
    {
        static Settings()
        {
            var settings = XElement.Load("Settings.xml");
            var creds = settings.Element("Credentials");
            Password = (string)creds.Attribute("password");
            UserName = (string)creds.Attribute("username");
        }

        public static string Password { get; }
        public static string UserName { get; }
    }
}
