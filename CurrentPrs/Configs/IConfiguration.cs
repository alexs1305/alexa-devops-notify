namespace CurrentPrs.Configs
{
    public interface IConfiguration
    {
        string CollectionUri { get; }
        string MyEmail { get; }
        string NotificationToken { get; }
        string ProjectName { get; }
        string[] Projects { get; }
        int TimeRangeInMinutes { get; }
        string Token { get; }
    }
}