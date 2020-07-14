using System.Collections.Generic;
using CurrentPrs.Configs;

namespace CurrentPrs
{
    public interface IConfigurations
    {
        IEnumerable<IConfiguration> Configs { get; }
    }
}