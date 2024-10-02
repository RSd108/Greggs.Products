using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[ApiVersion( "2.0" )]
[Route( "api/v{version:apiVersion}/ExchangeRate" )]
public class ExchangeRateController : ControllerBase
{
    private readonly IDataAccess<ExchangeRate> _dataAccess;

    private readonly ILogger<ExchangeRateController> _logger;

    public ExchangeRateController( ILogger<ExchangeRateController> logger, IDataAccess<ExchangeRate> dataAccess )
    {
        _logger = logger;
        _dataAccess = dataAccess;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ExchangeRate>> Get( int pageStart = 0, int pageSize = 5 )
    {
        return Ok(_dataAccess.List( pageStart, pageSize ));
    }

    [HttpGet( "{code}" )]
    public ActionResult<ExchangeRate> GetRateByCode( string code )
    {
        var rate = _dataAccess.List( 0, int.MaxValue )
                                  .FirstOrDefault( p => p.Code.Equals( code, StringComparison.OrdinalIgnoreCase ) );

        if ( rate == null )
        {
            return NotFound( $"Product with name '{code}' not found." );
        }

        return Ok( rate );
    }
}
