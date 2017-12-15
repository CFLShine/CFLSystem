

namespace RuntimeExec
{
    public class REAnd : REBinaryOperator
    {
        public REAnd(){ }
        public REAnd(object operandLeft, object operandRight)
            :base(operandLeft, operandRight)
        { }

        public override REBase Copy()
        {
            REExpression _operandL = (OperandLeft != null)? (REExpression)OperandLeft.Copy() : null;
            REExpression _operandR = (OperandRight != null)? (REExpression)OperandRight.Copy() : null;
            return new REAnd(_operandL, _operandR);
        }

        protected override object Result(dynamic valLeft, dynamic valRight)
        {
            return valLeft && valRight;
        }
    }
}
