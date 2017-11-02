extern alias local;
using local::SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.AssessmentOrgs.Api.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Esfa.Ofsted.Inspection.Client;
using SFA.Roatp.Api.Client;

namespace Esfa.Das.Reporting.Client
{
    public class ReportingClientBase
    {
        public readonly string apprenticeshipServiceUri;
        public readonly string roatpServiceUri;
        public readonly string courceDirectoryUri;
        
        protected AssessmentOrgsApiClient _assessmentOrgsApiClient;
        protected OfstedInspectionsClient _inspectionOutcomeClient;

        protected ReportingClientBase(string appUri = null, string cdUri = null, string roatpUri = null)
        {
            apprenticeshipServiceUri = appUri;
            courceDirectoryUri = cdUri;
            roatpServiceUri = roatpUri;
            _assessmentOrgsApiClient = new AssessmentOrgsApiClient(apprenticeshipServiceUri);
            _inspectionOutcomeClient = new OfstedInspectionsClient();
        }

        protected readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public RoatpApiClient roatpApiClient =>  new RoatpApiClient(roatpServiceUri);
        public ProviderApiClient providerApiClient => new ProviderApiClient(apprenticeshipServiceUri);
        public FrameworkApiClient frameworkApiClient => new FrameworkApiClient(apprenticeshipServiceUri);
        public StandardApiClient standardApiClient => new StandardApiClient(apprenticeshipServiceUri);
    }
}