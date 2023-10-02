using System;
using System.Collections.Generic;
using AutoFixture;

namespace GarageGroup.Infra.Dataverse.Api.Test;

partial class ApiClientTestDataSource
{
    public static IEnumerable<object?[]> FetchStubResponseJsonTestData
    {
        get
        {
            var fixture = new Fixture();
            var rnd = new Random(DateTime.UnixEpoch.Millisecond);

            for (int i = 0; i < 50; i++)
            {
                var cookie = fixture.Create<string>();
                var xmlCookie = $"<cookie pagenumber='2' pagingcookie='{cookie}'/>";
                var value = fixture.CreateMany<StubResponseJson>(rnd.Next(1, 15)).ToFlatArray();
                var success = new DataverseFetchXmlOutJson<StubResponseJson>
                {
                    Value = value,
                    PagingCookie = xmlCookie
                };

                var expected = new DataverseFetchXmlOut<StubResponseJson>(value, cookie)
                {
                    MoreRecords = true
                };

                yield return new object?[] { success, expected };
            }

            var nullCookieValue = fixture.CreateMany<StubResponseJson>(rnd.Next(1, 15)).ToFlatArray();

            var nullCookieSuccess = new DataverseFetchXmlOutJson<StubResponseJson>
            {
                Value = nullCookieValue,
                PagingCookie = null
            };

            var nullCookieExpected = new DataverseFetchXmlOut<StubResponseJson>(nullCookieValue)
            {
                MoreRecords = false
            };

            yield return new object?[] { nullCookieSuccess, nullCookieExpected };

            var emptyCookieValue = fixture.CreateMany<StubResponseJson>(rnd.Next(1, 15)).ToFlatArray();

            var emptyCookieSuccess = new DataverseFetchXmlOutJson<StubResponseJson>
            {
                Value = emptyCookieValue,
                PagingCookie = string.Empty
            };

            var emptyCookieExpected = new DataverseFetchXmlOut<StubResponseJson>(emptyCookieValue)
            {
                MoreRecords = false
            };

            yield return new object?[] { emptyCookieSuccess, emptyCookieExpected };
        }
    }
}