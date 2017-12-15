
using CFL_1.CFL_Data;
using CFL_1.CFL_Data.Defunts;
using CFL_1.CFL_Data.Thanatopraxie;
using CFL_1.CFLGraphics.MyControls.GraphEditor;
using MSTD.ShBase;
using SqlOrm;

namespace CFL_1.CFL_System.DB
{
    public class DBContext_CFL : ShContext
    {
        private DBContext_CFL()
        {
            CreateOrCompleteDataBase();
        }

        private static DBContext_CFL __instance = null;
        public static DBContext_CFL instance
        {
            get
            {
                if(__instance == null)
                    __instance = new DBContext_CFL();
                return __instance;
            }
        }

        public void CreateOrCompleteDataBase()
        {
            DBCreation _creator = new DBCreation(Connection, this);
            _creator.CreateOrCompleteDataBase();
        }

        public DBConnection Connection
        {
            get => CFLDBConnection.instance;
        }

        public bool SaveChanges()
        {
            DBSaveChanges _saver = new DBSaveChanges(this, Connection);
            return _saver.Exe();
        }

        public Set<GraphProject>        GraphProject       { get; set; }
        public Set<ShapeTypeInfo>       ShapeTypeInfo      { get; set; }
                                                           
        public Set<PlaningJournalier>  Planing_journalier  { get; set; }
        public Set<PageJour>            PageJour           { get; set; }
        public Set<ZoneInfo>            ZoneInfo           { get; set; }
        public Set<ZoneAction>          ZoneAction         { get; set; }

        public Set<Autorisation>        Autorisation       { get; set; }

        public Set<Commune>             Commune            { get; set; }

        public Set<Lieu>                Lieu               { get; set; }

        public Set<Entreprise>          Entreprise         { get; set; }

        public Set<ChambreFuneraire>    ChambreFuneraire   { get; set; }
        public Set<Salon>               Salon              { get; set; }
        public Set<CaseRefrigeree>                Case               { get; set; }

        public Set<Pf>                  Pf                 { get; set; }

        public Set<Crematorium>         Crematorium        { get; set; }
        public Set<Four>                Four               { get; set; }

        public Set<Defunt>              Defunt             { get; set; }
        public Set<Deces>               Deces              { get; set; }

        public Set<Naissance>           Naissance          { get; set; }
        public Set<SituationFamiliale>  SituationFamillale { get; set; }
        public Set<Filiation>           Filiation          { get; set; }
        public Set<Pouvoir>             Pouvoir            { get; set; }
        public Set<Parent>              Parent             { get; set; }

        public Set<Cimetiere>           Cimetiere          { get; set; }
        public Set<Sepulture>           Sepulture          { get; set; }

        public Set<StagiaireThanato>    StagiaireThanato   { get; set; }

        public Set<OperationFune>       OperationFune      { get; set; }
        public Set<Soin>                Soin               { get; set; }
        public Set<MEB>                 MEB                { get; set; }
        public Set<Ceremonie>           Ceremonie          { get; set; }
        public Set<Transport>           Transport          { get; set; }
        public Set<Cremation>           Cremation          { get; set; }
        public Set<Inhumation>          Inhumation         { get; set; }
        public Set<Dispersion>          Dispersion         { get; set; }
        public Set<Exhumation>          Exhumation         { get; set; }
        public Set<RemiseCendres>       RemiseCendres      { get; set; }
        public Set<AutreOperation>      AutreOperation     { get; set; }

        public Set<Vehicule>            Vehicule           { get; set; }

        public Set<RaisonSociale>       Raison_sociale     { get; set; }

        public Set<Personne>            Personne           { get; set; }

        public Set<Metier>              Metier             { get; set; }
        public Set<Utilisateur>         Utilisateur        { get; set; }
        public Set<Login>               Login              { get; set; }
        public Set<Identite>            Identite           { get; set; }
        public Set<Coordonnees>         Coordonnees        { get; set; }
        public Set<Contacts>            Contacts           { get; set; }

    }
}
