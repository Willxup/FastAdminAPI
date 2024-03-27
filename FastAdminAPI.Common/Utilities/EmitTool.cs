using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FastAdminAPI.Common.Utilities
{
    public static class EmitTool
    {
        /// <summary>
        /// 为对象的属性设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">对象的属性名</param>
        /// <param name="value">要设置的值</param>
        public static void SetObjectPropertyValue<T>(T obj, string propertyName, object value)
        {
            //获取泛型的类型
            Type type = typeof(T);

            //创建动态方法
            DynamicMethod dynamicMethod = new("EmitSetObjectPropertyValue", null, new[] { type, typeof(object) }, type.Module);

            //获取属性SET方法
            MethodInfo callMethod = type.GetMethod("set_" + propertyName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);

            //获取方法的参数
            ParameterInfo parameterInfo = callMethod.GetParameters()[0];

            //获取IL生成器
            ILGenerator iLGenerator = dynamicMethod.GetILGenerator();

            //声明一个指定类型的本地变量
            LocalBuilder local = iLGenerator.DeclareLocal(parameterInfo.ParameterType, true);

            //加载第二个参数到堆栈
            iLGenerator.Emit(OpCodes.Ldarg_1);

            //将属性值转换为适当的类型
            if (parameterInfo.ParameterType.IsValueType) //是否为值类型
            {
                iLGenerator.Emit(OpCodes.Unbox_Any, parameterInfo.ParameterType); //如果是值类型，拆箱
            }
            else
            {
                iLGenerator.Emit(OpCodes.Castclass, parameterInfo.ParameterType); //如果是引用类型，转换
            }

            //将堆栈上的值存储到本地变量
            iLGenerator.Emit(OpCodes.Stloc, local);
            //加载第一个参数到堆栈
            iLGenerator.Emit(OpCodes.Ldarg_0);
            //将本地变量加载到堆栈
            iLGenerator.Emit(OpCodes.Ldloc, local);

            //调用对象的虚方法
            iLGenerator.EmitCall(OpCodes.Callvirt, callMethod, null);
            //退出
            iLGenerator.Emit(OpCodes.Ret);

            //创建委托并调用：为对象属性设置值
            var setter = dynamicMethod.CreateDelegate(typeof(Action<T, object>)) as Action<T, object>;
            setter(obj, value);


            #region ChatGPT生成
            //Type type = typeof(T);

            //PropertyInfo property = type.GetProperty(propertyName);

            //// 创建动态方法
            //DynamicMethod setterMethod = new("SetObjectPropertyValue", typeof(void), new Type[] { type, typeof(object) }, true);

            //ILGenerator il = setterMethod.GetILGenerator();

            //// 将对象引用加载到堆栈
            //il.Emit(OpCodes.Ldarg_0);

            //// 将属性值转换为适当的类型
            //if (property.PropertyType.IsValueType)
            //{
            //    il.Emit(OpCodes.Ldarg_1);
            //    il.Emit(OpCodes.Unbox_Any, property.PropertyType);
            //}
            //else
            //{
            //    il.Emit(OpCodes.Ldarg_1);
            //    il.Emit(OpCodes.Castclass, property.PropertyType);
            //}

            //// 调用属性的set方法
            //MethodInfo setMethod = property.GetSetMethod();
            //il.Emit(OpCodes.Callvirt, setMethod);

            //// 返回
            //il.Emit(OpCodes.Ret);

            //// 创建委托并调用
            //Action<T, object> setter = (Action<T, object>)setterMethod.CreateDelegate(typeof(Action<T, object>));
            //setter(obj, value);
            #endregion
        }
    }
}
