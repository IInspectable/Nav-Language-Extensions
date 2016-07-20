#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Imaging.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    sealed class ImageMonikerImageList: IVsImageMonikerImageList {

        readonly ImmutableList<ImageMoniker> _imageMonikers;

        public ImageMonikerImageList(IEnumerable<ImageMoniker> imageMonikers) {
            _imageMonikers = imageMonikers.ToImmutableList();
        }

        public ImageMonikerImageList(params ImageMoniker[] imageMonikers) {
            _imageMonikers = imageMonikers.ToImmutableList();
        }

        public void GetImageMonikers(int firstImageIndex, int imageMonikerCount, ImageMoniker[] imageMonikers) {
            for (int index = 0; index < imageMonikerCount; index++) {
                imageMonikers[index] = _imageMonikers[index + firstImageIndex];
            }
        }

        public int ImageCount {
            get { return _imageMonikers.Count; }
        }
    }
}