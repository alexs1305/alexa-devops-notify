
using CurrentPrs.Configs;
using System.Collections.Generic;

namespace CurrentPrs
{
    class Configurations : IConfigurations
    {
        public IEnumerable<IConfiguration> Configs => new[] { new ExampleConfiguration() };
    }
}
