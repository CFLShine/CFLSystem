

namespace RuntimeExec
{
    public class RESmaller : REBinaryOperator
    {
        public RESmaller()
        {}

        public RESmaller(object operandLeft, object operandRight)
            :base(operandLeft, operandRight)
        {}

        public override REBase Copy()
        {
            REExpression _operandL = (OperandLeft != null)? (REExpression)OperandLeft.Copy() : null;
            REExpression _operandR = (OperandRight != null)? (REExpression)OperandRight.Copy() : null;
            return new RESmaller(_operandL, _operandR);
        }

        protected override object Result(dynamic valLeft, dynamic valRight)
        {
            return valLeft < valRight;
        }
    }
}
