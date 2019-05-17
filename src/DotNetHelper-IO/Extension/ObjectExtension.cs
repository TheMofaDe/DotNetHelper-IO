using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetHelper_IO.Extension
{
    internal static class ObjectExtension
    {
        internal static int ToInt(this object o)
        {
            return o.ToInt(true);
        }

        internal static int ToInt(this object o, bool throwEx, int returnValueOnError = default(int))
        {
            if (throwEx)
            {
                return Convert.ToInt32(o);
            }
            else
            {
                if (o == null) return returnValueOnError;
                if (o is string s)
                {
                    if (string.IsNullOrEmpty(s)) return returnValueOnError;
                    int.TryParse(s, out var i);
                    return i;
                }
                else
                {
                    int i;
                    try
                    {
                        i = Convert.ToInt32(o);
                    }
                    catch
                    {
                        i = returnValueOnError;
                    }

                    return i;
                }
            }
        }
    }
}
