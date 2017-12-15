using System;

namespace RuntimeExec
{
    public class REValue : REExpression
    {
        public REValue(){}
        public REValue(object _internalValue)
        {
            InternalValue = _internalValue;
        }

        public override REBase Copy()
        {
            return new REValue(InternalValue);
        }

        public override REBase ReValue 
        {
            get => this;
            set
            {
                if(value is REExpression _expr)
                    InternalValue = _expr.CValue;
                else
                    InternalValue = value;
            }
        }

        public override object CValue 
        { 
            get => InternalValue; 
            set => InternalValue = value; 
        }

        public object InternalValue { get; set; }

        public override REBase[] Children => new REBase[0];

        public override REExpression Update(REClassObject _object)
        {
            if(InternalValue != null && _object != null && InternalValue.GetType() == _object.GetType())
                ReValue = _object;
            return this;
        }

        public override REExpression Invoke()
        {
            //do nothing
            return this;
        }
    }
}
