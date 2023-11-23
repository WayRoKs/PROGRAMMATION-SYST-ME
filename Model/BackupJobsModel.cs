using System;
using System.Collections.Generic;
using System.Text;

namespace PROGRAMMATION_SYST_ME.Model
{
    class BackupJobsModel
    {
        public string Name {  get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public int type { get; set; } // O is full backup and 1 is differential backup
    }
}
