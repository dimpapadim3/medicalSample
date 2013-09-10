using System;
using ErrorClasses;

namespace Business.Interfaces
{
    public interface IEventsService
    {
        bool GetPopupEvent(long l, DateTime? getUserCreationDate, out bool energyEvent, out bool questionnaireEvent,
                           out bool weightEvent, out GenericError error);

        bool UpdateUserDates(long userId, DateTime? lastQ, DateTime? lastEn);

        bool GetUserDates(long id, out DateTime? latestIncludedDate, out DateTime? latestQuestionnaireDate,
                          out DateTime? latestEnergyLevelDate);


        void ClearUserDates(int userId);
    }
}