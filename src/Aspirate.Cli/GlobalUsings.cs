// Global using directives

global using System.CommandLine;
global using System.CommandLine.Builder;
global using System.CommandLine.Invocation;
global using System.CommandLine.NamingConventionBinder;
global using System.CommandLine.Parsing;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;

global using Aspirate.Cli;
global using Aspirate.Cli.Middleware;
global using Aspirate.Commands;
global using Aspirate.Commands.Commands.Apply;
global using Aspirate.Commands.Commands.Build;
global using Aspirate.Commands.Commands.Destroy;
global using Aspirate.Commands.Commands.Generate;
global using Aspirate.Commands.Commands.Init;
global using Aspirate.Commands.Commands.Run;
global using Aspirate.Commands.Commands.Settings;
global using Aspirate.Commands.Commands.Stop;
global using Aspirate.Processors;
global using Aspirate.Secrets;
global using Aspirate.Services;
global using Aspirate.Shared.Literals;
global using Spectre.Console;
