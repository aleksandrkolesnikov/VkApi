using VkApi;


namespace VkApiTests
{
    static class VkClient
    {
        static VkClient() => Get = new Client(Settings.UserName, Settings.Password);

        public static Client Get { get; }
    }
}
