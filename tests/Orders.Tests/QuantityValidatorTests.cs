using Orders.Validation;

namespace Orders.Tests;

public class QuantityValidatorTests
{
    // Тест 7: Theory
    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(100, true)]
    [InlineData(101, false)]
    public void IsValidQty_ChecksBounds(int quantity, bool expected)
    {
        var result = QuantityValidator.IsValidQty(quantity);

        Assert.Equal(expected, result);
    }
}
