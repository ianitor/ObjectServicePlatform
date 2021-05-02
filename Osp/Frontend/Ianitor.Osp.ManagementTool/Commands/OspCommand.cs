using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;
using Microsoft.Extensions.Options;
using NLog;

namespace Ianitor.Osp.ManagementTool.Commands
{
    internal abstract class OspCommand : IOspCommand
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string _commandDescription;

        protected OspCommand(string commandValue, string commandDescription, IOptions<OspToolOptions> options)
        {
            _commandDescription = commandDescription;
            CommandValue = commandValue;
            Options = options;
        }

        public string CommandValue { get; }

        protected ICommandArgumentValue CommandArgumentValue { get; private set; }
        protected IOptions<OspToolOptions> Options { get; }

        public void AddCommand(ICommandArgument commandArgument)
        {
            CommandArgumentValue = commandArgument.AddCommandValue(CommandValue, _commandDescription);
            AddArguments();
        }

        protected virtual void AddArguments()
        {

        }

        public virtual Task PreValidate()
        {
            return Task.CompletedTask;
        }

        public abstract Task Execute();


    }
}
