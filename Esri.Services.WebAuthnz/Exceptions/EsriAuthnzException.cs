using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esri.Services.WebAuthnz.Exceptions
{
    public sealed class EsriAuthnzException : Exception
    {
        public int HttpStatusCode { get; set; }
    }
}
