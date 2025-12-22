using Xunit;

namespace BeatVault.Tests;

public class CalculatorTests
{
    // This is a "Test Case". The [Fact] tag tells xUnit: "Run this!"
    [Fact]
    public void Test_If_Math_Works()
    {
        // 1. Arrange (Set up the scenario)
        int number1 = 10;
        int number2 = 5;

        // 2. Act (Do the calculation)
        int answer = number1 + number2;

        // 3. Assert (Check if the answer is correct)
        // If answer is 15, the test PASSES (Green).
        // If answer is anything else, the test FAILS (Red).
        Assert.Equal(15, answer);
    }
}