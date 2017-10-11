using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esfa.Das.Reporting.Types.Apprenticeship
{
    public class ApprenticeshipStandard
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string OverviewOfRole { get; set; }
    }
    public class ApprenticeshipFramework
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public int Duration { get; set; }
    }
}
