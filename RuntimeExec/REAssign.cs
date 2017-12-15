

namespace RuntimeExec
{
    public class REAssign : REBinaryOperator
    {
        public override REBase Copy()
        {
            return new REAdd(OperandLeft.Copy(), OperandRight.Copy());
        }

        /// <summary>
        /// get :
        /// retourne OperandLeft.
        /// Exception si OperandLeft ou OperandRight null.
        /// set:
        /// Exception.
        /// </summary>
        public override REBase ReValue 
        { 
            get
            {
                return OperandLeft;
            }
            set => throw new System.NotImplementedException(); 
        }

        /// <summary>
        /// Assigne la valeur interne de <see cref="OperandRight"/> à <see cref="OperandLeft"/>
        /// </summary>
        public override REExpression Invoke()
        {
            OperandLeft.CValue = OperandRight?.CValue;
            return this;
        }

        protected override object Result(dynamic valLeft, dynamic valRight)
        {
            throw new System.NotImplementedException();
        }
    }
}
