using System;
using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using Esfa.Das.Reporting.Types;

namespace Esfa.Das.Reporting.Client
{
    public interface IRepotingClient
    {
        /// <summary>
        /// Get all the active providers
        /// GET /providers/
        /// </summary>
        /// <returns>a collection of Providers</returns>
        IEnumerable<ProviderSummary> GetAllMainProviders();

        IEnumerable<ProviderLocations> GetAllMainProviderLocations();
    }
}
