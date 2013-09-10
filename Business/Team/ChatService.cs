using System;
using System.Collections.Generic;
using System.Linq;
using Business.Interfaces;
using DataAccess.Interfaces;
using ErrorClasses;
using Model.Chat;

using NoSqlDataAccess.Entities.Chat;

namespace Business.Team
{
    internal class ChatService : IChatService
    {
        public IConverastionMessagesRepository MessagesRepository { get; set; }

        #region Save
        public void SaveMessageForUser(long senderId, ConverastionMessage message)
        {
            GenericError error;

            message.SenderUserId = senderId;

            MessagesRepository.InsertEntity(out error, message);
        }
        #endregion

        #region Pending Messages
         
        public IEnumerable<ConverastionMessage> GetPendingMessagesForUser(long userId, long teamId)
        {
            GenericError error;
            List<ConverastionMessage> messages = MessagesRepository.GetEntities(out error, m => m.TeamId == teamId && m.Receivers.Any(r => r.UserId == userId && r.IsPending));
            if (messages != null) return messages;
            return new List<ConverastionMessage>();
        }

        public IEnumerable<ConverastionMessage> GetPublicPendingMessagesForUser(long userId, long teamId)
        {
            GenericError error;
            List<ConverastionMessage> messages = MessagesRepository.GetEntities(out error, m => m.TeamId == teamId && m.Receivers.Any(r => r.UserId == userId && r.IsPending) && !m.IsPrivate);
            if (messages != null) return messages;
            return new List<ConverastionMessage>();
        }

        public IEnumerable<ConverastionMessage> GetPrivatePendingMessagesForUser(long userId, long teamId)
        {
            GenericError error;
            List<ConverastionMessage> messages = MessagesRepository.GetEntities(out error, m => m.TeamId == teamId && m.Receivers.Any(r => r.UserId == userId && r.IsPending) && m.IsPrivate);
            if (messages != null) return messages;
            return new List<ConverastionMessage>();
        }

        public IEnumerable<ConverastionMessage> GetPrivatePendingMessagesForUserFromSender(long userId, long senederId, long teamId)
        {
            GenericError error;
            List<ConverastionMessage> messages = MessagesRepository.GetEntities(out error, m => m.TeamId == teamId && m.Receivers.Any(r => r.UserId == userId && r.IsPending) && m.IsPrivate && m.SenderUserId == senederId);
            if (messages != null) return messages;
            return new List<ConverastionMessage>();
        }

        #endregion

        #region Read  Messages

        public IEnumerable<ConverastionMessage> GetLattestMessagesForUser(long userId1, long teamId, DateTime time, bool includePending)
        {
            GenericError error;
            Func<ConverastionMessage, bool> selector = m => (m.Date.Date.CompareTo(time.Date) >= 0) && ((IsParticipant(userId1, m)) && m.TeamId == teamId);

            return MessagesRepository.GetEntities(out error, selector);

        }

        public IEnumerable<ConverastionMessage> GetLattestPublicMessagesForUser(long userId, long teamId, DateTime time)
        {
            GenericError error;
            Func<ConverastionMessage, bool> selector = m => (m.Date.Date.CompareTo(time.Date) >= 0) && ((IsParticipant(userId, m)) && m.TeamId == teamId && !m.IsPrivate);

            return MessagesRepository.GetEntities(out error, selector);
        }

        public IEnumerable<ConverastionMessage> GetLattestMessagesBetweenUsers(long userId1, long userId2, long teamId, DateTime time)
        {
            GenericError error;
            Func<ConverastionMessage, bool> selector = m => (m.Date.Date.CompareTo(time.Date) >= 0) && (IsParticipant(userId1, m) && IsParticipant(userId2, m)) && m.TeamId == teamId && m.IsPrivate;

            return MessagesRepository.GetEntities(out error, selector);
        }

        #endregion

        #region Update Pending

        public void UpdatePendingMesagesForUser(string senedrUserId, bool updatePublic)
        {

            ((ConverastionMessagesRepository)MessagesRepository).UpdateRecord(long.Parse(senedrUserId),
                                                                                      updatePublic);

        }

        public void UpdatePendingMesagesForUser(string senedrUserId, string receiverId, bool updatePublic)
        {
            ((ConverastionMessagesRepository)MessagesRepository).UpdateRecord(long.Parse(senedrUserId),
                                                                                      long.Parse(receiverId), false);
        }
        #endregion

        private static bool IsParticipant(long userId, ConverastionMessage message)
        {
            return (message.Receivers.Any(r => r.UserId == userId)) || message.SenderUserId == userId;
        }
        public int GetPendingTalksForUser(long userId, long teamId)
        {
            var memebers = User.GetCoreTeamUsers(teamId, -1);
            var count = 0;

            count += GetPublicPendingMessagesForUser(userId, teamId).Any() ? 1 : 0;

            memebers.ToList().ForEach(m =>
                {
                    count += GetPrivatePendingMessagesForUserFromSender(userId, m.Key, teamId).Any(s => s.IsPrivate) ? 1 : 0;
                });

            return count;
        }
    }

}