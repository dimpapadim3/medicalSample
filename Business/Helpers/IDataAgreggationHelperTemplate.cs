using System;
using System.Collections.Generic;

namespace Business.Helpers
{
    public interface IDataAgreggationHelperTemplate<TModel, TBaseModel, TServiceModel>
    {
        void UpdateEntityRecords(TBaseModel baseEntity);
        IList<TServiceModel> GetInfoView(int userid, DateTime now, int numberOfMonthsToDisplay);
    }
}