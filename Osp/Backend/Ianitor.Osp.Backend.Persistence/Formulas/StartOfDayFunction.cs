using System;
using org.mariuszgromada.math.mxparser;

namespace Ianitor.Osp.Backend.Persistence.Formulas
{
    public class StartOfDayFunction : FunctionExtension
    {
        private double _dayCount;

        public StartOfDayFunction()
        {
            _dayCount = Double.NaN;
        }

        public StartOfDayFunction(double dayCount)
        {
            _dayCount = dayCount;
        }

        public int getParametersNumber()
        {
            return 1;
        }

        public void setParameterValue(int parameterIndex, double parameterValue)
        {
            if (parameterIndex == 0) _dayCount = parameterValue;
        }

        public string getParameterName(int parameterIndex)
        {
            if (parameterIndex == 0) return "dayCount";
            return "";
        }

        public double calculate()
        {
            return DateTime.Today.AddDays(_dayCount).Ticks;
        }

        public FunctionExtension clone()
        {
            return new StartOfDayFunction(_dayCount);
        }
    }
}