using org.mariuszgromada.math.mxparser;

namespace Ianitor.Osp.Backend.Persistence.Formulas
{
    public class OspExpression : Expression
    {
        private readonly Function _startOfDayFunction = new Function("startOfDay", new StartOfDayFunction());
        private readonly Function _nowFunction = new Function("now", new NowFunction());

        private readonly Constant _nullFunction = new Constant("null", double.NegativeInfinity);
        
       public OspExpression(string expressionString) : base(expressionString) {
           
            addDefinitions(_startOfDayFunction);
            addDefinitions(_nowFunction);
            addDefinitions(_nullFunction);
        }
    }
}