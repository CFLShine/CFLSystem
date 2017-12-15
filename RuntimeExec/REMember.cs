
namespace RuntimeExec
{
    /// <summary>
    /// <see cref="REMember"/> est la classe abstraite dont héritent 
    /// les classes qui expriment un membre Property ou Field.
    /// Elle contientla propriété <see cref="TreeAncestor"/>, le <see cref="REClassObject"/>
    /// qui contient ce membre.
    /// </summary>
    public abstract class REMember : REExpression
    {
        public REMember(){}

        public REMember(string _memberName) => MemberName = _memberName;

        /// <summary>
        /// Le <see cref="REClassObject"/> dont fait partie ce <see cref="REMember"/>.
        /// </summary>
        public virtual REClassObject Parent
        {
            get => __parent;
            set
            {
                __parent = value;
                __parentTypeName = (__parent == null)? "" : __parent.ObjectTypeName;
            }
        }

        public string ParentTypeName
        {
            get => __parentTypeName;
            set
            {
                __parentTypeName = value;
            }
        }

        public string MemberName { get; set; }

        public override REBase[] Children
        {
            get
            {
                return new REBase[0];
            }
        }

        /// <summary>
        /// Donne _object à <see cref="TreeAncestor"/> si <see cref="ParentTypeName"/> != ""
        /// et _object.TypeName == <see cref="ParentTypeName"/>.
        /// </summary>
        public override REExpression Update(REClassObject _object)
        {
            if(_object != null)
            {
                if(!string.IsNullOrWhiteSpace(ParentTypeName) && _object.ObjectTypeName == ParentTypeName)
                    Parent = _object;
            }
            return this;
        }

        protected REClassObject __parent = null;
        protected string __parentTypeName = "";
    }
}
