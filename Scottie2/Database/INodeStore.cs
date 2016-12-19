using System.Collections.Generic;
using System.Collections.Immutable;
using Scottie.Server;

namespace Scottie.Database
{
    public interface INodeStore
    {
        void Init();

        string Create(long sessionId, string path, CreateMode createMode, string data);
        long Update(string path, string data, long version);
        bool Delete(string path, long version);
        void Multi(long sessionId, IEnumerable<MultiOpParams> operations);
        ZNode Get(string path);
        IImmutableList<string> GetChildren(string path);
    }
}