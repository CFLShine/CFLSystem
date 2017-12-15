using System;
using System.Collections.Generic;

namespace RuntimeExec
{
    public class REClass : REBase
    {
        public override REBase Copy()
        {
            return this;
        }

        public string TypeName { get; set; }

        public IEnumerable<REProperty> Properties
        {
            get
            {
                foreach(REProperty _pr in __properties)
                    yield return _pr;
            }
        }

        public IEnumerable<REField> Fields
        {
            get
            {
                foreach(REField _fld in __fields)
                    yield return _fld;
            }
        }

        public void AddProperty(REProperty _pr)
        {
            _pr.TreeAncestor = this;
            __properties.Add(_pr);
        }

        public void AddField(REField _fld)
        {
            _fld.TreeAncestor = this;
            __fields.Add(_fld);
        }

        public REProperty Property(string _name)
        {
            foreach(REProperty _pr in __properties)
            {
                if(_pr.MemberName == _name)
                    return _pr;
            }
            return null;
        }

        public REField Field(string _name)
        {
            foreach(REField _fld in __fields)
            {
                if(_fld.MemberName == _name)
                    return _fld;
            }
            return null;
        }

        /// <summary>
        /// propriété public pour répondre aux conventions de <see cref="SqlOrm"/>,
        /// mais ne pas utiliser autrement qu'en lecture.
        /// Pour modifier, utiliser <see cref=""/>.
        /// </summary>
        public List<REProperty> __properties { get; set; } = new List<REProperty>();
        /// <summary>
        /// 
        /// </summary>
        public List<REField> __fields { get; set; } = new List<REField>();

        public override REBase[] Children
        {
            get
            {
                REBase[] _children = new REBase[__properties.Count + __fields.Count];
                int _i = 0;
                foreach(REProperty _pr in __properties)
                {
                    _children[_i] = _pr;
                    ++_i;
                }
                foreach(REField _fld in __fields)
                {
                    _children[_i] = _fld;
                    ++_i;
                }
                return _children;
            }
        }
    }
}
