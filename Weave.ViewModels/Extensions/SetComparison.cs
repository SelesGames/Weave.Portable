using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weave.ViewModels
{
    internal class SetComparison<T>
    {
        public IEnumerable<Tuple<T, T>> Same { get; set; }
        public IEnumerable<Tuple<T, T>> Added { get; set; }
        public IEnumerable<Tuple<T, T>> Removed { get; set; }

        public static SetComparison<T> Empty 
        { 
            get 
            {
                return new SetComparison<T>
                {
                    Same = new List<Tuple<T, T>>(0),
                    Added = new List<Tuple<T, T>>(0),
                    Removed = new List<Tuple<T, T>>(0)
                };
            }
        }
    }
}
