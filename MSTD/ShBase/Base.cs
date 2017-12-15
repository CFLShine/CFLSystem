using System;

namespace MSTD.ShBase
{
    public abstract class Base
    {
        public Base()
        {}
        
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Display(AutoGenerateField = false)]
        public Guid ID { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Retourne une string de ce qui doit être affiché lorsque cet object 
        /// est représenté.
        /// Exemple : une classe Commune pourrait retourner Commune.Nom "Paris".
        /// </summary>
        public virtual string Display()
        {
            return "";
        }
    }   
}
