using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinVer
{
    public static class Tools
    {
        public static string[] ExpandFlags(Enum SomeEnum)
        {
            var ParamType = SomeEnum.GetType();
            if (!ParamType.IsEnum)
            {
                throw new ArgumentException("Parameter Type must be Enum");
            }
            if (ParamType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
            {
                var Values = new List<string>();
                foreach(var Value in Enum.GetValues(ParamType).OfType<Enum>())
                {
                    if (SomeEnum.HasFlag(Value))
                    {
                        Values.Add(Value.ToString());
                    }
                }
                return Values.ToArray();
            }
            else
            {
                if(!ParamType.IsEnumDefined(SomeEnum))
                {
                    throw new ArgumentException($"{SomeEnum} is not defined in {ParamType.FullName}");
                }
                return new string[] { SomeEnum.ToString() };
            }
        }
    }
}
