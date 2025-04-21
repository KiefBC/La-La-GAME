using System.Collections.Generic;

namespace Stats
{
    public interface IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifiers(Stat stat);
        public IEnumerable<float> GetPercentageModifiers(Stat stat);
    }
}
