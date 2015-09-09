using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library
{
    public enum ErrorCodes
    {
        AlreadyPresentState = 1,
        UnknownState,
        InvalidArc,
        AlreadyPresentArc,
        UnknownArc,
        InvalidTransition,
        //PostedStateAreadySet,
        AlreadyTransiting
    }
}
