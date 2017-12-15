

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Npgsql;
using SqlOrm;

namespace CFL_1.CFL_System.SqlServerOrm
{

    /// <summary>
    /// [FROM][fromUserId][OBJECTS][_tableName1 id1,_tableName2 id2...]
    /// [FROM][fromUserId][ADDRESSES][Guid1,Guidd2,...][MESSAGE][message...]
    /// [USERCONNECT][FROM][userGuid][CONNECTED][true] ou [false]
    /// [TIMEDISPACH][datetime]
    /// </summary>
    [DataContract]
    public class DBNotification
    {
        public DBNotification(){ }

        private enum KeyElement
        {
            UNKNOWN,
            FROM,
            OBJECTS,
            ADDRESSES,
            CONNECTED,
            MESSAGE,
            TIMEDISPATCH
        }

        public static DBNotification Factory(DBConnection _connection, NpgsqlNotificationEventArgs e)
        {
            //DBNotification _notification = new DBNotification();
            //_notification.Notification = e.AdditionalInformation;
            //_notification.IsCorrupted = (_notification.Init() == false);
            //_notification.IsNotifiedByMe = (e.PID == _connection.PID);
            DBNotification _notification = null;
            MemoryStream _ms = new MemoryStream(Encoding.UTF8.GetBytes(e.AdditionalInformation));
            DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(DBNotification));
            _notification = (DBNotification)_serializer.ReadObject(_ms);
            _ms.Close();
            _notification.IsSentBySelf = (e.PID == _connection.PID);
            return _notification; 
        }

        public bool IsCorrupted { get; private set; }

        [DataMember]
        public Guid SenderGuid { get; set; }

        [DataMember]
        public bool IsConnectedSender { get; set; }

        /// <summary>
        /// Liste des Guid des utilisateurs auquels s'adresse la notification.
        /// Pour une notification qui vise tous les utilisateurs,
        /// la liste est vide.
        /// </summary>
        [DataMember]
        public List<Guid> Addressees { get; set; } = new List<Guid>();

        public bool DestinedToAllUsers
        {
            get
            {
                return(Addressees == null || Addressees.Count == 0);
            }

            set
            {
                if(value == true && Addressees != null)
                    Addressees.Clear();
            }
        }

        /// <summary>
        /// Est true si la notification a été envoyée 
        /// par le même utilisateur qui la reçoit.
        /// </summary>
        public bool IsSentBySelf
        { get; private set; }

        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Instant auquel la notification à été envoyée.
        /// </summary>
        [DataMember]
        public DateTime TimeOfDispatch { get; set; }

        /// <summary>
        /// List des entités qui ont été inserées ou modifiées  
        /// par un <see cref="DBSaveChanges"/>.
        /// </summary>
        [DataMember]
        public List<DBObject> Entities { get; set; } = new List<DBObject>();

        /// <summary>
        /// Ajoute un <see cref="DBObject"/> à <see cref="Entities"/>
        /// signifiant que l'entité de la table _tableName et d'id _entityID
        /// a été inserée ou modifiée par un <see cref="DBSaveChanges"/>
        /// </summary>
        public void AddEntity(string _tableName, Guid _entityID)
        {
            if(Entities == null)
                Entities = new List<DBObject>();
            Entities.Add(new DBObject(_tableName, _entityID));
        }

        /// <summary>
        /// Retourne true si la notification n'est pas corrompue.
        /// </summary>
        private bool Init()
        {
            int _at = 0;
            string _element = "";

            while(GetElement(ref _element, ref _at ))
            {
                KeyElement _key = GetKeyElement(_element);
                switch (_key)
                {
                    case KeyElement.UNKNOWN:
                        return false;
                    case KeyElement.FROM:
                        if(!GetElement(ref _element, ref _at))
                            return false;
                        if(!SetSenderGuid(_element))
                            return false;
                        break;
                    case KeyElement.OBJECTS:
                        if(!GetElement(ref _element, ref _at))
                            return false;
                        if(!AddConcernedObjects(_element))
                            return false;
                        break;
                    case KeyElement.ADDRESSES:
                        if(!GetElement(ref _element, ref _at))
                            return false;
                        AddAddressees(_element);
                        break;
                    case KeyElement.CONNECTED:
                        if(!GetElement(ref _element, ref _at))
                            return false;
                        if(!SetIsConnectedSender(_element))
                            return false;
                        break;
                    case KeyElement.MESSAGE:
                        if(!GetElement(ref _element, ref _at))
                            return false;
                        Message += _element;
                        break;
                    case KeyElement.TIMEDISPATCH:
                        if(!GetElement(ref _element, ref _at))
                            return false;
                        if(!SetTimeDispach(_element))
                            return false;
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }

        public string Notification
        {
            get
            {
                DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(DBNotification));
                MemoryStream _ms = new MemoryStream();

                _serializer.WriteObject(_ms, this);
                byte[] _json = _ms.ToArray();
                _ms.Close();
                string _s = Encoding.UTF8.GetString(_json, 0, _json.Length);
                return _s;
            }
        }

        private bool GetElement(ref string _element, ref int _at)
        {
            _element = "";

            if(_at == Notification.Length)
                    return false;

            char _c = Notification[_at];
            if(_c != '[')
                return false;

            int _opening = 1;

            while(true)
            {
                ++ _at;
                if(_at == Notification.Length)
                    return false;

                _c = Notification[_at];

                if(_c == '[')
                    ++_opening;

                if(_c == ']')
                {
                    --_opening;
                    ++_at;
                    if(_opening == 1)
                        return true;
                }
                _element += _c;
            }
        }

        KeyElement GetKeyElement(string _element)
        {
            switch (_element)
            {
                case "FROM":
                    return KeyElement.FROM;
                case "OBJECTS":
                    return KeyElement.OBJECTS;
                case "ADDRESSES":
                    return KeyElement.ADDRESSES;
                case "CONNECTED":
                    return KeyElement.CONNECTED;
                case "MESSAGE":
                    return KeyElement.MESSAGE;
                case "TIMEDISPATCH":
                    return KeyElement.TIMEDISPATCH;
            }
            return KeyElement.UNKNOWN;
        }

        private bool SetSenderGuid(string _element)
        {
            Guid _guid = Guid.Empty;
            if(!Guid.TryParse(_element, out _guid))
                return false;
            SenderGuid = _guid;
            return true;
        }

        private bool SetIsConnectedSender(string _element)
        {
            bool _bool = false;
            if(!bool.TryParse(_element.ToLower(), out _bool))
                return false;
            IsConnectedSender = _bool;
            return true;
        }

        private bool SetTimeDispach(string _element)
        {
            DateTime _dateTime = new DateTime();
            if(!DateTime.TryParse(_element, out _dateTime))
                return false;
            TimeOfDispatch = _dateTime;
            return true;
        }

        private void AddAddressees(string _element)
        {

        }

        private bool AddConcernedObjects(string _element)
        {
            if(string.IsNullOrWhiteSpace(_element))
                return true;

            string[] _objects = _element.Split(',');
            return true;
        }
    }
}
