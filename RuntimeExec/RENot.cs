

namespace RuntimeExec
{
    public class RENot : REUnaryOperator
    {
        public RENot(){ }

        public RENot(object operand)
            :base(operand)
        {}

        public override REBase Copy()
        {
            if(Operand != null)
                return new RENot(Operand.Copy());
            else return new RENot();
        }

        protected override object Result(dynamic val)
        {
            return !val;
        }
    }
}
