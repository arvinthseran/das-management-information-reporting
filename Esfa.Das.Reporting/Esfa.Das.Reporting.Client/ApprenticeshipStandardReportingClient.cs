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
            var standards = _standardApiClient.FindAll().Where(x => x.IsPublished == true);
            foreach (var standardcode in standards.Select(x => x.Id))
            {
                var standard = _standardApiClient.Get(standardcode);
                appStandards.Add(new ApprenticeshipStandard { Id = standardcode, Title = standard.Title, OverviewOfRole = standard.OverviewOfRole });
            }
            return appStandards;
        }
    }
}
