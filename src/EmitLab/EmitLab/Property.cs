using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EmitLab
{
    /// <summary>
    /// This property
    /// </summary>
    public class Property
    {
        /// <summary>
        /// cache the currnet module to speed up the creation of getter/setter delegates
        /// </summary>
        static Module currentModule = Assembly.GetExecutingAssembly().GetType().Module;
        PropertyInfo _propInfo;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="propInfo"></param>
        public Property(PropertyInfo propInfo)
        {
            if (propInfo == null)
                throw new ArgumentNullException("propInfo");

            this._propInfo = propInfo;
            this._name = _propInfo.Name;
            Init(_propInfo);
        }

        /// <summary>
        /// Init
        /// </summary>
        private void Init(PropertyInfo propInfo)
        {
            Getter = CreateGetter(propInfo);
            Setter = CreateSetter(propInfo);
        }
        private Action<object,object> CreateSetter(PropertyInfo propInfo)
        {
            Type[] methodArgs = { typeof(object) };
            DynamicMethod dm = new DynamicMethod(propInfo.Name + "Setter", typeof(object), methodArgs, currentModule);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, propInfo.GetGetMethod());
            // Return
            il.Emit(OpCodes.Ret);
            return (Action<object, object>)dm.CreateDelegate(typeof(Action<object, object>));
        }

        private Func<object,object> CreateGetter(PropertyInfo propInfo)
        {
            Type[] methodArgs = { typeof(object) };
            DynamicMethod dm = new DynamicMethod(propInfo.Name + "Getter", typeof(object), methodArgs, currentModule);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, propInfo.GetGetMethod());
            // Return
            il.Emit(OpCodes.Ret);
            return (Func<object, object>)dm.CreateDelegate(typeof(Func<object, object>));
        }

        string _name;
        public string Name
        {
            get { return _name; }
        }

        Func<object> ObjResolver { get; set; }

        public Func<object,object> Getter { get; private set; }
        public Action<object,object> Setter { get; private set; }

        public bool IsReadOnly { get { return Setter == null; } }
    }
}
