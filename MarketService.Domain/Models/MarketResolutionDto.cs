using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketService.Domain.Models
{
    public class MarketResolutionDto
    {
        public Guid Id { get; set; }
        public string MarketPubkey { get; set; } = default!;
        public int WinningOutcomeIndex { get; set; }
    }
}
