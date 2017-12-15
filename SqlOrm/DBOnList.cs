using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using MSTD;

// ARRAY[1,4,3] OVERLAPS  ARRAY[2,1]  => retourne true si les deux tableaux ont des éléments en commun.
// select cast (string_to_array('ab,cd,ef', ',') as text[]  => convertit la chaine en un tableau de strings

namespace SqlOrm
{
    /// <summary>
    /// <see cref="DBOnList"/> est utile pour selectionner les objets dont un membre liste contient un
    /// ou des éléments donnés directement ou par un select.
    /// A utiliser dans le Where d'un <see cref="DBSelect"/>,
    /// ex : 
    /// Une classe MyClass contenant un membre myList Liste de classes Identite : 
    /// new DBSelect("*").From("myclass")
    ///                  .Where(
    ///                         new DBOnList(()=>((MyClass)null).myList)
    ///                             .Contains( 
    ///                                         new DBSelectMember(()=>((Identite)null))
    ///                                             .Where(new MemberPath(()=>((Identite)null).Nom), "=", "DUPONT") 
    ///                                      )
    ///                         )
    /// </summary>
    public class DBOnList : DBQueryable
    {
        /// <summary>
        /// path exprime le membre liste de la classe sur laquelle est effectuée la recherche.
        /// ex : MyClass.myList
        /// </summary>
        public DBOnList(MemberPath path)
        {
            Path = path;
        }

        public DBOnList(Expression<Func<object>> expression)
        {
            Path = new MemberPath(expression);
        }

        public DBOnList Contains(DBSelect select)
        {
            Select = select;
            return this;
        }

        public override string Query()
        {
            if(Select.SelectedMembers.Count == 0)
                Select.Select("objectrepresentation");
            return string.Concat(
                "(string_to_array(", Path.LastPropertyName.ToLower(), ", ',')) && ARRAY (", Select.Query(), ")"); 
        }

        public MemberPath Path {get; private set; } 

        public DBSelect Select { get; private set; }
    }

    // select * from defunt where 
    // (string_to_array(operationsfune, ',') as TEXT[] ) OVERLAPS  (SELECT objectrepresentation FROM inhumation WHERE ... )
}
