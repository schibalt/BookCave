using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookCave.Service
{
    [System.Management.Automation.Cmdlet("Remove", "UseStatement")]
    public class DeploymentCmdlet : System.Management.Automation.PSCmdlet
    {
        [System.Management.Automation.Parameter(Position = 0, Mandatory = true)]
        public string AppProvisioningPath;

        protected override void ProcessRecord()
        {
            var lines = new List<string>(File.ReadAllLines(AppProvisioningPath));
            lines[10] = "--" + lines[10];
            lines[11] = "--" + lines[11];

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(AppProvisioningPath))
                foreach (string line in lines)
                {
                    file.WriteLine(line);

                    //Console.WriteLine(line);
                }
            Console.WriteLine("USE [bookcave]; commented out");
            }
    }
}
