using CalciAI.Models;
using System.Threading.Tasks;

namespace CalciAI.Persistance
{
    public interface ISqlProcessor<T> where T : ISqlCommand
    {
        ValueTask<ProcessResult> Validate(T command);

        Task<ProcessResult> Execute(T command);
    }
}
