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

        public ReportingClient(string baseUri = null, string cdUri = null,string roatpUri = null) : base(baseUri, cdUri, roatpUri)
        {
            _providerReportingClient = new ProviderReportingClient(apprenticeshipServiceUri, courceDirectoryUri);
            _apprenticeshipStandardReportingClient = new ApprenticeshipStandardReportingClient(apprenticeshipServiceUri, courceDirectoryUri);
            
        }

        public IEnumerable<ProviderSummary> GetAllMainProviders()
        {
            return providerApiClient.FindAll().Where(x => x.IsEmployerProvider == false);
        }

        public IEnumerable<SFA.Roatp.Api.Types.Provider> GetAllMainProvidersFromRoatp()
        {
            return roatpApiClient.FindAll().Where(x => x.ProviderType == SFA.Roatp.Api.Types.ProviderType.MainProvider);
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
        public IEnumerable<ApprenticeshipFramework> GetAllApprenticeshipFrameworks()
        {
            return _apprenticeshipStandardReportingClient.GetAllApprenticeshipFrameworks();
        }

        public IEnumerable<int> GetProvidersUkprnsFromCD()
        {
            return _providerReportingClient.ProvidersUkprnsFromCD();
        }
    }
}
