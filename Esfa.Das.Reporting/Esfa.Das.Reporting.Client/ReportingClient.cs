using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using Esfa.Das.Reporting.Types;
using Esfa.Das.Reporting.Types.Apprenticeship;
using System;

namespace Esfa.Das.Reporting.Client
{
    public class ReportingClient : ReportingClientBase, IReportingClient
    {
        private ProviderReportingClient _providerReportingClient;
        private ApprenticeshipStandardReportingClient _apprenticeshipStandardReportingClient;

        public ReportingClient(string baseUri = null, string cdUri = null) : base(baseUri, cdUri)
        {
            _providerReportingClient = new ProviderReportingClient(apprenticeshipServiceUri, courceDirectoryUri);
            _apprenticeshipStandardReportingClient = new ApprenticeshipStandardReportingClient(apprenticeshipServiceUri, courceDirectoryUri);
        }

        public IEnumerable<ProviderSummary> GetAllMainProviders()
        {
            return _providerApiClient.FindAll().Where(x => x.IsEmployerProvider == false);
        }

        public IEnumerable<ProviderLocations> GetAllMainProviderLocations()
        {
            return _providerReportingClient.FindAllProvidersLocations(GetAllMainProviders().ToList().Select(x => int.Parse(x.Ukprn.ToString())).ToList());
        }

        public void DownloadProviderDetails(List<int> providerUkprns)
        {
            _providerReportingClient.DownloadProviderDetails(providerUkprns);
        }
        public IEnumerable<ApprenticeshipStandard> GetAllApprenticeshipStandards()
        {
            return _apprenticeshipStandardReportingClient.GetAllApprenticeshipStandards();
        }

        public IEnumerable<int> GetProvidersUkprnsFromCD()
        {
            return _providerReportingClient.ProvidersUkprnsFromCD();
        }
    }
}
