using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esri.Services.WebAuthnz.Authorization
{
    internal sealed class Decision
    {
        public bool IsAuthorized { get; private set; }

        public Evidence Evidence { get; private set; }

        public Decision(bool isAuthorized, Evidence evidence)
        {
            this.IsAuthorized = isAuthorized;
            this.Evidence = evidence;
        }
    }
}
