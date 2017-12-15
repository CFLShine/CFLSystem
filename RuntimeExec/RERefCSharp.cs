using System;
using System.Reflection;

namespace RuntimeExec
{
    public class RERefCSharp : REProperty
    {
        #region constructors

        public RERefCSharp(){ }

        public RERefCSharp(string _parentTypeName, string _memberName)
        {
            ParentTypeName = _parentTypeName;
            MemberName = _memberName;
        }

        public RERefCSharp(REClassObject _parent, string _memberName)
        {
            Parent = _parent;
            MemberName = _memberName;
        }

        #endregion constructors

        public override REBase Copy()
        {
            return new RERefCSharp(ParentTypeName, MemberName);
        }

        /// <summary>
        /// si <see cref="MemberInfo"/> ou <see cref="Parent"/> sont nuls,
        /// get retourne null,
        /// set provoque une <see cref="ArgumentNullException"/>
        /// </summary>
        public override REBase ReValue 
        { 
            get
            {
                return this;
            }

            set
            {
                if(value is REExpression _expr)
                    CValue = _expr.CValue;
                else
                    if(value == null)
                        throw new NullReferenceException("value");
                else
                    throw new Exception("Il n'est pas possible d'assigner un objet de type " + value.GetType().Name +
                                        " à un " + this.GetType().Name);
            }
        }

        /// <summary>
        /// Le set provoque une erreur si <see cref="Parent"/> == null.
        /// </summary>
        public override object CValue 
        { 
            get
            {
                if(Parent == null || Parent.CValue== null || string.IsNullOrWhiteSpace(MemberName))
                    return null;
                Type _cSharpType = Parent.CValue.GetType();
                PropertyInfo _prInfo = _cSharpType.GetProperty(MemberName);
                if(_prInfo != null)
                    return _prInfo.GetValue(Parent.CValue);
               
                FieldInfo _fldInfo = _cSharpType.GetField(MemberName);
                if(_fldInfo != null)
                    return (_fldInfo.GetValue(Parent.CValue));
                throw new Exception("Le membre " + MemberName + " n'a pas été trouvé dans les champs ni propriétés du type " + _cSharpType.Name);
            }
            
            set
            {
                if(Parent == null)
                    throw new NullReferenceException("Parent");
                if(Parent.CValue == null)
                    throw new NullReferenceException("Parent.CValue");
                
                Type _cSharpType = Parent.CValue.GetType();

                PropertyInfo _prInfo = _cSharpType.GetProperty(MemberName);
                if(_prInfo != null)
                    _prInfo.SetValue(Parent.CValue, value);
                else
                {
                    FieldInfo _fldInfo = _cSharpType.GetField(MemberName);
                    if(_fldInfo != null)
                        _fldInfo.SetValue(Parent.CValue, value);
                    else
                        throw new Exception("Le membre " + MemberName + " n'a pas été trouvé dans les membres du type " + _cSharpType.Name);
                }
            }
        }
        
    }
}
