using System;
using System.Runtime.Serialization;

namespace CFL_1.CFL_System.SqlServerOrm
{
    /// <summary>
    /// Encapsule la représentation d'un objet présent dans la DB.
    /// <see cref="DBNotification"/>, par exemple, tient une liste de <see cref="DBObject"/>
    /// correspondant au objets qui ont été insérés ou modifiés.
    /// </summary>
    public class DBObject
    {
        public DBObject(){}

        public DBObject(string _tableName, Guid _id)
        {
            TableName = _tableName;
            ID = _id;
        }

        [DataMember]
        public string TableName { get; set; }

        [DataMember]
        public Guid ID { get; set; }
    }
}
