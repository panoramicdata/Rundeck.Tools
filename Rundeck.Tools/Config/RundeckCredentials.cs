using System.Collections.Generic;

namespace Rundeck.Tools.Config
{
    /// <summary>
    /// Rundeck credentials
    /// </summary>
    public class RundeckCredentials
    {
        /// <summary>
        /// The Rundeck URI
        /// </summary>
        public string Uri { get; set; } = default!;

        /// <summary>
        /// The Rundeck API token
        /// </summary>
        public string ApiToken { get; set; } = default!;

        /// <summary>
        /// Ensures that all values are set and are of the expected length
        /// Throws an exception if this is not the case
        /// </summary>
        internal void Validate()
        {
            // Uri
            if (string.IsNullOrWhiteSpace(Uri))
            {
                throw new ConfigurationException($"{nameof(Uri)} is not set");
            }

            // ApiToken
            if (string.IsNullOrWhiteSpace(ApiToken))
            {
                throw new ConfigurationException($"{nameof(ApiToken)} is not set");
            }
        }
    }
}