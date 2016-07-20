using System;
using Microsoft.VisualStudio.ComponentModelHost;

namespace Pharmatechnik.Nav.Language.Extension.Common {

    static class ServiceProviderExtensions {

        public static T GetMefService<T>(this IServiceProvider serviceProvider) where T : class {

            var componentModel = (IComponentModel)serviceProvider.GetService(typeof(SComponentModel));
            return componentModel.GetService<T>();            
        }
    }
}