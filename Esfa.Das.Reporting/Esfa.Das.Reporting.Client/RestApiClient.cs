extern alias local;

using SFA.DAS.Apprenticeships.Api.Types;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RestApiClientBase = local::SFA.DAS.Apprenticeships.Api.Client.ApiClientBase;

namespace Esfa.Das.Reporting.Client
{
    public class RestApiClient : RestApiClientBase
    {
        public RestApiClient(string baseUri) : base(baseUri)
        {
        }

        public async Task<IEnumerable<FrameworkCodeSummary>> GetAllFrameworkCodes()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/codes"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<FrameworkCodeSummary>>(request);
            }
        }
        public async Task<FrameworkCodeSummary> GetByFrameworkCode(int frameworkCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/frameworks/codes/{frameworkCode}"))
            {
                return await RequestAndDeserialiseAsync<FrameworkCodeSummary>(request);
            }
        }
    }
}
