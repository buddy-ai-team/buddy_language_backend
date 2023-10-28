using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Domain.Enumerations
{
    public enum WordEntityStatus
    {
        /// <summary>
        ///   Word is being learned
        /// </summary>
        Learning,

        /// <summary>
        ///  Word is learned
        /// </summary>
        Learned,

        /// <summary>
        ///  Word is removed from learning
        /// </summary>
        Dropped
    }
}
