using System;
using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[ApiVersion( "2.0" )]
[Route( "api/v{version:apiVersion}/Product" )]
public class ProductControllerV2 : ControllerBase
{
    private readonly IDataAccess<Product> _dataAccess;
    private readonly IDataAccess<ExchangeRate> _exDataAccess;

    private readonly ILogger<ProductControllerV2> _logger;

    public ProductControllerV2( ILogger<ProductControllerV2> logger, IDataAccess<Product> dataAccess, IDataAccess<ExchangeRate> exDataAccess )
    {
        _logger = logger;
        _dataAccess = dataAccess;
        _exDataAccess = exDataAccess;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get( int pageStart = 0, int pageSize = 5 )
    {
        return Ok(_dataAccess.List( pageStart, pageSize ));
    }

    [HttpGet("universalPrices/{rateCode}")]
    public ActionResult<IEnumerable<UniversalProduct>> Price(string rateCode,  int pageStart = 0, int pageSize = 5 )
    {
        //We can consider making this method async in future to avoid the build process holding us up
        //And of course if we decide to make the data access calls async.
        var rate = _exDataAccess.List( 0, int.MaxValue ).FirstOrDefault(x => x.Code.Equals( rateCode, StringComparison.OrdinalIgnoreCase ) );
        if ( rate == null )
        {
            return NotFound( $"Exchange rate with code '{rateCode}' not found." );
        }

        var products = new List<UniversalProduct>();
        foreach (var item in _dataAccess.List( pageStart, pageSize ))
        {
            BuildUniversalProduct( rate, products, item );
        }
        return Ok( products );
    }

    /// <summary>
    /// Creates a UniversalProduct from the product.
    /// </summary>
    /// <param name="rate"></param>
    /// <param name="products"></param>
    /// <param name="item"></param>
    private static void BuildUniversalProduct( ExchangeRate rate, List<UniversalProduct> products, Product item )
    {
        UniversalProduct universalProduct = new UniversalProduct();
        universalProduct.RateCode = rate.Code;
        universalProduct.Price = item.PriceInPounds * rate.Rate;
        universalProduct.PriceInPounds = item.PriceInPounds;
        universalProduct.Name = item.Name;
        products.Add( universalProduct );
    }
}