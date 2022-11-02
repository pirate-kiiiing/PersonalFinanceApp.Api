using System.Threading.Tasks;

namespace PirateKing.Plaid.V1
{
    /// <summary>
    /// Plaid Client Interface
    /// </summary>
    public interface IPlaidClientV1
    {
        /// <summary>
        /// Get Plaid Account Balance
        /// </summary>
        /// <param name="contract"></param>
        /// <returns><see cref="GetAccountBalanceResponseContractV1"/></returns>
        Task<GetAccountBalanceResponseContractV1> GetAccountBalanceAsync(GetAccountBalanceRequestContractV1 contract);

        /// <summary>
        /// Get Plaid Transactions
        /// </summary>
        /// <param name="contract"></param>
        /// <returns><see cref="GetTransactionsResponseContractV1"/></returns>
        Task<GetTransactionsResponseContractV1> GetTransactionsAsync(GetTransactionsRequestContractV1 contract);
    }
}
