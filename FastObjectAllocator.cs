using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace enbask.utils
{    
    public static class FastObjectAllocator<T> where T : new()
    {
        //since this is a static generic class, every type of T will have its
        //own instance. This leads to a unique mObjectCreator for every type of T.
        private static Func<T> mObjectCreator = null;

        public static T New()
        {
            //lazy create the alloc generator
            //This introduces a branch test for every allocation, which could be removed
            //if you implemented a pre-create system for Types that use this.
            if (mObjectCreator == null)
            {
                var objectType = typeof(T);
                var defaultCtor = objectType.GetConstructor(new Type[] { });

                var dynMethod = new DynamicMethod(
                    name: string.Format("_{0:N}", Guid.NewGuid()),
                    returnType: objectType,
                    parameterTypes: null
                    );

                ILGenerator il = dynMethod.GetILGenerator();
                il.Emit(OpCodes.Newobj, defaultCtor);
                il.Emit(OpCodes.Ret);

                mObjectCreator = dynMethod.CreateDelegate(typeof(Func<T>)) as Func<T>;
            }

            //There is no getting around having to make a function call to execute the newobj op code
            //The the JIT will also never inline this, as it's a delegate call.
            //This is why we can never be as fast as new Object() if the type is known at compile time.
            return mObjectCreator();
        }

    }
}
