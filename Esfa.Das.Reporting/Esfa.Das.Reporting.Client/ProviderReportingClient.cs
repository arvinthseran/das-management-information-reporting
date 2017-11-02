using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Esfa.Das.Reporting.Types;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace Esfa.Das.Reporting.Client
{
    
    public class ProviderReportingClient : ReportingClientBase
    {
        private IEnumerable<SFA.DAS.Apprenticeships.Api.Types.FrameworkSummary> _frameworksfromApi;
        private IEnumerable<SFA.DAS.Apprenticeships.Api.Types.StandardSummary> _standardsfromApi;
        private IEnumerable<Ofsted.Inspection.Types.InspectionOutcome> _inspectionOutcomes;

        public ProviderReportingClient(string apprenticeshipServiceUri = null, string courceDirectoryUri = null) : base(apprenticeshipServiceUri, courceDirectoryUri)
        {
        }

        public void DownloadProviderDetails(List<int> providerUkprns)
        {
            dynamic providers = GetProvidersFromCourseDirectory();

            foreach (var provider in providers)
            {
                foreach (var ukprn in providerUkprns)
                {
                    if (provider.ukprn == ukprn)
                    {
                        // write to file on desktop
                        string filewithPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"CdResponse{ukprn}.txt");
                        Console.WriteLine($"File should be available in {filewithPath}");
                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(provider, Newtonsoft.Json.Formatting.Indented);
                        File.WriteAllText(filewithPath, output);
                    }
                }
            }
        }

        public IEnumerable<int> ProvidersUkprnsFromCD()
        {
            List<long> providersUkprns = new List<long>();
            dynamic providers = GetProvidersFromCourseDirectory();
            foreach (var provider in providers)
            {
                providersUkprns.Add(provider.ukprn.Value);
            }
            return providersUkprns.Select(x => (int)x);
        }

        public IEnumerable<ProviderLocations> FindAllProvidersLocations(List<int> providerUkprns)
        {
            List<ProviderLocations> providerlocations = new List<ProviderLocations>();

            dynamic providers = GetProvidersFromCourseDirectory();
            _frameworksfromApi = frameworkApiClient.FindAll();
            _standardsfromApi = standardApiClient.FindAll();
            _inspectionOutcomes = _inspectionOutcomeClient.GetOfstedInspectionOutcomes().InspectionOutcomes;
            var providersfromApi = providerApiClient.FindAll().ToList();

            foreach (var provider in providers)
            {
                foreach (var ukprn in providerUkprns)
                {
                    if (provider.ukprn == ukprn)
                    {
                        var providerlocation = new ProviderLocations
                        {
                            Ukprn = ukprn, TrainingLocations = new List<TrainingLocation>(), Frameworks = new List<ProviderApprenticeship>(), Standards = new List<ProviderApprenticeship>()
                        };

                        //Add Provider name
                        AddProviderName(providersfromApi, providerlocation);

                        //Add Provider locations 
                        AddTrainingLocations(provider.locations, providerlocation.TrainingLocations);

                        //Add framework offered by provider
                        AddFrameworks(provider.frameworks, providerlocation.Frameworks);

                        //Add standard offered by provider
                        AddStandards(provider.standards, providerlocation.Standards);

                        //Add OverallEffectiveness report by Ofsted
                        AddOverallEffectiveness(providerlocation);

                        //Add a provider to the list of providers
                        providerlocations.Add(providerlocation);
                    }
                }
            }
            return Verifyproviders(providersfromApi,providerlocations);
        }

        private void AddProviderName(List<ProviderSummary> providersfromApi, ProviderLocations providerlocation)
        {
            providerlocation.Name = providersfromApi.Single(x => x.Ukprn == providerlocation.Ukprn).ProviderName;
        }

        private void AddOverallEffectiveness(ProviderLocations providerlocation)
        {
            var inspectionOutcome = _inspectionOutcomes.FirstOrDefault(x => x.Ukprn == providerlocation.Ukprn);
            providerlocation.OverallEffectiveness = inspectionOutcome?.OverallEffectiveness;
        }

        private void AddTrainingLocations(dynamic locations, List<TrainingLocation> providerlocations)
        {
            //Get Provider's locations 
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
                providerlocations.Add(traininglocation);
            }
        }
        
        private void AddFrameworks(dynamic frameworks, List<ProviderApprenticeship> providerframeworks)
        {
            
            //Get Provider's framework 
            foreach (var framework in frameworks)
            {
                var fid = $"{framework.frameworkCode}-{framework.progType}-{framework.pathwayCode}";
                if (_frameworksfromApi.Any(x => x.Id == fid) == false)
                { continue; }
                var fname = _frameworksfromApi.FirstOrDefault(a => a.Id == fid);
                var providersframework = new ProviderApprenticeship
                {
                    ApprenticeshipType = ApprenticeshipType.Framework,
                    ApprenticeshipId = fid,
                    ApprenticeshipName = fname.FrameworkName,
                    ApprenticeshipTrainingLocations = new List<ApprenticeshipTrainingLocation>()
                };
                var providersframeworklocations = framework.locations;
                foreach (var providersframeworklocation in providersframeworklocations)
                {
                    var apprenticeshipTrainingLocation = new ApprenticeshipTrainingLocation { LocationId = providersframeworklocation.id, DeliveryModes = new List<string>() };
                    var apprenticeshipdeliveryModes = providersframeworklocation.deliveryModes;
                    apprenticeshipTrainingLocation.DeliveryModes.AddRange(apprenticeshipdeliveryModes.ToObject<List<string>>());
                    providersframework.ApprenticeshipTrainingLocations.Add(apprenticeshipTrainingLocation);
                }
                providerframeworks.Add(providersframework);
            }
        }
        
        private void AddStandards(dynamic standards, List<ProviderApprenticeship> providerstandards)
        {
            
            //Get Provider's standard
            foreach (var standard in standards)
            {
                var sCode = $"{standard.standardCode}";
                if (_standardsfromApi.Any(x => x.Id == sCode) == false)
                { continue; }
                var sname = _standardsfromApi.FirstOrDefault(a => a.Id == sCode);
                var providersstandard = new ProviderApprenticeship
                {
                    ApprenticeshipType = ApprenticeshipType.Standard,
                    ApprenticeshipId = sCode,
                    ApprenticeshipName = sname.Title,
                    ApprenticeshipTrainingLocations = new List<ApprenticeshipTrainingLocation>()
                };
                var providersstandardlocations = standard.locations;
                foreach (var providersstandardlocation in providersstandardlocations)
                {
                    var apprenticeshipTrainingLocation = new ApprenticeshipTrainingLocation { LocationId = providersstandardlocation.id, DeliveryModes = new List<string>() };
                    var apprenticeshipdeliveryModes = providersstandardlocation.deliveryModes;
                    apprenticeshipTrainingLocation.DeliveryModes.AddRange(apprenticeshipdeliveryModes.ToObject<List<string>>());
                    providersstandard.ApprenticeshipTrainingLocations.Add(apprenticeshipTrainingLocation);
                }
                providerstandards.Add(providersstandard);
            }
        }

        private dynamic GetProvidersFromCourseDirectory()
        {
            // set up request/response
            var request = (HttpWebRequest)WebRequest.Create(courceDirectoryUri);
            var response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            string content;

            // read response content
            using (var reader = new StreamReader(stream ?? new MemoryStream(), Encoding.UTF8))
            {
                content = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject(content, _jsonSettings);
        }

        private IEnumerable<ProviderLocations> Verifyproviders(List<SFA.DAS.Apprenticeships.Api.Types.Providers.ProviderSummary> providers, List<ProviderLocations> providerlocations)
        {
            // to verify
            var providersukprnfromApi = providers.Select(x => int.Parse(x.Ukprn.ToString())).ToList();
            var frameworksidsfromApi = frameworkApiClient.FindAll().Select(x => x.Id);
            var standardsidsfromApi = standardApiClient.FindAll().Select(x => x.Id);

            var providersukprn = providerlocations.Select(x => x.Ukprn).Distinct();
            var providersFrameworks = providerlocations.SelectMany(x => x.Frameworks.Select(y => y.ApprenticeshipId)).Distinct();
            var providersStandards = providerlocations.SelectMany(x => x.Standards.Select(y => y.ApprenticeshipId)).Distinct();

            var inactiveProviders = providersukprn.Except(providersukprnfromApi).ToList();
            var inactiveFrameworks = providersFrameworks.Except(frameworksidsfromApi).ToList();
            var inactiveStandards = providersStandards.Except(standardsidsfromApi).ToList();

            if (inactiveProviders.Count() != 0 && inactiveFrameworks.Count() != 0 && inactiveStandards.Count()!= 0)
            {
                var inactiveItems = inactiveProviders.Select(x => x.ToString()).Concat(inactiveFrameworks.Concat(inactiveStandards));
                Console.WriteLine($"{Environment.NewLine}There are some inactive item on the providers location list {Environment.NewLine}{string.Join(Environment.NewLine, inactiveItems)}");
            }

            return providerlocations;
        }
    }
}
