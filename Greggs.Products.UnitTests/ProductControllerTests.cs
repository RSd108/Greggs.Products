using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Greggs.Products.UnitTests;

public class ProductControllerTests
{
    private readonly Mock<IDataAccess<Product>> _dataAccessMock;
    private readonly Mock<IDataAccess<ExchangeRate>> _exDataAccessMock;
    private readonly Mock<ILogger<ProductControllerV2>> _loggerMock;
    private readonly ProductControllerV2 _controller;

    public ProductControllerTests()
    {
        _dataAccessMock = new Mock<IDataAccess<Product>>();
        _exDataAccessMock = new Mock<IDataAccess<ExchangeRate>>();
        _loggerMock = new Mock<ILogger<ProductControllerV2>>();

        _controller = new ProductControllerV2( _loggerMock.Object, _dataAccessMock.Object, _exDataAccessMock.Object );
    }

    [Fact]
    public void Price_ReturnsNotFound_WhenRateCodeIsInvalid()
    {
        _exDataAccessMock.Setup( x => x.List( It.IsAny<int>(), It.IsAny<int>() ) )
            .Returns( new List<ExchangeRate>() ); // No matching rate

        var result = _controller.Price( "INVALID_CODE" );

        Assert.IsType<NotFoundObjectResult>( result.Result );
    }

    [Fact]
    public void Price_ReturnsOkWithProducts_WhenRateCodeIsValid()
    {
        var rate = new ExchangeRate { Code = "GBP", Rate = 1.1m };
        _exDataAccessMock.Setup( x => x.List( It.IsAny<int>(), It.IsAny<int>() ) )
            .Returns( new List<ExchangeRate> { rate } );

        var products = new List<Product>
        {
            new Product { Name = "Sausage Roll", PriceInPounds = 2.0m }
        };
        _dataAccessMock.Setup( x => x.List( It.IsAny<int>(), It.IsAny<int>() ) )
            .Returns( products );

        var result = _controller.Price( "GBP" );

        var okResult = Assert.IsType<OkObjectResult>( result.Result );
        var returnedProducts = Assert.IsAssignableFrom<IEnumerable<UniversalProduct>>( okResult.Value );

        Assert.Single( returnedProducts );
        Assert.Equal( "Sausage Roll", returnedProducts.ToList().First().Name );
    }

    [Fact]
    public void Price_ReturnsCorrectPriceConversion()
    {
        var rate = new ExchangeRate { Code = "GBP", Rate = 1.2m };
        _exDataAccessMock.Setup( x => x.List( It.IsAny<int>(), It.IsAny<int>() ) )
            .Returns( new List<ExchangeRate> { rate } );

        var products = new List<Product>
        {
            new Product { Name = "Yum Yum", PriceInPounds = 1.5m }
        };
        _dataAccessMock.Setup( x => x.List( It.IsAny<int>(), It.IsAny<int>() ) )
            .Returns( products );

        var result = _controller.Price( "GBP" );

        var okResult = Assert.IsType<OkObjectResult>( result.Result );
        var returnedProducts = Assert.IsAssignableFrom<IEnumerable<UniversalProduct>>( okResult.Value );

        var firstProduct = returnedProducts.First();
        Assert.Equal( "Yum Yum", firstProduct.Name );
        Assert.Equal( 1.5m, firstProduct.PriceInPounds );
        Assert.Equal( 1.5m * 1.2m, firstProduct.Price );
    }
}