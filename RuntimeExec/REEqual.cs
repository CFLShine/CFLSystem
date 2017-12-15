using System;

namespace RuntimeExec
{
    public class REEqual : REBinaryOperator
    {
        public REEqual(){ }
        public REEqual(REExpression operandLeft, REExpression operandRight)
            :base(operandLeft, operandRight)
        {}

        public override REBase Copy()
        {
            REExpression _operandL = (OperandLeft != null)? (REExpression)OperandLeft.Copy() : null;
            REExpression _operandR = (OperandRight != null)? (REExpression)OperandRight.Copy() : null;
            return new REEqual(_operandL, _operandR);
        }

        protected override object Result(dynamic valLeft, dynamic valRight)
        {
            return valLeft == valRight;
        }
    }
}
