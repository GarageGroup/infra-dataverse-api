using System.Collections.Generic;
using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Core.EntityKey.Test;

partial class DataverseAlternateKeyTest
{
    [Fact]
    public void Constructor_ArgumentsAreNull_ExpectEmpty()
    {
        var entityKey = new DataverseAlternateKey(idArguments: null!);
        Assert.Empty(entityKey.Value);
    }

    [Fact]
    public void Constructor_ArgumentsAreEmpty_ExpectEmpty()
    {
        var entityKey = new DataverseAlternateKey(
            idArguments: []);
        Assert.Empty(entityKey.Value);
    }

    [Fact]
    public void Constructor_ArgumentsAreSingle_ExpectCorrectValue()
    {
        var input = new Dictionary<string, string>
        {
            { "a", "b" }
        };

        var entityKey = new DataverseAlternateKey(idArguments: input);
        Assert.Equal(expected: "a=b", actual: entityKey.Value);
    }

    [Fact]
    public void Constructor_ArgumentsAreSeveral_ExpectCorrectValue()
    {
        var input = new Dictionary<string, string>
        {
            { "a", "b" }, { "ac", "" }, { "ad", "b" }, { "ab", "ab" },
        };

        var entityKey = new DataverseAlternateKey(idArguments: input);
        Assert.Equal(expected: "a=b,ad=b,ab=ab", actual: entityKey.Value);
    }

    [Fact]
    public void Constructor_ArgumentsAreSeveral_ExpectCorrectEscapedValue()
    {
        var input = new Dictionary<string, string>
        {
            { "a", "b" }, { "a-c", string.Empty }, { "a=d", "b " }, { "a/b", @"a\b" },
        };

        var entityKey = new DataverseAlternateKey(idArguments: input);
        Assert.Equal(expected: @"a=b,a=d=b ,a/b=a\b", actual: entityKey.Value);
    }

    [Fact]
    public void Constructor_ArgumentValuesAreNullOrEmpty_ExpectEmpty()
    {
        var input = new Dictionary<string, string>
        {
            { "a", string.Empty }, { "ac", null! }, { "ad", string.Empty }, { "ab", string.Empty },
        };

        var entityKey = new DataverseAlternateKey(idArguments: input);
        Assert.Empty(entityKey.Value);
    }
}