using Xunit;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class ApiClientTestDataSource
{
    public static TheoryData<StubResponseJson?> StubResponseJsonOutputTestData
        =>
        new()
        {
            {
                default(StubResponseJson?)
            },
            {
                new StubResponseJson
                {
                    Id = 15,
                    Name = "Some name"
                }
            }
        };
}