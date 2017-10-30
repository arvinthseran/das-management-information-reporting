
using Esfa.Ofsted.Inspection.Types;
using System.Collections.Generic;


namespace Esfa.Das.Reporting.Types
{
    public class ProviderLocations 
    {
        public int Ukprn { get; set; }

        public string Name { get; set; }

        public List<TrainingLocation> TrainingLocations { get; set; }

        public List<ProviderApprenticeship> Frameworks { get; set; }

        public List<ProviderApprenticeship> Standards { get; set; }

        public OverallEffectiveness? OverallEffectiveness;
    }

    public class ProviderApprenticeship
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
