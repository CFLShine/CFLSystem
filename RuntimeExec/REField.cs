using System;

namespace RuntimeExec 
{
    public class REField : REMember
    {
        public REField() { }

        public REField(string parentTypeName, REBase value, string memberName)
        {
            ParentTypeName = parentTypeName;
            ReValue = value;
            MemberName = memberName;
        }

        public REField(REBase _value, string _memberName)
        {
            ReValue = _value;
            MemberName = _memberName;
        }

        public REField(string _parentTypeName, string _memberName)
        {
            ParentTypeName = _parentTypeName; 
            MemberName = _memberName; 
        }

        public override REBase Copy()
        {
            return new REField(ParentTypeName, MemberName);
        }

        public override REBase ReValue 
        { 
            get
            {
                if(__revalue is REBase _base)
                    return _base;
                return this;
            }
            set
            {
                if(__revalue is REExpression _expr)
                {
                    _expr.ReValue = value;
                }
                else
                {
                    __revalue = value;
                }
            }
        }

        private REBase __revalue = null;

        public override object CValue
        {
            get
            {
                if(__revalue is REExpression _expr)
                    return _expr.CValue;
                else
                return null;
            }

            set
            {
                if(__revalue is REExpression _expr)
                    _expr.CValue = value;
                else
                    throw new Exception("Cette propriété désigne un objet de type " + __revalue.GetType().Name +
                                    " auquel une valeur CSharpe ne peut être assignée.");
            }
        }

        public override REExpression Invoke()
        {
            //do nothing
            return this;
        }
    }
}
