
namespace RuntimeExec
{
    public abstract class REOperator : REExpression
    {
        public override REBase ReValue 
        { 
            get
            {
                return __revalue;
            }

            set => throw new System.NotImplementedException(); 
        }

        protected REBase __revalue = null;

        public override object CValue 
        { 
            get => (ReValue != null)?((REExpression)ReValue).CValue : null; 
            set => throw new System.NotImplementedException(); 
        }

        public override REExpression Update(REClassObject _object)
        {
            // nothing to do.
            return this;
        }
    }
}
