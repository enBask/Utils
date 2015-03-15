using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enbask
{
    public static partial class Utils
    {
        //Since this is a static generic, a unique static version will be created for every combo of Arg & Ret.
        public static Func<Arg, Ret> Memoize<Arg, Ret>(this Func<Arg, Ret> functor)
        {
            //pay attention to what is being returned here. We are returning a lambda with the correct
            //Func<Arg,Ret> signature.
            //Along with that we are creating a dictionary instance outside the function, and using it within
            //the anonymous method.

            //since this is a static function for generics, it means every combo of Arg & Ret will have it's own
            //dictionary instance.
            var memo_table = new Dictionary<Arg, Ret>();
            return arg0 =>
            {
                //very simple wrapper that checks if we've stored the Arg value -> return value lookup.
                Ret func_return_val;
                if (!memo_table.TryGetValue(arg0, out func_return_val))
                {
                    //this will be the expensive call as we have to call the function. The arg->ret set will
                    //get added to the lookup for future calls.
                    func_return_val = functor(arg0);
                    memo_table.Add(arg0, func_return_val);
                }
                return func_return_val;
            };
        }
    }


}
