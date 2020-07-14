using CurrentPrs.Configs;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrentPrs
{
    class Repository
    {
        private GitHttpClient gitClient;
        private GitRepository gitRepository;
        private string project;


        public Repository(string project)
        {
            this.project = project;
        }

        public async Task<Repository> Setup(IConfiguration config)
        {
            try
            {
                VssConnection connection = new VssConnection(
                new Uri(config.CollectionUri), new VssBasicCredential(config.MyEmail, config.Token));

                // Get a GitHttpClient to talk to the Git endpoints
                this.gitClient = await connection.GetClientAsync<GitHttpClient>();
                this.gitRepository = await this.gitClient.GetRepositoryAsync(config.ProjectName, this.project);
                return this;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this;
            }
        }

        public async Task<GitPullRequest[]> ReadActivePrs(IConfiguration config)
        {
            if (gitClient is null || this.gitRepository is null) await this.Setup(config);
            try
            {
                List<GitPullRequest> prs = await gitClient.GetPullRequestsAsync(
                     this.gitRepository.Id,
                     new GitPullRequestSearchCriteria()
                     {
                         TargetRefName = "refs/heads/master",
                         Status = PullRequestStatus.Active

                     });
                return prs?.ToArray() ?? new GitPullRequest[0];
            }catch(Exception e)
            {
                return new GitPullRequest[0];
            }
        }
    }
}
