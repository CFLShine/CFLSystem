using System;
using System.IO;
using System.Windows;
using CFL_1.CFLGraphics;
using CFL_1.CFL_Data;
using MSTD;
using System.Collections.Generic;

namespace CFL_1.CFL_System
{
    public static class Gate
    {
        /// <summary>
        /// Crèe si necessaire et retourne le chemin vers le dossier pour la sauvegarde de données nécessaires en local,
        /// (ex avant connection, comme la config qui donne les éléments de connection à la DB).
        /// </summary>
        /// <returns></returns>
        public static string localPath()
        {
            string _dir = string.Empty;
            try
            {
                /* si l'on décide que le dossier local soit dans le dossier d'instalation de l'appli :
                 * dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CFL_local" */
                _dir = @"C:\CFL_local";
                Directory.CreateDirectory(_dir);
                return _dir;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.TargetSite + " : " + e.Message + "\n Dir path : " + _dir);
                return "";
            }
        }

        public static string configPath()
        {
            return Path.Combine(localPath(), "Config.txt");
        }

        public static string CommunesPath()
        {
            return Path.Combine(localPath(), "Communes.txt");
        }

        public static class Load
        {
            public static bool config(ref CFLConfig _config)
            {
                if(!File.Exists(configPath()))
                    return false;
                _config = JSONRelation<CFLConfig>.deserialize(File.ReadAllText(Path.Combine(localPath(), "Config.txt")));
                return true;
            }

            public static bool Communes(ref List<Commune> _communes)
            {
                _communes = new List<Commune>();
                string _path = CommunesPath();
                if(!File.Exists(_path))
                {
                    MessageBox.Show(@"Fichier d'initialisation des communes manquants : " + Environment.NewLine +
                                     _path );
                    return false;
                }

                string _line = "";

                try
                {
                    StreamReader _sr = new StreamReader(_path);

                    while((_line = _sr.ReadLine()) != null)
                    {
                        string[] _elements = _line.Split(';');
                        
                        Guid _id = Guid.Empty;

                        Guid.TryParse(_elements[0], out _id);

                        Commune _commune = new Commune()
                        {
                            ID = _id,
                            nom = _elements[1], 
                            codePost = _elements[2]
                        };
                        _communes.Add(_commune);
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show("Gate.load.communes(ref List<Tuple<string, string>> _list) : " + e.Message);
                    return false;
                }
                return true;
            }
        } 

        public static class Save
        {
            public static void config(CFLConfig _config)
            {
                File.WriteAllText(configPath(), JSONRelation<CFLConfig>.serialize(_config));
            }
        }
    }
}
