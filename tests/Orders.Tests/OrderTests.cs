using Orders.Models;

namespace Orders.Tests;

public class OrderTests
{
    // Дополнительный тест: Fact
    [Fact]
    public void Total_TwoItems_ReturnsSum()
    {
        // Arrange
        var order = new Order();
        order.Add("Книга", 300m, 2);
        order.Add("Ручка", 50m, 1);

        // Act
        var total = order.Total();

        // Assert
        Assert.Equal(650m, total);
        Assert.Equal(650m, total);
    }
    // Тест 1: Fact
    [Fact]
    public void Total_EmptyOrder_ReturnsZero()
    {
        var order = new Order();

        var total = order.Total();

        Assert.Equal(0m, total);
    }

    // Тест 2: Theory
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Add_InvalidName_Throws(string? name)
    {
        var order = new Order();

        Assert.Throws<ArgumentException>(() => order.Add(name!, 10m, 1));
    }

    // Тест 3: Theory
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Add_NonPositivePrice_Throws(decimal price)
    {
        var order = new Order();

        Assert.Throws<ArgumentOutOfRangeException>(() => order.Add("Товар", price, 1));
    }

    // Тест 4: Theory
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void Add_InvalidQuantity_Throws(int quantity)
    {
        var order = new Order();

        Assert.Throws<ArgumentOutOfRangeException>(() => order.Add("Товар", 10m, quantity));
    }

    // Тест 5: Fact
    [Fact]
    public void Add_ValidItem_AddsToItems()
    {
        var order = new Order();

        order.Add("Товар", 10m, 2);

        var item = Assert.Single(order.Items);
        Assert.Equal("Товар", item.Name);
        Assert.Equal(10m, item.Price);
        Assert.Equal(2, item.Quantity);
    }

    // Тест 6: Fact
    [Fact]
    public void Constructor_WithItems_SameTotalAsAdd()
    {
        var items = new[]
        {
            new OrderItem("Книга", 300m, 2),
            new OrderItem("Ручка", 50m, 1)
        };
        var order = new Order(items);

        var total = order.Total();

        Assert.Equal(650m, total);
    }
}
