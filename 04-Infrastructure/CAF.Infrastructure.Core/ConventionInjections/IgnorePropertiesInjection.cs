using System;
using System.Linq;
using Omu.ValueInjecter;

namespace CAF.Infrastructure.Core.ConventionInjections
{
    public class IgnorePropertiesInjection : LoopValueInjection
    {
        private string[] ignore;

        public IgnorePropertiesInjection(params string[] ignore)
        {
            this.ignore = ignore;
        }

        protected override bool UseSourceProp(string sourcePropName)
        {
            return !ignore.Any(i => string.Equals(i, sourcePropName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
