using System.Collections.Generic;

namespace Weave.ViewModels
{
    internal class FeedEqualityComparer : IEqualityComparer<Feed>
    {
        public bool Equals(Feed x, Feed y)
        {
            if (x == y)
                return true;

            if (x == null || y == null)
                return false;

            return x.Id == y.Id;
        }

        public int GetHashCode(Feed obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
