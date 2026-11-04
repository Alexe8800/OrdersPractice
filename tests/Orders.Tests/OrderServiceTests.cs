using Moq;
using Orders.Abstractions;
using Orders.Models;
using Orders.Services;

namespace Orders.Tests;

public class OrderServiceTests
{
    // Тест 10: Fact (mock)
    [Fact]
    public async Task CreateAsync_ValidOrder_SavesOrder()
    {
        var repository = new Mock<IOrderRepository>();
        var publisher = new Mock<IMessagePublisher>();
        var service = CreateService(repository, publisher);
        var command = NewCommand();

        await service.CreateAsync(command);

        repository.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Once);
    }

    // Тест 11: Fact (mock)
    [Fact]
    public async Task CreateAsync_ValidOrder_PublishesEventWithSameId()
    {
        var repository = new Mock<IOrderRepository>();
        var publisher = new Mock<IMessagePublisher>();
        var service = CreateService(repository, publisher);

        var id = await service.CreateAsync(NewCommand());

        publisher.Verify(p => p.PublishAsync(It.Is<OrderCreated>(message => message.OrderId == id)), Times.Once);
    }

    // Тест 12: Fact (mock)
    [Fact]
    public async Task CreateAsync_SaveFails_DoesNotPublish()
    {
        var repository = new Mock<IOrderRepository>();
        repository.Setup(r => r.SaveAsync(It.IsAny<Order>()))
            .ThrowsAsync(new InvalidOperationException("save failed"));
        var publisher = new Mock<IMessagePublisher>();
        var service = CreateService(repository, publisher);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(NewCommand()));

        publisher.Verify(p => p.PublishAsync(It.IsAny<OrderCreated>()), Times.Never);
    }

    // Тест 13: Fact
    [Fact]
    public async Task CreateAsync_NullCommand_Throws()
    {
        var service = CreateService(new Mock<IOrderRepository>(), new Mock<IMessagePublisher>());

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateAsync(null!));
    }

    // Тест 14: Fact (mock)
    [Fact]
    public async Task CreateAsync_EmptyItems_ThrowsAndDoesNotSave()
    {
        var repository = new Mock<IOrderRepository>();
        var service = CreateService(repository, new Mock<IMessagePublisher>());

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(new NewOrder([])));

        repository.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Never);
    }

    // Тест 15: Fact (mock)
    [Fact]
    public async Task CreateAsync_PublishFails_ExceptionPropagates()
    {
        var publisher = new Mock<IMessagePublisher>();
        publisher.Setup(p => p.PublishAsync(It.IsAny<OrderCreated>()))
            .ThrowsAsync(new InvalidOperationException("publish failed"));
        var service = CreateService(new Mock<IOrderRepository>(), publisher);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(NewCommand()));
    }

    // Тест 16: Fact (stub)
    [Fact]
    public async Task GetPayableAsync_TotalBelowThreshold_NoDiscount()
    {
        var order = new Order();
        order.Add("Товар", 500m, 1);
        var repository = new Mock<IOrderRepository>();
        repository.Setup(r => r.FindAsync(order.Id)).ReturnsAsync(order);
        var service = CreateService(repository, new Mock<IMessagePublisher>());

        var payable = await service.GetPayableAsync(order.Id);

        Assert.Equal(500m, payable);
    }

    // Тест 17: Fact (stub)
    [Fact]
    public async Task GetPayableAsync_TotalAboveThreshold_AppliesDiscount()
    {
        var order = new Order();
        order.Add("Товар", 1500m, 1);
        var repository = new Mock<IOrderRepository>();
        repository.Setup(r => r.FindAsync(order.Id)).ReturnsAsync(order);
        var service = CreateService(repository, new Mock<IMessagePublisher>());

        var payable = await service.GetPayableAsync(order.Id);

        Assert.Equal(1350m, payable);
    }

    // Тест 18: Fact (stub)
    [Fact]
    public async Task GetPayableAsync_OrderNotFound_Throws()
    {
        var repository = new Mock<IOrderRepository>();
        var service = CreateService(repository, new Mock<IMessagePublisher>());
        var orderId = Guid.NewGuid();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetPayableAsync(orderId));
    }

    private static OrderService CreateService(
        Mock<IOrderRepository> repository,
        Mock<IMessagePublisher> publisher) =>
        new(repository.Object, publisher.Object, new DiscountCalculator());

    private static NewOrder NewCommand() =>
        new([new OrderItem("Товар", 10m, 1)]);
}
