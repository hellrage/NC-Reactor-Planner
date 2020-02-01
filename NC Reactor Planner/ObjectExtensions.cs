using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NC_Reactor_Planner
{
    public static class ObjectExtensions
    {
        public static T Set<T>(this T item, Action<T> setter)
        {
            setter?.Invoke(item);
            return item;
        }
    }
}
