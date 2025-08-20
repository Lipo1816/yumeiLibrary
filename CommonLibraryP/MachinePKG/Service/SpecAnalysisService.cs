using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG.Service
{
 public   class SpecAnalysisService
    {
        // $"{scriptPath} {args}",
        public string RunPythonScript(string scriptPath, string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"{scriptPath}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(psi);
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }
}
