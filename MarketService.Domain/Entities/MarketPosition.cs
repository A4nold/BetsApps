using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketService.Domain.Entities
{
    public class MarketPosition
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; } //Auth service user id
        public Guid MarketId { get; set; }
        public Market Market { get; set; } = default!;

        public int OutcomeIndex { get; set; } //which outcome
        public ulong StakeAmount { get; set; }

        public string TxSignature { get; set; } = default!;
        public DateTime PlacedAt {  get; set; }
        public bool Claimed { get; set; }
        public DateTime? ClaimedAt { get; set; }
    }
}
