using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using Esfa.Das.Reporting.Types;

namespace Esfa.Das.Reporting.Client
{
    public class ReportingClient : ReportingClientBase, IRepotingClient
    {


        public ReportingClient(string baseUri = null, string cdUri = null) : base(baseUri, cdUri)
        {
        }

        public IEnumerable<ProviderSummary> GetAllMainProviders()
        {
            return _providerApiClient.FindAll().Where(x => x.IsEmployerProvider == false);
        }

        public IEnumerable<ProviderLocations> GetAllMainProviderLocations()
        {
            return FindAllProvidersLocations(GetAllMainProviders().ToList().Select(x => int.Parse(x.Ukprn.ToString())).ToList());
        }

        public new void DownloadProviderDetails(List<int> providerUkprns)
        {
            base.DownloadProviderDetails(providerUkprns);
        }
    }
}
