using System;
using System.Collections.Generic;
using System.Text;

namespace Dressly.Domain.DomainServices
{
    public interface IEstrategiaCombinacionColor
    {
        bool SonCompatibles(string colorA, string colorB);
    }
}
