using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RuntimeExec
{
    public abstract class REStatement : REExpression
    {
        public override REBase TreeAncestor { get; set; } = null;

        [NotMapped]
        public bool Break { get; set; } = false;

        public override REBase ReValue 
        { 
            get => NULL;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Si <see cref="ReValue"/> retourne une <see cref="REExpression"/>,
        /// retourne <see cref="REExpression.CValue"/>.
        /// </summary>
        public override object CValue 
        { 
            get => (ReValue as REExpression)?.CValue; 
            set => throw new NotImplementedException(); 
        }
    }
}
