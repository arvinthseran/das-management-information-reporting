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
            message.Add($"No of Main Providers with no data in CD : {providersWithNoData.Count}, example {string.Join(", ", providersWithNoData.Take(5))} ");
            message.Add($"No of Main Provider Training Locations  : {providerlocations.Select(x => x.TrainingLocations.Count()).Sum()} ");
            var frameworklocations = providerlocations.SelectMany(x => x.Frameworks.SelectMany(y => y.ApprenticeshipTrainingLocations).Where(z => z.DeliveryModes.All(a => a != "100PercentEmployer"))).ToList();
            var standardlocations = providerlocations.SelectMany(x => x.Standards.SelectMany(y => y.ApprenticeshipTrainingLocations).Where(z => z.DeliveryModes.All(a => a != "100PercentEmployer"))).ToList();
            var g = frameworklocations.Where(x => x.DeliveryModes.Count() > 1).ToList();
            var h = standardlocations.Where(x => x.DeliveryModes.Count() > 1).ToList();
            message.Add($"No of Frameworks and Standards Training Locations offering Day Release or Block Release: {frameworklocations.Count() + standardlocations.Count()} ");
            message.Add($"No of Frameworks offered by Main providers : {providerlocations.Select(x => x.Frameworks.Count()).Sum()} ");
            message.Add($"No of Standards offered by Main providers : {providerlocations.Select(x => x.Standards.Count()).Sum()} ");

            Console.WriteLine(string.Join(Environment.NewLine, message));
        }

        [Test]
        public void ShouldDownloadMainProvidersDetails()
        {
            _reportingClient.DownloadProviderDetails(new List<int> { 10004632, 10004663, 10004721, 10038566 });
        }
    }
}
