using System;
using Newtonsoft.Json.Linq;

namespace Rundeck.Tools.Config
{
    /// <summary>
    /// Application configuration, loaded from an appsettings.json file upon execution
    /// You can modify/extend this class and provide your own settings
    /// </summary>
    internal class Configuration
    {
        /// <summary>
        /// Rundeck credentials
        /// </summary>
        public RundeckCredentials RundeckCredentials { get; set; } = default!;

        /// <summary>
        /// Class
        /// </summary>
        public RundeckClass? Class { get; set; } = default!;

        /// <summary>
        /// Action
        /// </summary>
        public RundeckAction? Action { get; set; } = default!;

        /// <summary>
        /// JSON version of the object
        /// </summary>
        public string? Object { get; set; }
        public string? Parent { get; set; }

        internal void Validate()
        {
            if (RundeckCredentials is null)
            {
                throw new ConfigurationException($"Missing {nameof(RundeckCredentials)}");
            }
            RundeckCredentials?.Validate();

            if (Class is null)
            {
                throw new ConfigurationException($"Missing {nameof(Class)}");
            }

            if (Action is null)
            {
                throw new ConfigurationException($"Missing {nameof(Action)}");
            }

            switch (Action)
            {
                case RundeckAction.Create:
                    if (string.IsNullOrWhiteSpace(Object))
                    {
                        throw new ConfigurationException($"Missing {nameof(Object)} with action {nameof(Action)}");
                    }
                    break;
            }
        }
    }
}
