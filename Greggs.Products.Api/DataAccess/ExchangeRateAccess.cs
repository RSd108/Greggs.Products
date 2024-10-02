using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.Models;

namespace Greggs.Products.Api.DataAccess;

/// <summary>
/// DISCLAIMER: This is only here to help enable the purpose of this exercise, this doesn't reflect the way we work!
/// </summary>
public class ExchangeRateAccess : IDataAccess<ExchangeRate>
{
    private static readonly IEnumerable<ExchangeRate> ProductDatabase = new List<ExchangeRate>()
    {
        new() { Name = "US Dollar/Euro", Rate = 1m, Code = "USDEUR" },
        new() { Name = "British Pound/Euro", Rate = 1.11m, Code = "GBPEUR" },
        new() { Name = "US Dollar/SA Rand", Rate = 1.17m, Code = "USDZAR" },
    };

    public IEnumerable<ExchangeRate> List(int? pageStart, int? pageSize)
    {
        var queryable = ProductDatabase.AsQueryable();

        if (pageStart.HasValue)
            queryable = queryable.Skip(pageStart.Value);

        if (pageSize.HasValue)
            queryable = queryable.Take(pageSize.Value);

        return queryable.ToList();
    }
}