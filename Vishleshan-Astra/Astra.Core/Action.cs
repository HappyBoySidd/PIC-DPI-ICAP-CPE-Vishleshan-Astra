using System;
using System.Collections.Generic;
using System.Text;

namespace Vishleshan_Astra.Astra_Core
{
    class Action
    {
        public enum Actions
        {
            CheckConfiguration = 1,
            ValidateFileSanity = 2,
            NotifyStakeHolders = 3,
            Exit = 4
        }
    }
}
