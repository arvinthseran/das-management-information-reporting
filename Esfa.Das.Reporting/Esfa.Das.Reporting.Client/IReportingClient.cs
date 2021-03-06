﻿using System;
using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using Esfa.Das.Reporting.Types;
using Esfa.Das.Reporting.Types.Apprenticeship;

namespace Esfa.Das.Reporting.Client
{
    public interface IReportingClient
    {
        /// <summary>
        /// Get all the active providers
        /// GET /providers/
        /// </summary>
        /// <returns>a collection of Providers</returns>
        IEnumerable<ProviderSummary> GetAllMainProviders();

        IEnumerable<SFA.Roatp.Api.Types.Provider> GetAllMainProvidersFromRoatp();

        IEnumerable<ProviderLocations> GetAllMainProviderLocations();

        void DownloadProviderDetails(List<int> providerUkprns);

        IEnumerable<ApprenticeshipStandard> GetAllApprenticeshipStandards();

        IEnumerable<ApprenticeshipFramework> GetAllApprenticeshipFrameworks();
    }
}
