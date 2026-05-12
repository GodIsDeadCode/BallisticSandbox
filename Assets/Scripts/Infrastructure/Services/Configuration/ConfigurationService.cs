using System;
using System.Collections.Generic;
using System.Linq;
using BallisticSandbox.Infrastructure.Services.Logger;

namespace BallisticSandbox.Infrastructure.Services.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, Dictionary<string, Configurations.Base.Configuration>> _configurations;

        public ConfigurationService(ILogger logger)
        {
            _logger = logger;
            _configurations = new Dictionary<string, Dictionary<string, Configurations.Base.Configuration>>();
        }

        public TConfig GetConfig<TConfig>(string id, string domain) where TConfig : Configurations.Base.Configuration
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Configuration id is not valid.", nameof(id));

            if (string.IsNullOrEmpty(domain))
                throw new ArgumentException("Configuration domain is not valid.", nameof(domain));

            if (_configurations.TryGetValue(domain, out Dictionary<string, Configurations.Base.Configuration> configs))
            {
                if (configs.TryGetValue(id, out Configurations.Base.Configuration config))
                    return (TConfig)config;
            }

            throw new InvalidOperationException($"Configuration [{typeof(TConfig).Name}] " +
                                                $"id [{id}] domain [{domain}] not registered.");
        }

        public bool TryGetConfig<TConfig>(string id, string domain, out TConfig config) where TConfig : Configurations.Base.Configuration
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Configuration id is not valid.", nameof(id));

            if (string.IsNullOrEmpty(domain))
                throw new ArgumentException("Configuration domain is not valid.", nameof(domain));

            if (_configurations.TryGetValue(domain, out Dictionary<string, Configurations.Base.Configuration> configs))
            {
                if (configs.TryGetValue(id, out Configurations.Base.Configuration conf))
                {
                    config = (TConfig)conf;
                    return true;
                }
            }

            config = default(TConfig);
            return false;
        }

        public bool HasConfiguration<TConfig>(string id, string domain) where TConfig : Configurations.Base.Configuration
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Configuration id is not valid.", nameof(id));

            if (string.IsNullOrEmpty(domain))
                throw new ArgumentException("Configuration domain is not valid.", nameof(domain));

            if (_configurations.TryGetValue(domain, out Dictionary<string, Configurations.Base.Configuration> configs))
                return configs.ContainsKey(id);

            return false;
        }

        public IReadOnlyList<Configurations.Base.Configuration> GetDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
                throw new ArgumentException("Configuration domain is not valid.", nameof(domain));

            if (_configurations.TryGetValue(domain, out Dictionary<string, Configurations.Base.Configuration> configs))
                return configs.Values.ToList();

            return null;
        }

        public void RegisterConfig<TConfig>(TConfig config) where TConfig : Configurations.Base.Configuration
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config), "Config cannot be null.");

            if (string.IsNullOrEmpty(config.Domain))
                throw new ArgumentException("Configuration domain is not valid.", nameof(config.Domain));

            if (string.IsNullOrEmpty(config.ID))
                throw new ArgumentException("Configuration id is not valid.", nameof(config.ID));

            if (!_configurations.TryGetValue(config.Domain, out Dictionary<string, Configurations.Base.Configuration> configs))
            {
                configs = new Dictionary<string, Configurations.Base.Configuration>();
                _configurations.Add(config.Domain, configs);
            }

            if (configs.ContainsKey(config.ID))
                _logger.LogWarning($"Duplicate skipped: {config.GetType().Name} : {config.ID}", "ConfigurationService");
            else
            {
                configs[config.ID] = config;
                _logger.LogWarning($"Registered: {config.GetType().Name} : {config.ID}", "ConfigurationService");
            }
        }

        public void RegisterConfigs(Configurations.Base.ConfigurationDataBase configDB)
        {
            if (configDB == null)
                throw new ArgumentNullException(nameof(configDB), "Configs cannot be null.");

            IReadOnlyList<Configurations.Base.Configuration> configurations = configDB.Configurations;
            for (int i = 0; i < configurations.Count; i++)
            {
                Configurations.Base.Configuration config = configurations[i];
                if (config == null)
                    continue;

                RegisterConfig(config);
            }
        }

        public void UnloadDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
                throw new ArgumentException("Configuration domain is not valid.", nameof(domain));

            if (_configurations.Remove(domain))
                _logger.LogInfo($"Domain unloaded: {domain}", "ConfigurationService");
            else
                _logger.LogWarning($"Domain nor find: {domain}", "ConfigurationService");
        }

        public void Clear()
        {
            _configurations.Clear();
            _logger.LogInfo("All domains cleared.", "ConfigurationService");
        }
    }
}
