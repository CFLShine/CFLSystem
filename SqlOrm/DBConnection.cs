using System;
using System.Data;
using System.Windows;
using CFL_1.CFL_System.SqlServerOrm;
using Npgsql;

namespace SqlOrm
{
    public class DBConnection : IDisposable
    {
        public DBConnection()
        {}

        public NpgsqlConnection Connection
        {
            get;
            private set;
        }

        public bool Connect()
        {
            TryConnect();
            if(IsConnected && GetPID())
            {
                StartListening();
                return true;
            }
            return false;
        }

        public void Close()
        {
            if(Connection != null)
                Connection.Close();
        }

        private void TryConnect()
        {
            if(string.IsNullOrWhiteSpace(Server))
                throw new Exception("Serveur n'est pas renseigné.");
            if(string.IsNullOrWhiteSpace(Password))
                throw new Exception("Password n'est pas renseigné.");
            if(string.IsNullOrWhiteSpace(DataBase))
                throw new Exception("DataBase n'est pas renseigné.");
            if(string.IsNullOrWhiteSpace(UserId))
                throw new Exception("UserId n'est pas renseigné.");

            if (Connection == null)
            { 
                Connection = new NpgsqlConnection(ConnectionString);
            }

            if (!IsConnected)
            {
                try
                {
                    Connection.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                return (Connection != null && Connection.State.HasFlag(ConnectionState.Open));
            }
        }

        #region PID

        private bool GetPID()
        {
            bool _ok;
            object _pid = ExecuteScalar("SELECT pg_backend_pid();" ,out _ok);
            if (_pid != null && _ok)
                PID = (int)_pid;
            else
                PID = -1;
            return _ok;
        }

        public int PID { get; private set; } = -1;

        #endregion PID
        
        #region connection string elements

        public string ConnectionString
        {
            get
            {
                return string.Format("Server={0};User Id={1};Password={2};Database={3};Port=5432",
                Server, UserId, Password, DataBase);
            }
        }

        public string Server
        {
            get
            {
                return __server;
            }
            set
            {
                __server = value;
            }
        }

        public string DataBase
        {
            get
            {
                return __dbname;
            }
            set
            {
                __dbname = value.ToLower();
            }
        }

        public string UserId
        {
            get
            {
                return __userId;
            }
            set
            {
                __userId = value.ToLower();
            }
        }

        public string Password
        {
            get
            {
                return __password;
            }
            set
            {
                __password = value.ToLower();
            }
        }

        #endregion connection string elements

        #region Notification

        public bool NotifyChanges{ get; set; }

        /// <summary>
        /// Envoie une notification à tous les utilisateurs connectés à cette db,
        /// y compris à l'envoyeur.
        /// <see cref="DBSaveChanges"/> invoque cette méthode lors d'une sauvegarde 
        /// si <see cref="NotifyChanges"/> est mis à true.
        /// </summary>
        public bool Notify(DBNotification _notification)
        {
            return true;
            //if(_notification == null)
            //    throw new ArgumentNullException("_notification");
            //_notification.TimeOfDispatch = DateTime.Now;
            //return ExecuteNonQuery(new NpgsqlCommand("NOTIFY applinotification,"  + 
            //                                  "'" + _notification.Notification + "';"));
        }

        /// <summary>
        /// Le Guid pour identifier l'utilisateur qui envoie une notification,
        /// si l'application prévoit un système d'utilisateurs.
        /// C'est donc à l'application de fournir ce Guid.
        /// </summary>
        public Guid UserGuid { get; set; } = Guid.Empty;

        public delegate void OnNotificationEvent(DBNotification _notification);
        public OnNotificationEvent NotificationEvent;

        private void OnNotification(object sender, NpgsqlNotificationEventArgs e)
        {
            if(NotificationEvent != null)
            {
                DBNotification _notification = DBNotification.Factory(this, e);
                NotificationEvent.Invoke(_notification);
            }
        }

        private void StartListening()
        {
            if(Connection != null)
            {
                Connection.Notification -= OnNotification;
                Connection.Notification += OnNotification;
                ExecuteNonQuery(new NpgsqlCommand("LISTEN applinotification;"));
            }
        }

        private void StopListening()
        {
            if(Connection != null)
            {
                ExecuteNonQuery(new NpgsqlCommand("UNLISTEN applinotification;"));
            }
        }

        #endregion Notification

        #region execute query

        /// <summary>
        /// Si ShowMessage est à true, les messages d'erreurs lors de l'execution d'une commande
        /// sont montrés dans un MessageBox.
        /// </summary>
        public bool ShowMessages { get; set; }

        public bool ExecuteNonQuery(NpgsqlCommand _command)
        {
            try
            {
                _command.Connection = Connection;
                _command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                //if(ShowMessages)
                    MessageBox.Show(ex.StackTrace + Environment.NewLine + 
                                    "Error : " + Environment.NewLine + 
                                    ex.Message + Environment.NewLine + 
                                    "Query : " + Environment.NewLine +
                                    _command.CommandText);
            }
            return false;
        }

        public NpgsqlDataReader ExecuteQuery(NpgsqlCommand _command)
        {
            try
            {
                _command.Connection = Connection;
                NpgsqlDataReader _reader =  _command.ExecuteReader();
                return _reader;
            }
            catch (Exception ex)
            {
                //if(ShowMessages)
                //{
                    MessageBox.Show(ex.StackTrace + Environment.NewLine + 
                                    "Error : " + Environment.NewLine + 
                                    ex.Message + Environment.NewLine + 
                                    "Query : " + Environment.NewLine +
                                    _command.CommandText);
                //}
            }
                    return null;
        }

        public object ExecuteScalar(string _query, out bool _ok)
        {
            _ok = false;
            NpgsqlCommand _command = new NpgsqlCommand(_query, Connection);
            object _o = null;
            try
            {
                _o = _command.ExecuteScalar();
                _ok = true;
                return _o;
            }
            catch (Exception _ex)
            {
                //if(ShowMessages)
                //{
                    MessageBox.Show(_ex.StackTrace + Environment.NewLine + 
                                    "Error : " + Environment.NewLine + 
                                    _ex.Message + Environment.NewLine + 
                                    "Query : " + Environment.NewLine +
                                    _command.CommandText);
                //}
            }
            return null;
        }

        #endregion execute query

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool _disposing)
        {
            if(!__disposed)
            {
                Close();
                __disposed = true;
            }
        }

        private bool __disposed = false;

        #endregion Dispose

        private string __server   = "";
        private string __dbname   = "";
        private string __userId   = "";
        private string __password = "";
    }
}
