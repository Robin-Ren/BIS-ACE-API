using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BisAceAPIBase
{
    public abstract class ABisAceAPICallContext : ABisAPICallContext
    {
        public BisApplicationConfig BisConfig { get; set; }
    }
}
