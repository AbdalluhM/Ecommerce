using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Ecommerce.Core.Entities;

namespace Ecommerce.BLL.Comparers
{
    public class ApplicationComparer : IEqualityComparer<Application>
    {
        public bool Equals([AllowNull] Application x, [AllowNull] Application y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode([DisallowNull] Application obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}