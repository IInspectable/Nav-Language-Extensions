#region Using Directives

using System;
using Microsoft.CodeAnalysis;

using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Notification {

    public class ClassAnnotationChangedArgs : EventArgs {

        public DocumentId DocumentId { get; set; }
        public NavTaskAnnotation TaskAnnotation { get; set; }
    }

    interface IClassAnnotationChangeListener {
        void OnClassAnnotationsChanged(object sender, ClassAnnotationChangedArgs e);
    }

    static class NotificationService {

        static readonly WeakListenerManager<IClassAnnotationChangeListener> ClassAnnotationChangeListener;

        static NotificationService() {
            ClassAnnotationChangeListener = new WeakListenerManager<IClassAnnotationChangeListener>();
        }

        #region ClassAnnotationChanged

        public static void RaiseClassAnnotationChanged(object sender, ClassAnnotationChangedArgs e) {
            ClassAnnotationChangeListener.InvokeListener(listener => listener.OnClassAnnotationsChanged(sender, e));
        }

        public static void AddClassAnnotationChangeListener(IClassAnnotationChangeListener listener) {
            ClassAnnotationChangeListener.AddListener(listener);
        }

        public static void RemoveClassAnnotationChangeListener(IClassAnnotationChangeListener listener) {
            ClassAnnotationChangeListener.RemoveListener(listener);
        }

        #endregion
    }
}
