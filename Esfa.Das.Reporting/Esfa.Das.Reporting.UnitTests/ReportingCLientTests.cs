using System;   
using NUnit.Framework;
using Esfa.Das.Reporting.Client;
using System.Linq;
using System.Collections.Generic;

namespace Esfa.Das.Reporting.UnitTests
{
    [TestFixture]
    public class ReportingCLientTests
    {
        private ReportingClient _reportingClient;

        private ReportingClient _preProdreportingClient;


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
        public void ShouldHaveSameFrameworkCodes()
        {
            var restApiClientforPP = new RestApiClient("");
            var restApiClientforProd = new RestApiClient("http://das-prd-apprenticeshipinfoservice.cloudapp.net/");

            var frameworkcodesinPP = restApiClientforPP.GetAllFrameworkCodes().Result.Select(x => restApiClientforPP.GetByFrameworkCode(x.FrameworkCode).Result).ToList();
            var frameworkcodesinProd = restApiClientforProd.GetAllFrameworkCodes().Result.Select(x => restApiClientforProd.GetByFrameworkCode(x.FrameworkCode).Result).ToList();

            Assert.Multiple(() =>
            {
                foreach (var frameworkcodeinPP in frameworkcodesinPP)
                {
                    var frameworkcodeinprod = frameworkcodesinProd.Single(x => x.FrameworkCode == frameworkcodeinPP.FrameworkCode);
                    Assert.AreEqual(frameworkcodeinprod.Title, frameworkcodeinPP.Title, $"Framework Code Title looks different for {frameworkcodeinprod.FrameworkCode}");
                    Assert.AreEqual(frameworkcodeinprod.Ssa1, frameworkcodeinPP.Ssa1, $"Framework Code SSA1 looks different for {frameworkcodeinprod.FrameworkCode}");
                    Assert.AreEqual(frameworkcodeinprod.Ssa2, frameworkcodeinPP.Ssa2, $"Framework Code SSA2 looks different for {frameworkcodeinprod.FrameworkCode}");
                }
            });
        }

        private Func<IEnumerable<string>, IEnumerable<string>, List<string>> Comapare = (a, b) =>
        {
            return a.Except(b).ToList();
        };

