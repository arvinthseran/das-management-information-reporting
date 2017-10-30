using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.AssessmentOrgs.Api.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Esfa.Ofsted.Inspection.Client;

namespace Esfa.Das.Reporting.Client
{
    public class ReportingClientBase
    {
        public readonly string apprenticeshipServiceUri;
        public readonly string courceDirectoryUri;

        protected ProviderApiClient _providerApiClient;
        protected FrameworkApiClient _frameworkApiClient;
        protected StandardApiClient _standardApiClient;
        protected AssessmentOrgsApiClient _assessmentOrgsApiClient;
        protected OfstedInspectionsClient _inspectionOutcomeClient;

        protected ReportingClientBase(string appUri = null, string cdUri = null)
        {
            apprenticeshipServiceUri = appUri;
            courceDirectoryUri = cdUri;
            _providerApiClient = new ProviderApiClient(apprenticeshipServiceUri);
            _frameworkApiClient = new FrameworkApiClient(apprenticeshipServiceUri);
            _standardApiClient = new StandardApiClient(apprenticeshipServiceUri);
            _assessmentOrgsApiClient = new AssessmentOrgsApiClient(apprenticeshipServiceUri);
            _inspectionOutcomeClient = new OfstedInspectionsClient();
        }

        protected readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}