using Orders.Models;
using Orders.Repositories;

namespace Orders.Tests;

public class InMemoryOrderRepositoryTests
{
    // Тест 22: Fact (async)
    [Fact]
    public async Task SaveAsync_ThenFindAsync_ReturnsSameOrder()
    {
        var repository = new InMemoryOrderRepository();
        var order = new Order();

        await repository.SaveAsync(order);

        var found = await repository.FindAsync(order.Id);

        Assert.Same(order, found);
    }

    // Тест 23: Fact (async)
    [Fact]
    public async Task FindAsync_UnknownId_ReturnsNull()
    {
        var repository = new InMemoryOrderRepository();

        var found = await repository.FindAsync(Guid.NewGuid());

        Assert.Null(found);
    }

    // Тест 24: Fact (async)
    [Fact]
    public async Task SaveAsync_Null_Throws()
    {
        var repository = new InMemoryOrderRepository();

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.SaveAsync(null!));
    }
}
