namespace NoSqlDataAccess.Entities.Team
{
    public class ConverastionsRepository : NoSqlRepositoryBase<Model.MyTeam.Converastion>
    {
        public override string CollectionName
        {
            get
            {
                return "Converastions";
            }
        }
    }
}