
using System.Collections.Generic;
using MSTD.ShBase;

namespace RuntimeExec
{
    public abstract class REExpression : REBase
    {
        public abstract REBase ReValue { get; set; }

        public abstract object CValue { get; set; }

        public abstract REExpression Invoke();

        /// <summary>
        /// Si cette <see cref="REExpression"/> se base sur un <see cref="REClassObject"/>
        /// du même type que _object, le remplace par _object.
        /// Ne se propage pas aux enfants de cette expression.
        /// </summary>
        public abstract REExpression Update(REClassObject _object);

        public REExpression Update(Base _base)
        {
            return Update(new REClassObject(_base));
        }

        /// <summary>
        /// Fonction récursive qui retourne la liste complete de toutes les expressions
        /// enfants de cette expression et les enfants des enfants, etc.
        /// Provoque une exception si _list est null.
        /// </summary>
        public void ChildrenAll(List<REExpression> _list)
        {
            foreach(REExpression _child in Children)
            {
                if(_child != null && !_list.Contains(_child))
                {
                    _list.Add(_child);
                    _child.ChildrenAll(_list);
                }
            }
        }
    }
}
