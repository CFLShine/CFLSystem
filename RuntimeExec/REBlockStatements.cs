using System.Collections.Generic;

namespace RuntimeExec
{
    public class REBlockStatements : REStatement
    {
        public REBlockStatements(){ }

        public override REBase[] Children 
        {
            get
            {
                if(__statements != null)
                {
                    REBase[] _children = new REExpression[__statements.Count];
                    int _i = 0;
                    foreach(REStatement _statement in __statements)
                    {
                        _children[_i] = _statement;
                        _i++;
                    }
                    return _children;
                }
                return new REBase[0];
            }
        }

        public override REBase Copy()
        {
            REBlockStatements _block = new REBlockStatements();
            if(__statements != null)
            {
                foreach(REStatement _statement in __statements)
                {
                    _block.AddStatement((REStatement)_statement.Copy());
                }
            }
            return _block;
        }

        public override REExpression Update(REClassObject _object)
        {
            if(__statements != null)
            {
                foreach(REStatement _statement in __statements)
                {
                    _statement.Update(_object);
                }
            }
            return this;
        }

        public List<REStatement> Statements
        {
            get => __statements;
            set
            {
                __statements = new List<REStatement>();
                foreach(REStatement _statement in value)
                {
                    AddStatement(_statement);
                }
            }
        }

        public void AddStatement(REStatement _statement)
        {
            __statements.Add(_statement);
            _statement.TreeAncestor = this;
        }

        public void AddStatements(params REStatement[] _statements)
        {
            foreach(REStatement _statement in _statements)
                AddStatement(_statement);
        }

        public override REExpression Invoke()
        {
            if(__statements != null)
            {
                foreach(REStatement _statement in __statements)
                {
                    if(Break == true)
                    {
                        if(TreeAncestor is RELoop _loop)
                            _loop.Break = true;
                        break;
                    }
                    _statement.Invoke();
                }
            }
            return this;
        }

        private List<REStatement> __statements { get; set; } = new List<REStatement>();
    }
}
