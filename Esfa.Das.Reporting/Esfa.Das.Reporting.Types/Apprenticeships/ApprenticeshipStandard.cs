using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esfa.Das.Reporting.Types.Apprenticeship
{
    public class ApprenticeshipStandard : ApprenticeshipDetails
    {
        public string OverviewOfRole { get; set; }
    }
    public class ApprenticeshipFramework : ApprenticeshipDetails
    {
        public DateTime? EndDate { get; set; }
    }

    public class ApprenticeshipDetails
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public int MaxFunding { get; set; }

        public int Duration { get; set; }

        public int Level { get; set; }
    }
}
