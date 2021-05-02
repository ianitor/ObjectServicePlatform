using Ianitor.Common.CommandLineParser;

namespace Ianitor.Osp.ManagementTool
{
    internal static class CommandLineParserExtensions
    {
        internal static T GetArgumentScalarValueOrDefault<T>(this ICommandArgumentValue commandArgumentValue,
            IArgument argument)
        {
            if (commandArgumentValue.IsArgumentUsed(argument))
            {
                var nameArgData = commandArgumentValue.GetArgumentValue(argument);
                return nameArgData.GetValue<T>();
            }

            return default;
        }

        internal static T GetArgumentScalarValue<T>(this ICommandArgumentValue commandArgumentValue, IArgument argument)
        {
            var nameArgData = commandArgumentValue.GetArgumentValue(argument);
            return nameArgData.GetValue<T>();
        }
    }
}