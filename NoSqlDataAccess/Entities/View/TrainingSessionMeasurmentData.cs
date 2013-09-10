namespace NoSqlDataAccess.Entities.View
{
    public class TrainingSessionMeasurmentDataRepository : NoSqlRepositoryBase<Model.View.TrainingSessionMeasurmentData>
    {
        public override string CollectionName
        {
            get
            {
                return "TrainingSessionMeasurmentData";
            }
        }
    }
}
