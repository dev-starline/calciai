using CalciAI.Models;

namespace CalciAI.Persistance
{
    public interface IMongoCommand
    {

    }

    public abstract class MongoOperatorCommand : IMongoCommand
    {
        public string OperatorId { get; }

        protected MongoOperatorCommand(string operatorId)
        {
            OperatorId = operatorId;
        }
    }

    public abstract class MongoClientUserCommand : IMongoCommand
    {
        public OperatorUserId UserId { get; }

        protected MongoClientUserCommand(OperatorUserId userId)
        {
            UserId = userId;
        }
    }

    public abstract class MongoMasterUserCommand : IMongoCommand
    {
        public OperatorUserId MasterUserId { get; }

        public string MasterPassword { get; }

        protected MongoMasterUserCommand(OperatorUserId masterUserId, string masterPassword)
        {
            MasterUserId = masterUserId;
            MasterPassword = masterPassword;
        }
    }

    public abstract class MongoMasterOperationCommand : IMongoCommand
    {
        public OperatorUserId MasterUserId { get; }

        public string MasterPassword { get; }

        public OperatorUserId UserId { get; }

        protected MongoMasterOperationCommand(OperatorUserId masterUserId, string masterPassword, OperatorUserId userId)
        {
            MasterUserId = masterUserId;
            MasterPassword = masterPassword;
            UserId = userId;
        }
    }
}
