using System.Collections.Immutable;

namespace Scottie.Database
{
    public interface ISessionStore
    {
        void Init();

        long Create();
        void Heartbeat(long id);
        void Delete(long id);
        bool Exists(long id);
        IImmutableList<Session> GetAllSessions();
    }
}