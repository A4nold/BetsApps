using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketService.Domain.Entities
{
    public class Market
    {
        public Guid Id { get; set; }

        //On-chain reference (Anchor Market Account)
        public string MarketPubKey { get; set; } = default!;

        public string Question { get; set; } = default!;
        public DateTime EndTime { get; set; }
        public MarketStatus Status { get; set; }

        public Guid CreatorUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte? WinningOutcomeIndex { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? SettledAt { get; set; }

        public ICollection<MarketOutcome> Outcomes { get; set; } = new List<MarketOutcome>();
        public MarketResolution? Resolution { get; set; }
    }
}
