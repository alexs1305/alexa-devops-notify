namespace CurrentPrs.Configs
{
    class ExampleConfiguration : IConfiguration
    {
        //link to example devops https://dev.azure.com/my-dev-ops/Projects/_git/ProjectAtitle
        public string CollectionUri => "https://dev.azure.com/my-dev-ops";
        public string ProjectName => "Projects";
        public string[] Projects => new[]
        {
            "ProjectATitle",
        };
        public int TimeRangeInMinutes => 1;
        public string Token => "[DevOpsToken]";
        public string MyEmail => "Devops Login Email";
        public string NotificationToken => "[NotifyMe token]";
    }
}
