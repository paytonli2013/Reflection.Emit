using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EmitLab
{
    public class MainClass
    {
        public static void Main()
        {
            Type[] methodArgs = { typeof(string), typeof(MainClass) };

            // The MapDR method will map a DbDataReader row to an instance of type T
            DynamicMethod dm = new DynamicMethod("ConsoleWriteline", typeof(string), methodArgs, Assembly.GetExecutingAssembly().GetType().Module);
            ILGenerator il = dm.GetILGenerator();
            var writeline = typeof(MainClass).GetMethod("WriteLine", new Type[] { typeof(string) });
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            
            il.Emit(OpCodes.Call, writeline);
            // Return
            il.Emit(OpCodes.Ret);
            Func<string, MainClass, string> func = (Func<string, MainClass, string>)dm.CreateDelegate(typeof(Func<string, MainClass, string>));
            var result = func.Invoke("Hello", new MainClass());
            System.Console.Read();
        }

        public string WriteLine(string msg)
        {
            System.Console.WriteLine(msg);
            return msg;
        }
    }
}
