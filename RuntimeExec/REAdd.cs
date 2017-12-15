
namespace RuntimeExec
{
    public class REAdd : REBinaryOperator
    {
        public REAdd(){ }
        public REAdd(object operandLeft, object operandRight)
            :base(operandLeft, operandRight)
        { }

        public override REBase Copy()
        {
            return new REAdd(OperandLeft.Copy(), OperandRight.Copy());
        }

        protected override object Result(dynamic valLeft, dynamic valRight)
        {
            return valLeft + valRight;
        }
    }
}
