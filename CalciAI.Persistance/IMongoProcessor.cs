using CalciAI.Models;
using System.Threading.Tasks;

namespace CalciAI.Persistance
{
    public interface IMongoProcessor<T> where T : IMongoCommand
    {
        ValueTask<ProcessResult> Validate(T command, OperatorUserId currentUser);

        Task<ProcessResult> Execute(T command, OperatorUserId currentUser);
    }
}
