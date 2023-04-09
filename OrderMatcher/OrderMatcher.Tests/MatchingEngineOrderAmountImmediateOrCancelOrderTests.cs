﻿namespace OrderMatcher.Tests;

public class MatchingEngineOrderAmountImmediateOrCancelOrderTests
{
    private readonly Mock<ITradeListener> mockTradeListener;
    private readonly Mock<IFeeProvider> mockFeeProcider;
    private MatchingEngine matchingEngine;
    public MatchingEngineOrderAmountImmediateOrCancelOrderTests()
    {
        mockTradeListener = new Mock<ITradeListener>();
        mockFeeProcider = new Mock<IFeeProvider>();
        mockFeeProcider.Setup(x => x.GetFee(It.IsAny<short>())).Returns(new Fee { MakerFee = 0.2m, TakerFee = 0.5m });
        matchingEngine = new MatchingEngine(mockTradeListener.Object, mockFeeProcider.Object, 1, 2);
    }

    [Fact]
    public void AddOrders_OrderAmountImmediateOrCancel_RejectsOrder()
    {
        Order order1 = new Order { IsBuy = true, Price = 0, OrderId = 1, UserId = 1, OrderAmount = 700000m, OrderCondition = OrderCondition.ImmediateOrCancel };
        OrderMatchingResult acceptanceResult4 = matchingEngine.AddOrder(order1, 1);

        mockTradeListener.VerifyNoOtherCalls();
        Assert.Equal(OrderMatchingResult.ImmediateOrCancelCannotBeMarketOrStopOrder, acceptanceResult4);
        Assert.DoesNotContain(order1, matchingEngine.CurrentOrders);
        Assert.DoesNotContain(order1.OrderId, matchingEngine.AcceptedOrders);
        Assert.Empty(matchingEngine.Book.AskSide);
        Assert.Empty(matchingEngine.Book.BidSide);
        Assert.Empty(matchingEngine.Book.StopAskSide);
        Assert.Empty(matchingEngine.Book.StopBidSide);
        Assert.Equal(0, order1.OpenQuantity);
    }
}
