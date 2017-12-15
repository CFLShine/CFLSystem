

namespace RuntimeExec
{
    public abstract class REBinaryOperator : REOperator
    {
        public REBinaryOperator(){}
        public REBinaryOperator(object operandLeft, object operandRight)
        {
            if(operandLeft is REExpression _exprL)
                OperandLeft = _exprL;
            else
                OperandLeft = new REValue(operandLeft);
            
            if(operandRight is REExpression _exprR)
                OperandRight = _exprR;
            else
                OperandRight = new REValue(operandRight);
        }

        public override REExpression Invoke()
        {
            object _valLeft = OperandLeft?.CValue;
            object _valRight = OperandRight?.CValue;
            __revalue = new REValue(Result(_valLeft, _valRight));
            return this;
        }

        public override REBase[] Children { get => new REBase[2]{ OperandLeft, OperandRight }; }

        public REExpression OperandLeft
        {
            get => __operandLeft;
            set
            {
                __operandLeft = value;
                if(__operandLeft != null)
                    __operandLeft.TreeAncestor = this;
            }
        }

        public REExpression OperandRight
        {
            get => __operandRight;
            set
            {
                __operandRight = value;
                if(__operandRight != null)
                    __operandRight.TreeAncestor = this;
            }
        }

        protected abstract object Result(dynamic valLeft, dynamic valRight);

        private REExpression __operandLeft = null;
        private REExpression __operandRight = null;
    }
}
