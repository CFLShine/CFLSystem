

namespace RuntimeExec
{
    public class REWhile : RELoop
    {

        public REWhile(){ }

        public REWhile(REExpression _condition)
        {
            Condition = _condition;
        }

        public REWhile(REExpression _condition, REStatement _statement)
        {
            Condition = _condition;
            Statement = _statement;
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

        public REStatement Statement 
        { 
            get => __Statement;
            set
            {
                __Statement = value;
                if(__Statement != null)
                    __Statement.TreeAncestor = this;
            }
        }

        public override REBase Copy()
        {
            return new REWhile((REExpression)Condition.Copy(), (REStatement)Statement.Copy());
        }

        public override REExpression Update(REClassObject _object)
        {
            if(Condition != null)
                Condition.Update(_object);
            if(Statement != null)
                Statement.Update(_object);
            return this;
        }

        public override REBase ReValue 
        { 
            get
            {
                return new REValue(null);
            }
            set => throw new System.NotImplementedException(); 
        }

        public override object CValue 
        { 
            get
            {
                REBase _reValue = ReValue;
                return (_reValue is REExpression _expr)? _expr.CValue : null;
            }
            set => throw new System.NotImplementedException(); 
        }

        public override REExpression Invoke()
        {
            if(Condition != null && Statement != null)
            {
                while(REHelper.Equal(Condition.CValue, true))
                {
                    if(Break)
                        break;

                    Statement.Invoke();
                }
            }
            return this;
        }

        public override REBase[] Children => new REBase[2]{ Condition, Statement };

        private REExpression __condition = null;
        private REStatement __Statement = null;

    }
}
