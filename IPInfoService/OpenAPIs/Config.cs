using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAPIs
{
    public class hostconfig
    {
        public string host { get; set; }
        public string url { get; set; }
    }

    public class openapiconfig
    {
        public List<hostconfig> config { get; set; }
    }  
}
