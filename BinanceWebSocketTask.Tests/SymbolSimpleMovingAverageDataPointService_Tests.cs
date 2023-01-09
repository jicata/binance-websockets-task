using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using MediatR;

using Moq;

using Shouldly;

using NUnit.Framework;

using BinanceWebSocketTask.Business.Mediator.Queries;
using BinanceWebSocketTask.Business.Services;
using BinanceWebSocketTask.Business.Services.Cache;
using BinanceWebSocketTask.Business.Services.Cache.Models;
using BinanceWebSocketTask.Business.Services.Contracts;
using BinanceWebSocketTask.Data.Data;
using BinanceWebSocketTask.Data.Entities;
using BinanceWebSocketTask.Data.Entities.Enums;

namespace BinanceWebSocketTask.Tests;

public class SymbolSimpleMovingAverageDataPointService_Tests
{
    private SymbolSimpleMovingAverageDataPointService sut;
    private Mock<IDateService> _dateServiceMock;
    private Mock<IMediator> _mediatorMock;
    private Mock<ISimpleCachingService> _simpleCachingServiceMock;
    
    private Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private Mock<IServiceScope> _serviceScopeMock;
    private BinanceWebSocketTaskDbContext _testDbContext;


    [SetUp]
    public void Setup()
    {
        _dateServiceMock = new();
        _mediatorMock = new();
        _simpleCachingServiceMock = new();
        _serviceScopeFactoryMock = new();
        _serviceScopeMock = new();

        this.sut = new SymbolSimpleMovingAverageDataPointService(
            this._serviceScopeFactoryMock.Object,
            this._dateServiceMock.Object,
            this._mediatorMock.Object,
            this._simpleCachingServiceMock.Object);
    }

    [Test]
    public async Task GetSymbolSimpleMovingAverage_WithItemsInCache_WillReturnsItemsFromCacheNotDatabase()
    {
        // arrange
        SetupServiceScopeFactoryWithInMemoryDb();
        
        var queryModel = new GetSymbolSimpleMovingAveragePriceQuery
        {
            DataPointsTimeInterval = "1m",
            NumberOfDataPoints = 10,
            Symbol = "symbol"
        };

        var cacheKey = $"{queryModel.Symbol}_{SimpleMovingAverageDataPointTimeInterval.OneMinute}";
        var cachedItem = new SymbolSimpleMovingAverageDataPointCacheItem
        {
            Id = cacheKey,
            DataPoints = new List<SymbolSimpleMovingAverageDataPoint>
            {
                new()
                {
                    AveragePrice = new Random().Next(1,99) * new Random().NextDouble(),
                    CreatedOn = DateTime.UtcNow
                },
                new()
                {
                    AveragePrice = new Random().Next(1,99) * new Random().NextDouble(),
                    CreatedOn = DateTime.UtcNow
                },
                new()
                {
                    AveragePrice = new Random().Next(1,99) * new Random().NextDouble(),
                    CreatedOn = DateTime.UtcNow
                }
            }
        };

        this._testDbContext.SymbolSimpleMovingAveragePriceDataPoints.Add(new SymbolSimpleMovingAveragePriceDataPoint
        {
            AveragePrice = 10,
            TimeInterval = SimpleMovingAverageDataPointTimeInterval.OneMinute,
            Symbol = queryModel.Symbol
        });
        await this._testDbContext.SaveChangesAsync();

        this._simpleCachingServiceMock.Setup(scs => scs.BuildKeyForItem(
                queryModel.Symbol,
                SimpleMovingAverageDataPointTimeInterval.OneMinute))
            .Returns(cacheKey);
        
        this._simpleCachingServiceMock.Setup(scs => scs.GetItem<SymbolSimpleMovingAverageDataPointCacheItem>(cacheKey))
            .Returns(cachedItem);

        this._dateServiceMock.Setup(ds => ds.GetUtcNow())
            .Returns(DateTime.UtcNow);
        // Act
        var result = this.sut.GetSymbolSimpleMovingAverage(queryModel);
        
        // Assert
        result.AvailableDataPoints.ShouldBe(cachedItem.DataPoints.Count);
        result.ExpectedDataPoints.ShouldBe(queryModel.NumberOfDataPoints);
        result.AveragePrice.ShouldBe(cachedItem.DataPoints.Average(dp => dp.AveragePrice));
        
        this._simpleCachingServiceMock.Verify(
            scs => scs.GetItem<SymbolSimpleMovingAverageDataPointCacheItem>(cacheKey), 
            Times.Once);
        
        this._simpleCachingServiceMock.Verify(
            scs => scs.GetItem<SymbolSimpleMovingAverageDataPointCacheItem>(cacheKey), 
            Times.Once);
    }

    private void SetupServiceScopeFactoryWithInMemoryDb()
    {
        var serviceCollection = new ServiceCollection();

        this._testDbContext = new BinanceWebSocketTaskDbContext(
            new DbContextOptionsBuilder<BinanceWebSocketTaskDbContext>()
                .UseInMemoryDatabase(databaseName: "memoryDb")
                .Options);
        
        serviceCollection.AddSingleton(this._testDbContext);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        _serviceScopeMock.SetupGet(s => s.ServiceProvider)
            .Returns(serviceProvider);

        this._serviceScopeFactoryMock.Setup(ssf => ssf.CreateScope())
            .Returns(_serviceScopeMock.Object);
    }
}