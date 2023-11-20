// Global using directives

global using System.IO.Abstractions;
global using System.ComponentModel;
global using System.Text;
global using System.Text.Json;
global using Microsoft.Extensions.DependencyInjection;

global using Aspirate.Cli.Commands.EndToEnd;
global using Aspirate.Cli.Extensions;
global using Aspirate.Cli.Logging;
global using Aspirate.Cli.Processors.Components.Final;
global using Aspirate.Cli.Processors.Components.Postgresql;
global using Aspirate.Cli.Processors.Components.Project;
global using Aspirate.Cli.Services;
global using Aspirate.Contracts.Interfaces;
global using Aspirate.Contracts.Literals;
global using Aspirate.Contracts.Models;
global using Aspirate.Contracts.Models.AspireManifests;
global using Aspirate.Contracts.Models.AspireManifests.Components;
global using Aspirate.Contracts.Models.AspireManifests.Components.V0;
global using Aspirate.Contracts.Models.Containers;
global using Aspirate.Contracts.Processors;
global using Aspirate.Spectre.DependencyInjection;
global using CliWrap;
global using Humanizer;
global using Microsoft.Extensions.Logging;
global using Serilog;
global using Spectre.Console;
global using Spectre.Console.Cli;
global using ILogger = Microsoft.Extensions.Logging.ILogger;