        [Test]
        public void ShouldHaveSameInformation()
        {
            
            _preProdreportingClient = new ReportingClient(baseUri: "");
            var standardsinPP = _preProdreportingClient.standardApiClient.FindAll().Where(x => x.IsPublished == true).Select(y => _preProdreportingClient.standardApiClient.Get(y.Id)).ToList();
            var frameworksinPP = _preProdreportingClient.frameworkApiClient.FindAll().Select(y => _preProdreportingClient.frameworkApiClient.Get(y.Id)).ToList();

            var standardsinProd = _reportingClient.standardApiClient.FindAll().Where(x => x.IsPublished == true).Select(y => _preProdreportingClient.standardApiClient.Get(y.Id)).ToList();
            var frameworksinProd = _reportingClient.frameworkApiClient.FindAll().Select(y => _preProdreportingClient.frameworkApiClient.Get(y.Id)).ToList();

            var frameworkmissinginProd = Comapare(frameworksinPP.Select(x => x.FrameworkId), frameworksinProd.Select(x => x.FrameworkId));
            var frameworkmissinginPP = Comapare(frameworksinProd.Select(x => x.FrameworkId), frameworksinPP.Select(x => x.FrameworkId));

            var standardmissinginProd = Comapare(standardsinPP.Select(x => x.StandardId), standardsinProd.Select(x => x.StandardId));
            var standardmissinginPP = Comapare(standardsinProd.Select(x => x.StandardId), standardsinPP.Select(x => x.StandardId));

                Assert.Multiple(() =>
                {
                    Assert.AreEqual(standardsinProd.Count, standardsinPP.Count, $"Standard counts mismatch");
                    Assert.AreEqual(frameworksinProd.Count, frameworksinPP.Count, $"Framework counts mismatch");

                    Assert.IsTrue(frameworkmissinginProd.Count == 0, $"Missing Frameworks in Prod " + string.Join(",", frameworkmissinginProd));
                    Assert.IsTrue(frameworkmissinginPP.Count == 0, $"Missing Frameworks in PP " + string.Join(",", frameworkmissinginPP));
    
                    Assert.IsTrue(standardmissinginProd.Count == 0, $"Missing Standards in Prod " + string.Join(",", standardmissinginProd));
                    Assert.IsTrue(standardmissinginPP.Count == 0, $"Missing Standards in PP " + string.Join(",", standardmissinginPP));
                });

                foreach (var standardinPP in standardsinPP)
                {
                    var standardinprod = standardsinProd.Single(x => x.StandardId == standardinPP.StandardId);
                    Assert.AreEqual(standardinprod.Title, standardinPP.Title, $"Standard level looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.Duration, standardinPP.Duration, $"Standard Duration looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.IsPublished, standardinPP.IsPublished, $"Standard IsPublished looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.Level, standardinPP.Level, $"Standard Level looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.MaxFunding, standardinPP.MaxFunding, $"Standard MaxFunding looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.Ssa1, standardinPP.Ssa1, $"Standard Ssa1 looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.Ssa2, standardinPP.Ssa2, $"Standard Ssa2 looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.AssessmentPlanPdf, standardinPP.AssessmentPlanPdf, $"Standard AssessmentPlanPdf looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.EntryRequirements, standardinPP.EntryRequirements, $"Standard EntryRequirements looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.IntroductoryText, standardinPP.IntroductoryText, $"Standard IntroductoryText looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.JobRoles, standardinPP.JobRoles, $"Standard JobRoles looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.Keywords, standardinPP.Keywords, $"Standard Keywords looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.OverviewOfRole, standardinPP.OverviewOfRole, $"Standard OverviewOfRole looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.ProfessionalRegistration, standardinPP.ProfessionalRegistration, $"Standard ProfessionalRegistration looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.Qualifications, standardinPP.Qualifications, $"Standard Qualifications looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.StandardPdf, standardinPP.StandardPdf, $"Standard StandardPdf looks different for {standardinprod.StandardId}");
                    Assert.AreEqual(standardinprod.WhatApprenticesWillLearn, standardinPP.WhatApprenticesWillLearn, $"Standard WhatApprenticesWillLearn looks different for {standardinprod.StandardId}");
                }

                foreach (var frameworkinPP in frameworksinPP)
                {
                    var frameworkinProd = frameworksinProd.Single(x => x.FrameworkId == frameworkinPP.FrameworkId);
                    Assert.AreEqual(frameworkinProd.Title, frameworkinPP.Title, $"Framework level looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.FrameworkCode, frameworkinPP.FrameworkCode, $"Framework FrameworkCode looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.FrameworkName, frameworkinPP.FrameworkName, $"Framework FrameworkName looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.Level, frameworkinPP.Level, $"Framework Level looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.PathwayCode, frameworkinPP.PathwayCode, $"Framework PathwayCode looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.PathwayName, frameworkinPP.PathwayName, $"Framework PathwayName looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.ProgType, frameworkinPP.ProgType, $"Framework ProgType looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.Ssa1, frameworkinPP.Ssa1, $"Framework Ssa1 looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.Ssa2, frameworkinPP.Ssa2, $"Framework Ssa2 looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.CombinedQualification, frameworkinPP.CombinedQualification, $"Framework CombinedQualification looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.CompetencyQualification, frameworkinPP.CompetencyQualification, $"Framework CompetencyQualification looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.CompletionQualifications, frameworkinPP.CompletionQualifications, $"Framework CompletionQualifications looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.EntryRequirements, frameworkinPP.EntryRequirements, $"Framework EntryRequirements looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.ExpiryDate, frameworkinPP.ExpiryDate, $"Framework ExpiryDate looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.FrameworkOverview, frameworkinPP.FrameworkOverview, $"Framework FrameworkOverview looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.KnowledgeQualification, frameworkinPP.KnowledgeQualification, $"Framework KnowledgeQualification looks different for {frameworkinProd.FrameworkId}");
                    Assert.AreEqual(frameworkinProd.ProfessionalRegistration, frameworkinPP.ProfessionalRegistration, $"Framework ProfessionalRegistration looks different for {frameworkinProd.FrameworkId}");
                    var ProdRole = frameworkinProd.JobRoleItems.Select(x => $"{x.Title}-{x.Description}");
                    var PpRole = frameworkinPP.JobRoleItems.Select(x => $"{x.Title}-{x.Description}");
                    Assert.AreEqual(ProdRole, PpRole, $"Framework JobRoleItems looks different for {frameworkinProd.FrameworkId}");
            }
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
        public void ShouldNothave0duration()
        {
            _preProdreportingClient = new ReportingClient(baseUri: "");
            var frameworks = _preProdreportingClient.GetAllApprenticeshipFrameworks();
            var standards = _preProdreportingClient.GetAllApprenticeshipStandards();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(0, frameworks.Where(x => x.Duration == 0).Count(), string.Join(Environment.NewLine, frameworks.Where(x => x.Duration == 0).Select(y => $"{y.Id} has 0 duration")));
                Assert.AreEqual(0, frameworks.Where(x => x.MaxFunding == 0).Count(), string.Join(Environment.NewLine, frameworks.Where(x => x.MaxFunding == 0).Select(y => $"{y.Id} has 0 funding")));
                Assert.AreEqual(0, standards.Where(x => x.Duration == 0).Count(), string.Join(Environment.NewLine, standards.Where(x => x.Duration == 0).Select(y => $"{y.Id} has 0 duration")));
                Assert.AreEqual(0, standards.Where(x => x.MaxFunding == 0).Count(), string.Join(Environment.NewLine, standards.Where(x => x.MaxFunding == 0).Select(y => $"{y.Id} has 0 funding")));
            });
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

