
using System.Runtime.Serialization;
using CFL_1.CFL_System;

namespace CFL_1.CFL_System
{
    [DataContract]
    public class CFLConfig 
    {
        public CFLConfig()
        {}
        
        public bool IsComplete()
        {
            return !string.IsNullOrWhiteSpace(Hostname)
                && !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password)
                && !string.IsNullOrWhiteSpace(Dbname);
        }

        // DB
        [DataMember]
        public string Hostname { get { return __hostname; } set { __hostname = value; } }
        [DataMember]
        public string Username { get { return __username; } set { __username = value; } }
        [DataMember]
        public string Password { get { return __password; } set { __password = value; } }
        [DataMember]
        public string Dbname   { get { return __dbname  ; } set { __dbname = value; } }
        //


        //private:
        private string __hostname = "";
        private string __username = "";
        private string __password = "";
        private string __dbname   = "";
    }   
}       
