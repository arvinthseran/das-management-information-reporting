using System;
using System.Collections.Generic;
using System.Linq;
using Esfa.Das.Reporting.Types.Apprenticeship;

namespace Esfa.Das.Reporting.Client
{
    public class ApprenticeshipStandardReportingClient : ReportingClientBase
    {
        public ApprenticeshipStandardReportingClient(string apprenticeshipServiceUri = null, string courceDirectoryUri = null) : base(apprenticeshipServiceUri, courceDirectoryUri)
        {
        }

        public IEnumerable<ApprenticeshipStandard> GetAllApprenticeshipStandards()
        {
            var appStandards = new List<ApprenticeshipStandard>();
            var standards = standardApiClient.FindAll().Where(x => x.IsPublished == true);
            foreach (var standardcode in standards.Select(x => x.Id))
            {
                var s = standardApiClient.Get(standardcode);
                appStandards.Add(new ApprenticeshipStandard
                {
                    Id = standardcode, Title = s.Title, OverviewOfRole = s.OverviewOfRole, Duration = s.Duration, MaxFunding = s.MaxFunding, Level = s.Level
                });
            }
            return appStandards;
        }

        public IEnumerable<ApprenticeshipFramework> GetAllApprenticeshipFrameworks()
        {
            var appFrameworks = new List<ApprenticeshipFramework>();
            var frameworks = frameworkApiClient.FindAll();
            foreach (var frameworkid in frameworks.Select(x => x.Id))
            {
                var f = frameworkApiClient.Get(frameworkid);
                appFrameworks.Add(new ApprenticeshipFramework
                {
                    Id = f.FrameworkId, Title = f.Title, Duration = f.Duration, MaxFunding = f.MaxFunding, EndDate = f.ExpiryDate, Level = f.Level
                });
            }
            return appFrameworks;
        }
    }
}
