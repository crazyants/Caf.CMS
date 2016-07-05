using System;

namespace CAF.Infrastructure.Core
{
    public interface IOrdered
    {
        // TODO: (MC) Make Nullable!
        int Ordinal { get; }
    }
}
