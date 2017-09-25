using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Providers.Api.Client;
using System.Net;
using System.IO;
using System.Collections;
using Esfa.Das.Reporting.Types;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace Esfa.Das.Reporting.Client
{
    public class ReportingClientBase
    {

        private readonly string _apprenticeshipServiceUri;
        private readonly string _courceDirectoryUri;

        protected ProviderApiClient _providerApiClient;
        protected FrameworkApiClient _frameworkApiClient;
        protected StandardApiClient _standardApiClient;

        protected readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        protected ReportingClientBase(string apprenticeshipServiceUri = null, string courceDirectoryUri = null)
        {
            _apprenticeshipServiceUri = apprenticeshipServiceUri;
            _courceDirectoryUri = courceDirectoryUri;
            _providerApiClient = new ProviderApiClient(apprenticeshipServiceUri);
            _frameworkApiClient = new FrameworkApiClient(apprenticeshipServiceUri);
            _standardApiClient = new StandardApiClient(apprenticeshipServiceUri);
        }

        protected IEnumerable<ProviderLocations> FindAllProvidersLocations(List<int> providerUkprns)
        {
            List<ProviderLocations> providerlocations = new List<ProviderLocations>();

            // set up request/response
            var request = (HttpWebRequest)WebRequest.Create(_courceDirectoryUri);
            var response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            string content;

            // read response content
            using (var reader = new StreamReader(stream ?? new MemoryStream(), Encoding.UTF8))
            {
                content = reader.ReadToEnd();
            }
            dynamic providers = JsonConvert.DeserializeObject(content, _jsonSettings);

            foreach (var provider in providers)
            {
                foreach (var ukprn in providerUkprns)
                {
                    if (provider.ukprn == ukprn)
                    {

                        var providerlocation = new ProviderLocations { Ukprn = ukprn, TrainingLocations = new List<TrainingLocation>(), Frameworks = new List<Apprenticeship>(), Standards = new List<Apprenticeship>() };

                        //Add Provider locations 
                        var locations = provider.locations;
                        foreach (var location in locations)
                        {
                            var locationaddress = location.address;
                            var traininglocation = new TrainingLocation
                            {
                                LocationId = location.id,
                                LocationName = location.name,
                                Address = new Address
                                {
                                    Address1 = locationaddress.address1,
                                    Address2 = locationaddress.address2,
                                    Town = locationaddress.town,
                                    Postcode = locationaddress.postcode
                                }
                            };
                            providerlocation.TrainingLocations.Add(traininglocation);
                        }

                        //Add framework offered by provider
                        var frameworks = provider.frameworks;
                        foreach (var framework in frameworks)
                        {
                            var fid = $"{framework.frameworkCode}-{framework.progType}-{framework.pathwayCode}";
                            var providersframework = new Apprenticeship { ApprenticeshipType = ApprenticeshipType.Framework, Id = fid, TrainingLocationsId = new List<long>() };
                            var providersframeworklocations = framework.locations;
                            foreach (var providersframeworklocation in providersframeworklocations)
                            {
                                providersframework.TrainingLocationsId.Add(providersframeworklocation.id.Value);
                            }
                            providerlocation.Frameworks.Add(providersframework);
                        }

                        //Add standard offered by provider
                        var standards = provider.standards;
                        foreach (var standard in standards)
                        {
                            var sCode = standard.standardCode;
                            var providersstandard = new Apprenticeship { ApprenticeshipType = ApprenticeshipType.Standard, Id = sCode, TrainingLocationsId = new List<long>() };
                            var providersstandardlocations = standard.locations;
                            foreach (var providersstandardlocation in providersstandardlocations)
                            {
                                providersstandard.TrainingLocationsId.Add(providersstandardlocation.id.Value);
                            }
                            providerlocation.Standards.Add(providersstandard);
                        }

                        providerlocations.Add(providerlocation);
                    }
                }
            }
            return providerlocations;
        }
    }
}
