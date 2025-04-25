using System.Collections.Generic;

namespace Stats
{
    /// <summary>
    /// Interface for components that can modify character statistics.
    /// Provides both additive and percentage-based stat modifications.
    /// </summary>
    public interface IModifierProvider
    {
        /// <summary>
        /// Returns a collection of additive modifiers for a given stat.
        /// </summary>
        /// <param name="stat">The stat to modify</param>
        /// <returns>Collection of additive modifier values</returns>
        public IEnumerable<float> GetAdditiveModifiers(Stat stat);

        /// <summary>
        /// Returns a collection of percentage-based modifiers for a given stat.
        /// </summary>
        /// <param name="stat">The stat to modify</param>
        /// <returns>Collection of percentage modifier values</returns>
        public IEnumerable<float> GetPercentageModifiers(Stat stat);
    }
}