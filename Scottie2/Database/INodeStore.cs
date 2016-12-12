﻿using System.Collections.Generic;
using System.Collections.Immutable;
using Scottie.Server;

namespace Scottie.Database
{
    public interface INodeStore
    {
        void Init();

        string Create(string path, string createMode, string data);
        long Update(string path, string data, long version);
        bool Delete(string path, long version);
        void Multi(IEnumerable<MultiOpParams> operations);
        ZNode Get(string path);
        IImmutableList<string> GetChildren(string path);
    }
}