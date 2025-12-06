using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketService.Domain.Entities
{
    public enum MarketStatus
    {
        Open = 0,
        Locked = 1,
        Resolved = 2,
        Settled = 3,
        Cancelled = 4
    }
}
