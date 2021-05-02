﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Ianitor.Osp.ManagementTool.Commands;
using Microsoft.Extensions.Options;

namespace Ianitor.Osp.ManagementTool
{
    internal class Parser : IParser
    {
        private readonly IEnumerable<IOspCommand> _commands;
        private readonly IParserService _parserService;
        private readonly ICommandArgument _commandArg;


        public Parser(IParserService parserService, IEnumerable<IOspCommand> commands, IOptions<OspToolOptions> options)
        {
            _parserService = parserService;
            _commands = commands;

            Options = options;

            _commandArg = _parserService.AddCommandArgument("c", "command",
                new[]
                {
                    "Command that has to be executed:"
                }, true);

            foreach (var ospCommand in _commands)
            {
                ospCommand.AddCommand(_commandArg);
            }

            CreateSamples();
        }

        public IOptions<OspToolOptions> Options { get; }

        public void ShowUsageInformation()
        {
            _parserService.ShowUsageInformation("OspTool.exe");
        }

        public async Task ParseAndValidateAsync()
        {
            _parserService.ParseAndValidate(Environment.GetCommandLineArgs());

            var commandArgData = _parserService.GetArgumentValue(_commandArg);
            var command = commandArgData.GetValue<string>().ToLower();

            var ospCommand = _commands.FirstOrDefault(c => c.CommandValue.ToLower() == command);
            if (ospCommand == null)
            {
                throw new InvalidProgramException($"Command value '{command}' is invalid.");
            }

            await ospCommand.PreValidate();
            await ospCommand.Execute();
        }

        public void CreateSamples()
        {
            _parserService.AddSample(
                "OspTool.exe -c Config -csu \"https://localhost:5001/\" -isu \"https://localhost:5003/\" -tid \"myTenant\"",
                "Configures the tool to be used with tenant 'myTenant' on OSP instance 'https://localhost:5001', using identity services at 'https://localhost:5003/'.");
            _parserService.AddSample(
                "OspTool.exe -c LogIn -u \"user\" -psw \"secretPassword\"",
                "Logs to the configured identity services with user 'user' and password 'secretPassword'. The access token is stored in a config file in the user profile.");

            _parserService.AddSample(
                "OspTool.exe -c Create -tid \"myTenant\" -db \"osp\"",
                "Creates a new database named 'osp' available as tenant 'myTenant' on the configured OSP instance.");
            _parserService.AddSample(
                "OspTool.exe -c Attach -tid \"myTenant\" -db \"osp\"",
                "Attach an existing database named 'osp' available as tenant 'myTenant' on the configured OSP instance.");
            _parserService.AddSample(
                "OspTool.exe -c Delete -tid \"myTenant\"",
                "Deletes the corresponding database of tenant named 'myTenant' on the configured OSP instance.");
            _parserService.AddSample(
                "OspTool.exe -c Clear -tid \"myTenant\"",
                "Deletes CK/RT data from tenant named 'myService' on the configured OSP instance.");
            _parserService.AddSample(
                "OspTool.exe -c ImportCk -f d:\\myckmodel.json",
                "Imports a construction kit model to the configured tenant.");
            _parserService.AddSample(
                "OspTool.exe -c ImportRt -f d:\\myrtmodel.json",
                "Imports a runtime model to the configured tenant.");
        }
    }
}
