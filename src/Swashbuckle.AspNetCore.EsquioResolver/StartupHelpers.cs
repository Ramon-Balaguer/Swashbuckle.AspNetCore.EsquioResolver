using Esquio.Abstractions;
using Esquio.AspNetCore.Endpoints.Metadata;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Swashbuckle.AspNetCore.EsquioResolver
{
    public static class StartupHelpers
    {
        internal static IServiceProvider serviceProvider;

        public static void ResolveConflictingActionsByFeatureToggles(this SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.ResolveConflictingActions(ResolveConflict);
        }


        public static void UseResolveConflictingActionsByFeatureToggles(this IApplicationBuilder applicationServices)
        {
            serviceProvider = applicationServices.ApplicationServices;
        }

        private static ApiDescription ResolveConflict(IEnumerable<ApiDescription> descriptions)
        {
            foreach (var apiDescription in descriptions)
            {
                if(CheckCandidate(apiDescription))
                    return apiDescription;
            }
            return descriptions.First();
        }

        private static bool CheckCandidate(ApiDescription apiDescription)
        {
            switch (apiDescription.ActionDescriptor)
            {
                case ControllerActionDescriptor controllerActionDescriptor:
                    var filter = controllerActionDescriptor.MethodInfo.GetCustomAttribute<FeatureFilter>();
                    if (filter != null)
                    {
                        return FeatureEnabled(filter.Name);
                    }
                    return false;
                case null:
                    throw new ArgumentNullException(nameof(apiDescription.ActionDescriptor));
                default:
                    return true;
            }
        }

        private static bool FeatureEnabled(string featureName)
        {
            bool featureEnabled;
            using (var serviceScope = GetScope())
            {
                var featureService = serviceScope.ServiceProvider.GetService<IFeatureService>();
                featureEnabled = featureService.IsEnabledAsync(featureName).GetAwaiter().GetResult();
            }
            return featureEnabled;
        }

        private static IServiceScope GetScope()
        {
            if (serviceProvider == null)
                throw new Exception("Conflict resolver not configured, please use UseResolveConflictingActionsByFeatureToggles on your startup.cs (Configure method)");
            return serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }


    }
}
