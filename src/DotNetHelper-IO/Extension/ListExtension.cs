using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetHelper_IO.Extension
{
    internal static class ListExtension
    {


        /// <summary>
        /// Method Name Pretty Much Says It All
        /// </summary> 
        /// <param name="source"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this List<T> source, Func<T, bool> whereClause = null)
        {
            if (whereClause == null) return source == null || !source.Any();
            return source == null || !source.Any(whereClause);
        }


        /// <summary>
        /// Obtains the data as a list; if it is *already* a list, the original object is returned without
        /// any duplication; otherwise, ToList() is invoked.
        /// </summary>
        /// <typeparam name="T">The type of element in the list.</typeparam>
        /// <param name="source">The enumerable to return as a list.</param>
        public static List<T> AsList<T>(this IEnumerable<T> source) => (source == null || source is List<T>) ? (List<T>)source : source.ToList();

    }
}
