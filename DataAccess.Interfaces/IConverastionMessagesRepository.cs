using ErrorClasses;
using Model.Chat;

namespace DataAccess.Interfaces
{
    public interface IConverastionMessagesRepository : IRepositoryBase<ConverastionMessage>
    {
        void UpdateRecord(long receiverId, bool isPending);
        void UpdateRecord(long senderuserId, long receiverId, bool isPending);
        void Update(out GenericError error);
    }
}