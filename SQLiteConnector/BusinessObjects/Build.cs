using System;
using System.Collections.Generic;

namespace SQLite.BusinessObjects
{
    public class Build
    {
        public int Id { get; set; }
        public string BuildNumber { get; set; }
        public string Status { get; set; }
    }
}
