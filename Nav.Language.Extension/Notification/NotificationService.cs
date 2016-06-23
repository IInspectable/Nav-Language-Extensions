#region Using Directives

using System;
using Microsoft.CodeAnalysis;

using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Notification {

    public class NavTaskAnnotationChangedArgs : EventArgs {

        public DocumentId DocumentId { get; set; }
        public NavTaskAnnotation TaskAnnotation { get; set; }
    }

    class NotificationService {
        
        public static void RaiseChanged(NavTaskAnnotationChangedArgs e) {
            
        }

    }
}
