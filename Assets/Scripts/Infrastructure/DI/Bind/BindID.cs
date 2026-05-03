using System;
using System.Collections.Generic;

namespace BallisticSandbox.Infrastructure.DI.Bind
{
    public readonly struct BindID : IEquatable<BindID>
    {
        private readonly Type _contractType;
        private readonly object _identifier;

        public BindID(Type contractType, object identifier)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType), "Contract type cannot be null.");

            _contractType = contractType;
            _identifier = identifier;
        }

        public bool Equals(BindID other)
        {
            return _contractType == other._contractType &&
                   EqualityComparer<object>.Default.Equals(_identifier, other._identifier);
        }

        public override bool Equals(object obj)
        {
            if (obj is BindID other)
                return Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_contractType, _identifier);
        }
    }
}
