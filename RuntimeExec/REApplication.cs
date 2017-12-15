using System.Collections.Generic;

namespace RuntimeExec
{
    public class REApplicattion : REBase
    {
        public override REBase Copy()
        {
            REApplicattion _application = new REApplicattion();
            foreach(RENameSpace _nameSpace in __nameSpaces)
            {
                _application.AddNameSpace((RENameSpace)_nameSpace.Copy());
            }
            return _application;
        }

        public void AddNameSpace(RENameSpace _nameSpace)
        {
            _nameSpace.TreeAncestor = this;
            __nameSpaces.Add(_nameSpace);
        }

        public RENameSpace NameSpace(string _name)
        {
            foreach(RENameSpace _nameSpace in __nameSpaces)
            {
                if(_nameSpace.Name == _name)
                    return _nameSpace;
            }
            return null;
        }

        private List<RENameSpace> __nameSpaces { get; set; } = new List<RENameSpace>();

        public override REBase[] Children => __nameSpaces.ToArray();
    }
}
