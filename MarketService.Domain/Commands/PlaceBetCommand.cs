using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketService.Domain.Commands
{
    public class PlaceBetCommand
    {
        //Which outcome index (0-based) to bet on.
        public int OutcomeIndex { get; set; }
        //stake amount in smallest units (e.g. 100 USDC with 6 decimals)
        public ulong StakeAmount { get; set; }
        //User's usdc token account that will send funds (same ATA used when creating/claiming)
        public string UserCollateralAta { get; set; } = default!;
        //Market vault token accountthat receieves collateral (same vault used in create_market)
        public string VaultTokenAta { get; set; } = default!;

    }
}
