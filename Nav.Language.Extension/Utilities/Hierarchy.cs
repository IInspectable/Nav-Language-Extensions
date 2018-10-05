#region Using Directives

using System;
using System.IO;

using JetBrains.Annotations;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Utilities {

    struct Hierarchy {

        private readonly IVsHierarchy _vsHierarchy;

        public Hierarchy(IVsHierarchy vsHierarchy) {
            _vsHierarchy = vsHierarchy ?? throw new ArgumentNullException(nameof(vsHierarchy));

        }

        const uint ItemId = VSConstants.VSITEMID_ROOT;

        public string FullPath {
            get {
                ThreadHelper.ThrowIfNotOnUIThread();

                var fullPath = GetMkDocument() ?? GetCanonicalName();

                if (!Path.IsPathRooted(fullPath)) {
                    return null;
                }

                return fullPath;
            }
        }

        [CanBeNull]
        public string GetMkDocument() {

            ThreadHelper.ThrowIfNotOnUIThread();

            // ReSharper disable once SuspiciousTypeConversion.Global
            var    ao  = _vsHierarchy as IVsProject;
            string doc = null;
            ao?.GetMkDocument(ItemId, out doc);
            return doc;
        }

        [CanBeNull]
        public string GetCanonicalName() {

            ThreadHelper.ThrowIfNotOnUIThread();

            string cn = null;
            if (ErrorHandler.Succeeded(_vsHierarchy?.GetCanonicalName(ItemId, out cn) ?? VSConstants.S_OK)) {
                return cn;
            }

            return null;
        }

        public string Name => GetProperty<string>(__VSHPROPID.VSHPROPID_Name);

        public Guid ProjectGuid {
            get {
                ThreadHelper.ThrowIfNotOnUIThread();

                _vsHierarchy.GetGuidProperty(ItemId, (int) __VSHPROPID.VSHPROPID_ProjectIDGuid, out var projectGuid);
               
                return projectGuid;
            }
        }

        T GetProperty<T>(__VSHPROPID propId, T defaultValue = default(T)) {

            ThreadHelper.ThrowIfNotOnUIThread();

            var value = GetPropertyCore((int) propId);
            if (value == null) {
                return defaultValue;
            }

            return (T) value;
        }

        object GetPropertyCore(int propId) {

            ThreadHelper.ThrowIfNotOnUIThread();

            if (propId == (int) __VSHPROPID.VSHPROPID_NIL) {
                return null;
            }

            _vsHierarchy.GetProperty(ItemId, propId, out object propValue);

            return propValue;
        }

    }

}