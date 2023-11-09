using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Domain.Enumerations
{
    /// <summary>
    /// Represents the supported speeds for text-to-speech synthesis.
    /// </summary>
    public enum TtsSpeed
    {
        /// <summary>
        /// Extra Slow Speed.
        /// </summary>
       Xslow,

        /// <summary>
        /// Slow Speed.
        /// </summary>
       Slow,

        /// <summary>
        /// Medium Speed.
        /// </summary>
       Medium,

        /// <summary>
        /// Fast Speed.
        /// </summary>
       Fast,

        /// <summary>
        /// Extra Fast Speed.
        /// </summary>
       Xfast,
    }
}
