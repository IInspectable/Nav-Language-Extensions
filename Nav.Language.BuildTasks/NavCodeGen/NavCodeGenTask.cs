#region Using Directives

using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

#endregion

namespace Nav.Language.BuildTasks {

    public class NavCodeGenTask: Task {

        public ITaskItem[] Files { get; set; }

        public override bool Execute() {

            WriteHeader();

            var files = Files.Select(item => item.ItemSpec);
            foreach(var file in files) {
                Log.LogWarning($"Nav: '{file}'");      
                //Log.LogMessage(MessageImportance.Low, $"Nav: '{file}'");
            }
            
            return true;
        }

        void WriteHeader() {
            var productInfo = $"{ThisAssembly.ProductName} Version {ThisAssembly.ProductVersion}";
            var separator = new string('=', productInfo.Length);
            Log.LogMessage(separator);
            Log.LogMessage(productInfo);
            Log.LogMessage(separator);
        }
    }
}
