using Rundeck.Tools.Config;
using Rundeck.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System;
using Rundeck.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Rundeck.Tools
{
    /// <summary>
    /// The main application
    /// </summary>
    internal class Application
    {
        /// <summary>
        /// Configuration
        /// </summary>
        private readonly Configuration _config;

        /// <summary>
        /// The client to use for API interaction
        /// </summary>
        private readonly RundeckClient _rundeckClient;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<Application> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="loggerFactory"></param>
        public Application(
            IOptions<Configuration> options,
            ILoggerFactory loggerFactory)
        {
            // Store the config
            _config = options?.Value ?? throw new ArgumentOutOfRangeException(nameof(options), "Config not set.");

            // Validate the config
            _config.Validate();
            // The config is valid

            // Create a logger
            _logger = loggerFactory.CreateLogger<Application>();

            // Create a portal client
            _rundeckClient = new RundeckClient(
                new RundeckClientOptions
                {
                    Uri = new Uri(_config.RundeckCredentials.Uri),
                    ApiToken = _config.RundeckCredentials.ApiToken,
                    Logger = _logger
                }
            );
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            // Log action
            _logger.LogInformation($"{_config.Action} {_config.Class}");

            // Ensure we have the correct credentials
            try
            {
                var projects = await _rundeckClient
                    .Projects
                    .GetAllAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                return;
            }
            // The credentials are correct

            switch ((_config.Class, _config.Action))
            {
                case (RundeckClass.Project, RundeckAction.Create):
                    _logger.LogInformation("Creating Project: ");
                    var project = GetObject<Project>();
                    await _rundeckClient
                        .Projects
                        .CreateAsync(project, cancellationToken)
                        .ConfigureAwait(false);
                    break;
                case (RundeckClass.Project, RundeckAction.List):
                    _logger.LogInformation("Listing Projects: ");
                    var projects = await _rundeckClient
                        .Projects
                        .GetAllAsync(cancellationToken)
                        .ConfigureAwait(false);
                    WriteObject(projects);
                    break;
                case (RundeckClass.ProjectResource, RundeckAction.List):
                    if (CheckNotNull(_config.Parent, "Parent not specified."))
                    {
                        _logger.LogInformation($"List Resources for Project {_config.Parent}: ");
                        var resources = await _rundeckClient
                            .Projects
                            .GetResourcesAsync(_config.Parent, cancellationToken)
                            .ConfigureAwait(false);
                        WriteObject(resources.Values.ToList());
                    }
                    break;
                case (RundeckClass.NodeSource, RundeckAction.List):
                    if (CheckNotNull(_config.Parent, "Parent not specified."))
                    {
                        _logger.LogInformation($"List Node Sources for Project {_config.Parent}: ");
                        var nodeSources = await _rundeckClient
                            .Projects
                            .GetSourcesAsync(_config.Parent, cancellationToken)
                            .ConfigureAwait(false);
                        WriteObject(nodeSources);
                    }
                    break;
                default:
                    break;
            }
        }

        private bool CheckNotNull([NotNullWhen(true)]string? text, string errorMessage)
        {
            if (text is null)
            {
                throw new ConfigurationException(errorMessage);
            }
            return true;
        }

        private void WriteObject(object resources)
        {
            _logger.LogInformation($"Items:\n{JsonConvert.SerializeObject(resources, Formatting.Indented)}");
        }

        private T GetObject<T>()
        {
            if (_config.Object is null)
            {
                throw new ConfigurationException("Object not provided.");
            }
            var @object = JsonConvert.DeserializeObject<T>(_config.Object);
            if (@object == null)
            {
                throw new ConfigurationException($"Invalid Object {typeof(T).Name}.");
            }
            return @object;
        }
    }
}