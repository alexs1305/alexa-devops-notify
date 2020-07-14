using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using CurrentPrs.Configs;
using System.Collections.Generic;

namespace CurrentPrs
{
    public class NewPullRequests
    {
        private IEnumerable<IConfiguration> configs;

        public NewPullRequests(IConfigurations configs)
        {
            this.configs = configs.Configs;
        }

        [FunctionName("NewPullRequests")]

        public async Task Run([TimerTrigger("0 * 9-17 * * *")]TimerInfo myTimer, ILogger log)
        {
            foreach (var config in configs)
            {
                var repos = config.Projects.Select(project => new Repository(project));

                var activePrs = (await Task.WhenAll(repos.Select(r => r.ReadActivePrs(config)))).ToArray();
                await NotifyPerUser(config, activePrs);
            }
        }

        private static async Task NotifyPerUser(IConfiguration config, GitPullRequest[][] activePrs)
        {
            for (var i = 0; i < activePrs.Length; i++)
            {
                activePrs[i] = activePrs[i]
                    .Where(p => !p.CreatedBy.UniqueName.Equals(config.MyEmail))
                    .Where(p => p.CreationDate.CompareTo(DateTime.UtcNow.AddMinutes(-config.TimeRangeInMinutes)) >= 0)
                    .ToArray();
                if (activePrs[i].Any())
                {
                    var project = config.Projects[i].Split('.').Last();
                    var isOnePr = activePrs[i].Length == 1;
                    var isAre = isOnePr ? "is a" : "are";
                    var withS = isOnePr ? "" : "s";

                    await Notifier.Notify($"There {isAre} new pull request{withS} for {project}","", config.NotificationToken);
                }
            }
        }
    }
}
