using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttoExporter.Util
{
    public struct TempFileScheme
    {
        public TempFileType Type;
        public string TempPath;
        public string NameScheme;
    }

    public enum TempFileType { ReportPreview, OxyPlot, Image }
}
