using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BisAceAPIBase;

namespace BisAceDIContainer.DIContainer
{
    public abstract class ABisCallContextFactory<T> where T : ABisAceAPICallContext
    {
        public abstract IDictionary<string, T> BuildCallContexts(int systemId);
    }
}
