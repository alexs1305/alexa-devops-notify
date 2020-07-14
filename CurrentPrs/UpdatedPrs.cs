using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrentPrs.Configs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace CurrentPrs
{
    public class UpdatedPrs
    {
        private readonly IConfigurations configs;

        public UpdatedPrs(IConfigurations configs)
        {
            this.configs = configs;
        }

        [FunctionName("UpdatedPrs")]
        public async Task RunAsync([TimerTrigger("0 * 9-17 * * *")]TimerInfo myTimer, ILogger log)
        {
            foreach (var config in configs.Configs)
            {
                var repos = config.Projects.Select(project => new Repository(project));
                await Task.WhenAll(repos.Select(r => r.Setup(config)));
                var activePrs = (await Task.WhenAll(repos.Select(r => r.ReadActivePrs(config)))).SelectMany(prs => prs).ToArray();
                await UpdatePerUser(config, activePrs);
            }
        }

        private static async Task UpdatePerUser(IConfiguration configuration, IEnumerable<GitPullRequest> allPrs)
        {
            var filtered = allPrs
                .Where(p => !p.CreatedBy.UniqueName.Equals(configuration.MyEmail))
                .Where(p => (p.LastMergeSourceCommit.Push?.Date
                    .CompareTo(DateTime.UtcNow.AddMinutes(-configuration.TimeRangeInMinutes)) ?? -1) >= 0);
            foreach (var pr in filtered)
            {
                var firstName = pr.CreatedBy.UniqueName.Split('.')[0];
                var prNumber = pr.PullRequestId;

                await Notifier.Notify($"{firstName} updated their pr {prNumber}","", configuration.NotificationToken);
            }
        }
    }
}
