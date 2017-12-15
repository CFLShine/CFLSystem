

using SqlOrm;

namespace CFL_1.CFL_System.DB
{
    public class CFLDBConnection : DBConnection
    {
        private CFLDBConnection()
        {}

        public CFLConfig Config
        {
            get
            {
                if(__config == null)
                    __config = new CFLConfig();
                return __config;
            }

            set
            {
                __config = value;
            }
        }

        public static CFLDBConnection instance
        {
            get
            {
                if(__unicInstance == null)
                {
                    __unicInstance = new CFLDBConnection();
                    // TODO remplacer par les données extraites de Config
                    __unicInstance.Server = "127.0.0.1";
                    __unicInstance.DataBase = "cfldb";
                    __unicInstance.UserId = "cfladmin";
                    __unicInstance.Password = "cfladminpwd";
                    //

                    __unicInstance.NotifyChanges = true;
                    __unicInstance.ShowMessages = true;
                    __unicInstance.Connect();
                }
                return __unicInstance;
            }
        }
        
        private static CFLDBConnection __unicInstance = null;
        private CFLConfig __config;
    }
}
