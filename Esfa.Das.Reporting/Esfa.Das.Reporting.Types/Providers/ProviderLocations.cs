using SFA.DAS.Apprenticeships.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esfa.Das.Reporting.Types
{
    public class ProviderLocations 
    {
        public int Ukprn { get; set; }

        public List<TrainingLocation> TrainingLocations { get; set; }

        public List<Apprenticeship> Frameworks { get; set; }

        public List<Apprenticeship> Standards { get; set; }
    }

    public class Apprenticeship
    {
        public string ApprenticeshipId { get; set; }

        public string ApprenticeshipName { get; set; }

        public ApprenticeshipType ApprenticeshipType { get; set; }

        public List<ApprenticeshipTrainingLocation> ApprenticeshipTrainingLocations { get; set; }
    }

    public enum ApprenticeshipType
    {
        Framework,
        Standard
    }

    public class ApprenticeshipTrainingLocation
    {
        public int LocationId { get; set; }

        public List<string> DeliveryModes { get; set; }
        
    }

    public class TrainingLocation
    {
        public int LocationId { get; set; }

        public string LocationName { get; set; }

        public Address Address { get; set; }

    }
    public class Address
    {
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Town { get; set; }

        public string Postcode { get; set; }
    }
}
