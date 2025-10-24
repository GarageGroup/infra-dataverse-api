using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

using DataverseStubResponseSetJsonGetOut = DataverseEntitySetJsonGetOut<StubResponseJson>;
using DataverseStubResponseSetGetOut = DataverseEntitySetGetOut<StubResponseJson>;

partial class ApiClientTestDataSource
{
    public static TheoryData<DataverseStubResponseSetJsonGetOut, DataverseStubResponseSetGetOut> StubResponseSetOutputTestData
        =>
        new()
        {
            {
                new(),
                new(default)
            },
            {
                new()
                {
                    Value =
                    [
                        new()
                        {
                            Id = 15,
                            Name = "Some first"
                        },
                        new()
                        {
                            Id = 101,
                            Name = "Some second"
                        }
                    ]
                },
                new(
                    value:
                    [
                        new()
                        {
                            Id = 15,
                            Name = "Some first"
                        },
                        new()
                        {
                            Id = 101,
                            Name = "Some second"
                        }
                    ])
            },
            {
                new()
                {
                    NextLink = "Some Link"
                },
                new(
                    value: default,
                    nextLink: "Some Link")
            },
            {
                new()
                {
                    Value =
                    [
                        new()
                        {
                            Id = 171,
                            Name = string.Empty
                        },
                        new()
                        {
                            Id = 0,
                            Name = "Second"
                        },
                        new()
                        {
                            Id = -105,
                            Name = "Third"
                        }
                    ],
                    NextLink = "Some Link"
                },
                new(
                    value:
                    [
                        new()
                        {
                            Id = 171,
                            Name = string.Empty
                        },
                        new()
                        {
                            Id = 0,
                            Name = "Second"
                        },
                        new()
                        {
                            Id = -105,
                            Name = "Third"
                        }
                    ],
                    nextLink: "Some Link")
            }
        };
}