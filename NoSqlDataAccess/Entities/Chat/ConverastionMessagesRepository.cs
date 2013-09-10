using System.Linq;
using DataAccess.Interfaces;
using Model.Chat;
using MongoDB.Driver.Linq;

namespace NoSqlDataAccess.Entities.Chat
{
    public class ConverastionMessagesRepository : NoSqlRepositoryBase<ConverastionMessage>, IConverastionMessagesRepository
    {
        public override string CollectionName
        {
            get
            {
                return "ConversationgMessages";
            }
        }


        public void UpdateRecord(long receiverId, bool isPending)
        {

            var messages = Collection.AsQueryable().Where(m => m.Receivers.Any(r => r.UserId == receiverId) && !m.IsPrivate);


            messages.ToList().ForEach(m =>
                {
                    m.Receivers.Where(r => r.UserId == receiverId).ToList().ForEach(r => r.IsPending = isPending );

                    Collection.Save(m);
                });
        }


        public void UpdateRecord(long senderuserId, long receiverId, bool isPending)
        {

            var messages = Collection.AsQueryable().Where(m => m.Receivers.Any(r => r.UserId == receiverId) && m.SenderUserId==senderuserId && m.IsPrivate);


            messages.ToList().ForEach(m =>
            {
                m.Receivers.Where(r => r.UserId == receiverId).ToList().ForEach(r => r.IsPending = isPending);

                Collection.Save(m);
            });
            
        }

    }
}