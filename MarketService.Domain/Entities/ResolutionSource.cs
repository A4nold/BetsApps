using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketService.Domain.Entities
{
    public enum ResolutionSource
    {
        Unknown = 0,
        ManualAdmin = 1,
        ExternalApi = 2,
        Oracle = 3
    }
}
