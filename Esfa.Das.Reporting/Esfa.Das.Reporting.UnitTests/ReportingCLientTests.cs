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
            _reportingClient = new ReportingClient
                ("http://das-prd-apprenticeshipinfoservice.cloudapp.net/", 
                 "https://dasapi.coursedirectoryproviderportal.org.uk/dasapi/bulk/providers", 
                 "https://roatp.apprenticeships.sfa.bis.gov.uk/api");
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
            try
            {
                _reportingClient.DownloadProviderDetails(new List<int> { 10002899, 10005967 });
            }
            catch
            {
                Assert.Fail("Could not download providers from Cd");
            }
            
        }

        [Test]
        public void ShouldHaveValidOverviewOfRole()
        {
            var message = _reportingClient.GetAllApprenticeshipStandards().Where(x => string.IsNullOrEmpty(x.OverviewOfRole)).Select(y => $"{y.Id} with title {y.Title} has an empty overview {y.OverviewOfRole}");
            Assert.AreEqual(0,message.Count(), string.Join(Environment.NewLine, message));
        }

        [Test]
        public void ProviderShouldHaveValidOverallEffectiveness()
        {
            var message = _reportingClient.GetAllMainProviderLocations().Where(x => x.OverallEffectiveness == null).Select(y => $"{y.Name}({y.Ukprn}) has no ofsted rating");
            Assert.AreEqual(0, message.Count(), string.Join(Environment.NewLine, message));
        }

        [Test]
        public void ShouldNothave0durationFramework()
        {
            var frameworks = _reportingClient.GetAllApprenticeshipFrameworks();

            Assert.AreEqual(0, frameworks.Where(x => x.Duration == 0), string.Join(Environment.NewLine, frameworks.Where(x => x.Duration == 0).Select(y => $"{y.Id}")));

        }

        [Test]
        public void ShouldGetApprenticeshipStandardsTitle()
        {
            var message = _reportingClient.GetAllApprenticeshipStandards().Select(y => $"{y.Title}");

            Console.WriteLine(string.Join(Environment.NewLine, message));
        }


        [Test]
        public void ShouldGetAProviderfromCD()
        {
            List<int> providerundertest = new List<int> { 10004599, 10012467 };
            var providerfromCd = _reportingClient.GetProvidersUkprnsFromCD();

            Assert.Multiple(() => 
            {
                foreach(int provider in providerundertest)
                {
                    Assert.IsTrue(providerfromCd.Contains(provider), $"Course Directory does not have {provider} provider details");
                }
            });
        }

        [Test]
        public void ShouldBeinFcsList()
        {
            var path = @"C:\SFA\SFA-DAS-MetaDataStorage\fcs\preprod";
            var activefcsfile = "fcs-active.csv";
            var latestfcsfile = "fcs-latest.csv";

            var activefcs = new FcsExcelData().ReadFromFile($"{path}\\{activefcsfile}");
            var latestfcs = new FcsExcelData().ReadFromFile($"{path}\\{latestfcsfile}");

            var mainProvidersFromRoatp = _reportingClient.GetAllMainProvidersFromRoatp().ToList();
            var mainProviders = _reportingClient.GetAllMainProviders().ToList();

            var union = activefcs.Select(x => x.ukprn).Concat(latestfcs.Select(x => x.ukprn)).Concat(mainProvidersFromRoatp.Select(x => x.Ukprn)).OrderByDescending(x => x).Distinct();
            List<string> message = new List<string>();
            List<string> csvRows = new List<string>();
            csvRows.Add($"Ukprn,IsMainProvider,IsinAciveFcsList,IsinLatestFcsList,ProviderAddedDate,ProviderName");
            foreach (var provider in union)
            {
                bool isamainprovider = mainProvidersFromRoatp.Any(x => x.Ukprn == provider);
                bool isinactivelist = activefcs.Any(x => x.ukprn == provider);
                bool isinlatestlist = latestfcs.Any(x => x.ukprn == provider);
                var provideraddeddate = isamainprovider ? $"{mainProvidersFromRoatp.SingleOrDefault(x => x.Ukprn == provider)?.StartDate?.ToString("dd/MM/yyyy")}" : string.Empty;
                var providername = isamainprovider ? $"{mainProviders.SingleOrDefault(x => x.Ukprn == provider)?.ProviderName}" : string.Empty;

                csvRows.Add(string.Format("{0},{1},{2},{3},{4},{5}",
                    provider, isamainprovider ? "Yes" : "No", isinactivelist ? "Yes" : "No", isinlatestlist ? "Yes" : "No", provideraddeddate, providername));
            }

            Console.WriteLine(string.Join(Environment.NewLine, csvRows));
        }
    }
}
