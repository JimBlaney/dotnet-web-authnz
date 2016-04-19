using System;

using Esri.Services.WebAuthnz.DataStructures;

namespace Esri.Services.WebAuthnz.Logic
{
    public interface ILogicalTruth
    {
        bool Negate { get; set; }

        bool Evaluate(ReadOnlyDictionary<string, string[]> properties);
    }
}
