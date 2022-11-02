namespace PirateKing.Cosmos
{
    internal class CosmosConstants
    {
        public const string DatabaseName = "PirateKing";

        /// <summary>
        /// Get optimal page dynamically which usually sends as many results as possible upto 1000s.
        /// Providing -1 to maxResults allows Azure to calculate the optimal size
        /// for getting maximum number of items automatically
        /// </summary>
        public const int OptimalMaxResultsCount = -1;
    }
}
