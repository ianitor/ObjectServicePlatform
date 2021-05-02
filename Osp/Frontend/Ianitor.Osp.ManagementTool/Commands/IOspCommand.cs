using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ianitor.Common.CommandLineParser;

namespace Ianitor.Osp.ManagementTool.Commands
{
    public interface IOspCommand
    {
        string CommandValue { get; }
        void AddCommand(ICommandArgument commandArgument);

        Task PreValidate();

        Task Execute();
    }
}
