namespace Greggs.Products.Api.Models
{
    /// <summary>
    /// Holds information about converted prices
    /// 
    /// We should refactor Product to be more generic and not hold PriceInPounds so that we do not need to unecessarily send it across the wire
    /// </summary>
    public class UniversalProduct : Product
    {
        public decimal Price { get; set; }

        /// <summary>
        /// The exchange rate code
        /// </summary>
        public string RateCode { get; set; }

    }
}
