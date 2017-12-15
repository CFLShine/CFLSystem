using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFL_1.CFL_System.MSTD
{
    /// <summary>
    ///<see cref="ObjectsBrowser"/> est une collection d'objets avec des méthodes pour faciliter
    /// le parcours des éléments.
    /// </summary>
    public class ObjectsBrowser<T> 
    {
        /// <summary>
        /// Construit <see cref="ObjectsBrowser{T}"/> à partir de la liste l.
        /// Provoque une exception si les éléments de l ne sont pas convertibles en <see cref="T"/>.
        /// </summary>
        public ObjectsBrowser(IList l)
        {
            foreach(object _o in l)
            {
                __elements.Add((T)_o);
            }
        }

        /// <summary>
        /// Construit <see cref="ObjectsBrowser{T}"/> à partir de l'Array a.
        /// Provoque une exception si les éléments de a ne sont pas convertibles en <see cref="T"/>.
        /// </summary>
        public ObjectsBrowser(Array a)
        {
            foreach(object _o in a)
            {
                __elements.Add((T)_o);
            }
        }

        /// <summary>
        /// Retourne l'élément à la position Pos, ou default(T) si Pos == -1.
        /// </summary>
        T Element
        {
            get
            {
                if(__pos > -1)
                    return __elements[__pos];
                return default(T);
            }
        } 

        /// <summary>
        /// Se positione avant le premier élément.
        /// </summary>
        public void GotoStart()
        {
            __pos = -1;
        }

        public int Pos => __pos;

        public bool CanStep(int steps)
        {
            int _nextPos = __pos + steps;
            return _nextPos > -1 && _nextPos < __elements.Count;
        }

        public bool Step(int steps)
        {
            int _nextPos = __pos + steps;
            if(_nextPos > -1 && _nextPos < __elements.Count)
            {
                __pos = _nextPos;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retourne l'élément à la position Pos + r,
        /// ou default(T) si Pos + r est en dehors des limites.
        /// </summary>
        public T Relative(int r)
        {
            int _nextPos = __pos + r;
            if(_nextPos > -1 && _nextPos < __elements.Count)
                return __elements[_nextPos];
            return default(T);
        }

        private List<T> __elements;
        private int __pos = -1;
    }
}
