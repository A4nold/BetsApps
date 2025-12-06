using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketService.Domain.Entities
{
    public class MarketOutcome
    {
        public int Id { get; set; }

        public Guid MarketId { get; set; }
        public Market Market { get; set; } = default!;

        public int OutcomeIndex { get; set; }
        public string Label { get; set; } = default!;
    }
}
