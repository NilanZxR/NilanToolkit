using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NilanToolkit.Utils{

    public static class ReflectionUtil {

        public static bool IsAttributeExist<T>(MemberInfo info) where T : Attribute {
            return info.GetCustomAttributes(typeof(T)).Any();
        }

        public static IEnumerable<MemberInfo> GetAllMemberByAttribute<T>(Type type) where T : Attribute {
            return type.GetMembers().Where(IsAttributeExist<T>);
        }

        public static IEnumerable<MethodInfo> GetAllMethodByAttribute<T>(Type type) where T : Attribute {
            return type.GetMethods().Where(IsAttributeExist<T>);
        }

    }

}
