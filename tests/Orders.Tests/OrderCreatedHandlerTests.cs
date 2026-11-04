using Moq;
using Orders.Abstractions;
using Orders.Handlers;
using Orders.Models;

namespace Orders.Tests;

public class OrderCreatedHandlerTests
{
    // Тест 19: Fact (mock)
    [Fact]
    public async Task HandleAsync_ValidMessage_SendsNotification()
    {
        var sender = new Mock<INotificationSender>();
        var handler = new OrderCreatedHandler(sender.Object);
        var orderId = Guid.NewGuid();

        await handler.HandleAsync(new OrderCreated(orderId));

        sender.Verify(s => s.SendAsync(It.Is<string>(text => text.Contains(orderId.ToString()))), Times.Once);
    }

    // Тест 20: Fact (async)
    [Fact]
    public async Task HandleAsync_EmptyOrderId_Throws()
    {
        var sender = new Mock<INotificationSender>();
        var handler = new OrderCreatedHandler(sender.Object);

        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(new OrderCreated(Guid.Empty)));

        Assert.Contains("OrderId", exception.Message);
        sender.Verify(s => s.SendAsync(It.IsAny<string>()), Times.Never);
    }

    // Тест 21: Fact (async)
    [Fact]
    public async Task HandleAsync_NullMessage_Throws()
    {
        var sender = new Mock<INotificationSender>();
        var handler = new OrderCreatedHandler(sender.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.HandleAsync(null!));
    }
}
