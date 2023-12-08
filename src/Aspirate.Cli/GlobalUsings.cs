// Global using directives

global using System.CommandLine;
global using System.CommandLine.Builder;
global using System.CommandLine.Invocation;
global using System.CommandLine.Parsing;
global using System.IO.Abstractions;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;

global using Aspirate.Cli;
global using Aspirate.Cli.Extensions;
global using Aspirate.Cli.Middleware;
global using Aspirate.Commands;
global using Aspirate.Commands.Actions;
global using Aspirate.Commands.Actions.Configuration;
global using Aspirate.Commands.Actions.Containers;
global using Aspirate.Commands.Actions.Manifests;
global using Aspirate.Commands.Extensions;
global using Aspirate.Processors.Dockerfile;
global using Aspirate.Processors.Extensions;
global using Aspirate.Processors.Final;
global using Aspirate.Processors.Postgresql;
global using Aspirate.Processors.Project;
global using Aspirate.Processors.RabbitMQ;
global using Aspirate.Processors.Redis;
global using Aspirate.Services;
global using Aspirate.Services.Extensions;
global using Aspirate.Services.Implementations;
global using Aspirate.Services.Interfaces;
global using Aspirate.Shared.Literals;
global using Aspirate.Shared.Processors;
global using Spectre.Console;
