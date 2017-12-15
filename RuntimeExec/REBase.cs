
using System;
using MSTD.ShBase;

namespace RuntimeExec
{
    public abstract class REBase : Base
    {
        public static REValue NULL = new REValue(null);

        /// <summary>
        /// <see cref="TreeAncestor"/> est le <see cref="REBase"/> dont celui-ci fait partie.
        /// Permet de remonter l'arbre jusqu'à l'ancêtre racine.
        /// </summary>
        public virtual REBase TreeAncestor 
        { 
            get => __treeAncestor; 
            set => __treeAncestor = value;
        }
        
        public abstract REBase[] Children { get; }

        public abstract REBase Copy();

        /// <summary>
        /// Retourne le premier ancêtre trouvé de type t
        /// </summary>
        public REBase FindAncestorOfType(Type t)
        {
            REBase _treeAncestor = TreeAncestor;
            while(_treeAncestor != null)
            {
                if(_treeAncestor.GetType() == t)
                    return _treeAncestor;
                _treeAncestor = _treeAncestor.TreeAncestor;
            }
            return null;
        }

        #region Helper methods

        /// <summary>
        /// Retourne true si l'objet est de type <see cref="ShBase"/> 
        /// mais pas de type <see cref="REBase"/>.
        /// </summary>
        public static bool IsCustomClass(Type t)
        {
            return t != null 
                && t.IsSubclassOf(typeof(Base)) 
                && (!t.IsSubclassOf(typeof(REBase)));
        }

        #endregion

        private REBase __treeAncestor = null;
    }
}
