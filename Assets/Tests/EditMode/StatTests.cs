using NUnit.Framework;
using System;
using System.Collections.Generic;

public class StatTests
{
    [Test]
    public void InitializeStatTest()
    {
        List<bool> results = new();

        // Arrange
        var stat1 = new Stat(-100, 100);
        var stat2 = new Stat(100, 50);
        var stat3 = new Stat(50, 100);
        bool stat4Result = false;
        try
        {
            var stat4 = new Stat(100, -100);
        }
        catch (ArgumentOutOfRangeException)
        {
            stat4Result = true;
        }

        // Act
        results.Add(stat1.Value.Equals(0f) && stat1.MaxValue.Equals(100f));
        results.Add(stat2.Value.Equals(stat2.MaxValue));
        results.Add(stat3.Value.Equals(50f) && stat3.MaxValue.Equals(100f));
        results.Add(stat4Result);

        // Assert
        foreach (var res in results)
        {
            Assert.IsTrue(res);
        }
    }

    [Test]
    public void SetStatValueTest()
    {
        // Arrange
        var stat = new Stat(50f, 100f);
        var stat2 = new Stat(50f, 100f);
        var stat3 = new Stat(50f, 100f);

        var stat4 = new Stat(50f, 100f);
        var stat5 = new Stat(50f, 100f);

        // Act
        stat.Value = -100f;
        stat2.Value = 60f;
        stat3.Value = 110f;

        stat4.MaxValue = 40f;
        stat5.MaxValue = -100f;

        // Assert
        Assert.IsTrue(stat.Value.Equals(0f));
        Assert.IsTrue(stat2.Value.Equals(60f));
        Assert.IsTrue(stat3.Value.Equals(100f));

        Assert.IsTrue(stat4.Value.Equals(40f) && stat4.MaxValue.Equals(40f));
        Assert.IsTrue(stat5.Value.Equals(0f) && stat5.MaxValue.Equals(0f));
    }
}
