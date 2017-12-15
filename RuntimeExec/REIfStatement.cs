using System;

namespace RuntimeExec
{
    public class REIfStatement : REStatement
    {
        public REIfStatement(){ }

        public REIfStatement(REExpression condition, REStatement thenStatement, REStatement elseStatement = null)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseStatement = elseStatement;
        }

        public override REBase Copy()
        {
            REExpression _condition = (Condition != null)? (REExpression)Condition.Copy() : null;
            REStatement _thenStatement = (ThenStatement != null)? (REStatement)ThenStatement.Copy() : null;
            REStatement _elseStatement = (ElseStatement != null)? (REStatement)ElseStatement.Copy() : null;

            return new REIfStatement(_condition, _thenStatement, _elseStatement);
        }

        public REExpression Condition
        {
            get => __condition;
            set
            {
                __condition = value;
                if(__condition != null)
                    __condition.TreeAncestor = this;
            }
        }

        public REStatement ThenStatement 
        { 
            get => __thenStatement;
            set
            {
                __thenStatement = value;
                if(__thenStatement != null)
                    __thenStatement.TreeAncestor = this;
            }
        }

        public REStatement ElseStatement 
        { 
            get => __elseStatement;
            set
            {
                __elseStatement = value;
                if(__elseStatement != null)
                    __elseStatement.TreeAncestor = this;
            }
        }

        public override REExpression Invoke()
        {
            if(REHelper.Equal(Condition.CValue, true))
                ThenStatement.Invoke();
            else
            {
                if(ElseStatement != null)
                    ElseStatement.Invoke();
            }
            return this;
        }

        public override REExpression Update(REClassObject _object)
        {
            if(Condition != null)
                Condition.Update(_object);
            if(ThenStatement != null)
                ThenStatement.Update(_object);
            if(ElseStatement != null)
                ElseStatement.Update(_object);
            return this;
        }

        public override REBase[] Children => new REBase[3]{ Condition, ThenStatement, ElseStatement };

        private REExpression __condition = null;
        private REStatement __thenStatement = null;
        private REStatement __elseStatement = null;
    }
}
