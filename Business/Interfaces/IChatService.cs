using System;
using System.Collections.Generic;
using Model.Chat;

namespace Business.Interfaces
{
    public interface IChatService
    {
        /// <summary>
        /// Gets All Pending messages for User Including Private
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        IEnumerable<ConverastionMessage> GetPendingMessagesForUser(long userId, long teamId);
        IEnumerable<ConverastionMessage> GetLattestPublicMessagesForUser(long userId, long teamId, DateTime time);
        IEnumerable<ConverastionMessage> GetLattestMessagesBetweenUsers(long userId1, long userId2, long teamId, DateTime addDays);

        void SaveMessageForUser(long senderId, ConverastionMessage message);
 
        void UpdatePendingMesagesForUser(string senederId, string receiverId, bool updatePublic);
        void UpdatePendingMesagesForUser(string senederId, bool updatePublic);

        int GetPendingTalksForUser(long userId, long teamId);


        IEnumerable<ConverastionMessage> GetPrivatePendingMessagesForUserFromSender(long userId, long senederId, long teamId);
    }
}