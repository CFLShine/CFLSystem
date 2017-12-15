using System.Collections.Generic;

namespace RuntimeExec
{
    public class RENameSpace : REBase
    {
        public REApplicattion Sollution { get; set; }

        public override REBase Copy()
        {
            return this;
        }

        public string Name { get; set; }

        /// <summary>
        /// Provoque une exeption si _class est null.
        /// </summary>
        /// <param name="_class"></param>
        public void AddClass(REClass _class)
        {
            _class.TreeAncestor = this;
            __classes.Add(_class);
        }

        public REClass Class(string _typeName)
        {
            foreach(REClass _class in __classes)
            {
                if(_class.TypeName == _typeName)
                    return _class;
            }
            return null;
        }

        public IEnumerable<REClass> Classes
        {
            get
            {
                foreach(REClass _class in __classes)
                    yield return _class;
            }
        }

        private List<REClass> __classes { get; set; } = new List<REClass>();

        public override REBase[] Children => __classes.ToArray();
    }
}
