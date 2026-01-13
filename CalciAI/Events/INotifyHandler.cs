using System.Threading.Tasks;

namespace CalciAI.Events
{
    public interface INotifyHandler<in T> where T : INotify
    {
        Task Handle(T evt);
    }
}
