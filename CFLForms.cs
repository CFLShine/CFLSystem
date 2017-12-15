using CFL_1.CFLGraphics;
using CFL_1.CFLGraphics.Editor;
using System.Collections.Generic;
using CFL_1.CFL_System.SqlServerOrm;
using CFL_1.CFL_System.DB;

namespace CFL_1.CFL_System
{
    /// <summary>
    /// Singleton.
    /// Instancie stock et donne accessibilité aux <seealso cref="CFLForm"/> et à la <seealso cref="CFLForm"/> courante.
    /// </summary>
    class CFLForms
    {
        public MainWindow mainWindow ;
        public Form_accueuil form_accueil ;
        public Form_config_entreprise form_config_entreprise;
        public Form_config_connection form_config_connection ;
        public Form_user form_user ;
        public Form_planing_journalier form_planingJournalier;
        public Form_codeEditor form_codeEditor ;
        public Form_test form_test ;


        #region currentForm

        public CFLForm currentForm
        {
            get
            {
                return __currentForm;
            }

            set
            {
                __currentForm = value ;
                mainWindow.currentForm = __currentForm ;
                __currentForm.Visibility = System.Windows.Visibility.Visible ;
                __currentForm.BecomeCurrent() ;
            }
        }

        private CFLForm __currentForm;

        #endregion currentForm

        public void initForms()
        {
            form_accueil           = new Form_accueuil();
            form_config_entreprise = new Form_config_entreprise();
            form_config_connection = new Form_config_connection();
            form_user              = new Form_user();
            form_planingJournalier = new Form_planing_journalier();
            form_codeEditor        = new Form_codeEditor();
            form_test              = new Form_test();

            forms.Add(form_accueil);
            forms.Add(form_config_entreprise);
            forms.Add(form_config_connection);
            forms.Add(form_user);
            forms.Add(form_planingJournalier);
            forms.Add(form_codeEditor);

            forms.Add(form_test);
        }

        /// <summary>
        /// Liste des CFL_form de CFL.
        /// </summary>
        public List<CFLForm> forms
        {
            get
            {
                if(__forms == null)
                    __forms = new List<CFLForm>();
                return __forms;
            }
        }

        private List<CFLForm> __forms;

        // Singleton:
        private CFLForms()
        {}

        public static readonly CFLForms instance = new CFLForms();
    }
}
