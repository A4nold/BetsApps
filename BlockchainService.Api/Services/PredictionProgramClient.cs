using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Models;
using Solnet.Wallet;

namespace BlockchainService.Api.Services
{
    public class SolanaOptions
    {
        public string RpcUrl { get; set; } = default!;
        public string ProgramId { get; set; } = default!;
        // We won’t use AuthorityKeypairPath for now
        public string AuthorityKeypairPath { get; set; } = default!;
    }

    public class CreateMarketResult
    {
        public string MarketPubkey { get; set; } = default!;
        public string TransactionSignature { get; set; } = default!;
    }

    public class PredictionProgramClient
    {
        private readonly IRpcClient _rpc;
        private readonly Account _authority;
        private readonly PublicKey _programId;

        public PredictionProgramClient(IOptions<SolanaOptions> options)
        {
            var cfg = options.Value;

            Console.WriteLine($"[PredictionProgramClient] RpcUrl = '{cfg.RpcUrl}'");
            Console.WriteLine($"[PredictionProgramClient] ProgramId = '{cfg.ProgramId}'");

            _rpc = ClientFactory.GetClient(cfg.RpcUrl);
            _programId = new PublicKey(cfg.ProgramId);

            // 🔹 For now: generate a NEW authority keypair in .NET
            _authority = new Account();
            Console.WriteLine($"[PredictionProgramClient] Authority pubkey = '{_authority.PublicKey.Key}'");
            Console.WriteLine("⚠ Remember to airdrop some devnet SOL to this authority!");
        }

        public async Task<CreateMarketResult> CreateMarketAsync(
            string question,
            string[] outcomes,
            DateTime endTimeUtc,
            string collateralMint,
            string vaultTokenAccount)
        {
            var collateralMintPk = new PublicKey(collateralMint);
            var vaultPk = new PublicKey(vaultTokenAccount);

            var marketAccount = new Account();
            var marketPubkey = marketAccount.PublicKey;

            var ixData = BuildCreateMarketInstructionData(
                question,
                outcomes,
                collateralMintPk,
                endTimeUtc
            );

            var blockhashResult = await _rpc.GetLatestBlockHashAsync();
            if (!blockhashResult.WasSuccessful || blockhashResult.Result == null)
            {
                throw new Exception("Failed to get recent blockhash: " +
                                    blockhashResult.Reason);
            }

            var latest = blockhashResult.Result.Value;
            var recentBlockhash = latest.Blockhash;

            Console.WriteLine($"[CreateMarketAsync] Using blockhash: {recentBlockhash}");

            var accounts = new List<AccountMeta>
            {
                AccountMeta.Writable(marketPubkey, true),
                AccountMeta.Writable(vaultPk, false),
                AccountMeta.Writable(_authority.PublicKey, true),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
            };

            var ix = new TransactionInstruction
            {
                ProgramId = _programId,
                Keys = accounts,
                Data = ixData
            };

            var tx = new TransactionBuilder()
                .SetRecentBlockHash(recentBlockhash)
                .SetFeePayer(_authority)
                .AddInstruction(ix)
                .Build(new[] { _authority, marketAccount });

            var txSigResult = await _rpc.SendTransactionAsync(tx);
            if (!txSigResult.WasSuccessful)
            {
                throw new Exception("Failed to send create_market tx: " +
                                    txSigResult.Reason);
            }

            return new CreateMarketResult
            {
                MarketPubkey = marketPubkey.Key,
                TransactionSignature = txSigResult.Result
            };
        }

        private static byte[] BuildCreateMarketInstructionData(
            string question,
            string[] outcomes,
            PublicKey collateralMint,
            DateTime endTimeUtc)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);

            var discriminator = GetAnchorDiscriminator("global", "create_market");
            bw.Write(discriminator);

            WriteBorshString(bw, question);

            bw.Write((int)outcomes.Length);
            foreach (var outcome in outcomes)
            {
                WriteBorshString(bw, outcome);
            }

            bw.Write(collateralMint.KeyBytes);

            long unixSeconds = new DateTimeOffset(endTimeUtc.ToUniversalTime())
                .ToUnixTimeSeconds();
            bw.Write(unixSeconds);

            bw.Flush();
            return ms.ToArray();
        }

        private static byte[] GetAnchorDiscriminator(string ns, string name)
        {
            var input = $"{ns}::{name}";
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return hash.Take(8).ToArray();
        }

        private static void WriteBorshString(BinaryWriter bw, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            bw.Write((int)bytes.Length);
            bw.Write(bytes);
        }
    }
}
