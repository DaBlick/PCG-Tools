/*
 * using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace PcgTools.Synths.Common.Synth
{
    public class Parameters: ObservableCollection<Parameter>
    {
        private Parameter this[string name]
        {
            get { return this.FirstOrDefault(t => t.Name == name); }
        }

        public static bool GetBoolValue(INavigation navigation, string name)
        {
            var parameter = navigation.StaticParameters.FirstOrDefault(param => param.Name == name);
            if (parameter == null)
            {
                throw new ApplicationException(String.Format("Parameter {0} not found.", name));
            }

            return parameter.BoolValue;
        }


        public static void SetBoolValue(INavigation navigation, string name, int boolValue)
        {
            var parameter = navigation.StaticParameters.FirstOrDefault(param => param.Name == name);
            if (parameter == null)
            {
                throw new ApplicationException(String.Format("Parameter {0} not found.", name));
            }

            parameter.BoolValue = (boolValue != 0);
        }


        public static void SetBoolValue(INavigation navigation, string name, bool boolValue)
        {
            var parameter = navigation.StaticParameters.FirstOrDefault(param => param.Name == name);
            if (parameter == null)
            {
                throw new ApplicationException(String.Format("Parameter {0} not found.", name));
            }

            parameter.BoolValue = boolValue;
        }

        public static int GetIntValue(INavigation navigation, string name)
        {
            var parameter = navigation.StaticParameters.FirstOrDefault(param => param.Name == name);
            if (parameter == null)
            {
                throw new ApplicationException(String.Format("Parameter {0} not found.", name));
            }

            return parameter.IntValue;
        }

        public static void SetIntValue(INavigation navigation, string name, int intValue)
        {
            var parameter = navigation.StaticParameters.FirstOrDefault(param => param.Name == name);
            if (parameter == null)
            {
                throw new ApplicationException(String.Format("Parameter {0} not found.", name));
            }

            parameter.IntValue = intValue;
        }
    }
}
*/