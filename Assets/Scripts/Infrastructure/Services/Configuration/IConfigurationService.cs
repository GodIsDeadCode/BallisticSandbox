using BallisticSandbox.Configurations.Base;
using System.Collections.Generic;

namespace BallisticSandbox.Infrastructure.Services.Configuration
{
    public interface IConfigurationService
    {
        void Clear();
        TConfig GetConfig<TConfig>(string id, string domain) where TConfig : Configurations.Base.Configuration;
        IReadOnlyList<Configurations.Base.Configuration> GetDomain(string domain);
        bool HasConfiguration<TConfig>(string id, string domain) where TConfig : Configurations.Base.Configuration;
        void RegisterConfig<TConfig>(TConfig config) where TConfig : Configurations.Base.Configuration;
        void RegisterConfigs(ConfigurationDataBase configDB);
        bool TryGetConfig<TConfig>(string id, string domain, out TConfig config) where TConfig : Configurations.Base.Configuration;
        void UnloadDomain(string domain);
    }
}