using System;
using System.Linq;

namespace Business
{
    public class UserSettings
    {

        public static NoSqlDataAccess.Entities.UserSettings UserSettingsRepository { get; set; }

        static UserSettings()
        {
            Initialize();
        }
        public static void Initialize()
        {
            UserSettingsRepository = new NoSqlDataAccess.Entities.UserSettings();
        }

        public static DateTime? GetLattestAnsweredQuestionaireDate(long userId)
        {
            var lattest = UserSettingsRepository.GetAsQueryable<Model.UserSettings>().FirstOrDefault(s => s.UserId== userId);
            if (lattest != null)
                return lattest.LastAnsweredQuestionnaireDate;
            return null;
        }

        public static DateTime? GetLastAnsweredEnergyLevelDate(long userId)
        {
            var lattest = UserSettingsRepository.GetAsQueryable<Model.UserSettings>().FirstOrDefault(s => s.UserId== userId);
            if (lattest != null)
                return lattest.LastAnsweredEnergyLevelDate;
            return null;
        }


        public static void UpdateLattestAnsweredQuestionaireDate(long userId, DateTime updatedTime)
        {
            UserSettingsRepository.UpdateLattestAnsweredQuestionaireDate(userId, updatedTime);
        }

        public static void UpdateLastAnsweredEnergyLevelDate(long userId,DateTime updatedTime)
        {
            UserSettingsRepository.UpdateLastAnsweredEnergyLevelDate(userId,updatedTime);
        }


    }
}
