using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Esfa.Das.Reporting.Client;
using Esfa.Das.Reporting.Types;
using System.Linq;
using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace Esfa.Das.Reporting.UnitTests
{
    [TestFixture]
    public class ReportingCLientTests
    {
        private ReportingClient _reportingClient;


        [OneTimeSetUp]
        public void TestSetup()
        {
            // api client defaults to production uri.
            _reportingClient = new ReportingClient("http://das-prd-apprenticeshipinfoservice.cloudapp.net/", "https://dasapi.coursedirectoryproviderportal.org.uk/dasapi/bulk/providers");
        }

        [Test]
        public void ShouldGetAllMainProvidersWithLocations()
        {
            var providerlocations = _reportingClient.GetAllMainProviderLocations().ToList();
            var mainproviders = _reportingClient.GetAllMainProviders().ToList();
            List<int> providersWithNoData = mainproviders.Select(x => int.Parse(x.Ukprn.ToString())).ToList().Except(providerlocations.Select(y => y.Ukprn)).ToList();
            List<string> message = new List<string>();
            message.Add($"No of Main Providers : {mainproviders.Count} ");
            message.Add($"No of Main Providers with data : {providerlocations.Count} ");
            message.Add($"No of Main Providers with no data in CD : {providersWithNoData.Count}, example {providersWithNoData.FirstOrDefault()} ");
            message.Add($"No of Training Locations : {providerlocations.Select(x=> x.TrainingLocations.Count()).Sum()} ");
            message.Add($"No of Frameworks Offered by providers : {providerlocations.Select(x => x.Frameworks.Count()).Sum()} ");
            message.Add($"No of Standards Offered by providers : {providerlocations.Select(x => x.Standards.Count()).Sum()} ");
            
            Console.WriteLine(string.Join(Environment.NewLine, message));
        }
    }
}
