

using System;

namespace RuntimeExec
{
    public class REBreak : REStatement
    {
        public override REExpression Invoke()
        {
            REBase _ancestor = FindAncestorOfType(typeof(RELoop));
            if(_ancestor != null)
            {
                ((RELoop)_ancestor).Break = true;
            }
            else
                throw new Exception(this.GetType().Name + " n'a pas trouvé l'encêtre auquel transmetre un break.");
            return this;
        }

        public override REBase[] Children => new REBase[0];

        public override REBase Copy()
        {
            return new REBreak();
        }

        public override REExpression Update(REClassObject _object)
        { 
            //do nothing
            return this;
        }
    }
}
