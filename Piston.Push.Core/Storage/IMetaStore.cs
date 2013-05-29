//     IMetaStore.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using System.Collections.Generic;

namespace Piston.Push.Core
{
    public interface IMetaStore
    {
        void Save(AppMetadata app);
        void Delete(string appId);
        IEnumerable<AppMetadata> GetApps();
    }
}
