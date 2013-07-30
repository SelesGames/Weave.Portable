using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weave.ViewModels
{
    internal static class IEnumerableExtensions
    {
        //public static SetComparison<T> GetSetComparison<T>(
        //    this IEnumerable<T> first, 
        //    IEnumerable<T> second, 
        //    IEqualityComparer<T> comparer)
        //{
        //    //List<T> same = new List<T>(), added = new List<T>(), removed = new List<T>();

        //    if (first == null)
        //        throw new ArgumentNullException("first in IEnumerableExtensions.GetSetComparison");

        //    if (second == null)
        //        return SetComparison<T>.Empty;

        //    var same = first.Intersect(second, comparer);
        //    var added = second.Except(first, comparer);
        //    var removed = first.Except(second, comparer);

        //    return new SetComparison<T>
        //    {
        //        Same = same,
        //        Added = added,
        //        Removed = removed
        //    };
        //}
    }
}
