using NuLibrary.Migration.SQLDatabase.SQLHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.AppEnums
{
    public class DynamicEnums
    {
        AppDomain curDomain = AppDomain.CurrentDomain;
        AssemblyName aName = new AssemblyName("NuMedicsGlobalEnums");
        NumedicsGlobalHelpers gHelp = new NumedicsGlobalHelpers();

        AssemblyBuilder aBuilder;
        ModuleBuilder mBuilder;

        public DynamicEnums()
        {
            Intit();
        }

        private void Intit()
        {
            aBuilder = curDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);
            mBuilder = aBuilder.DefineDynamicModule(aName.Name, $"{aName.Name}.dll");

            CreateUserTypeEnum();
            CreateTherapyTypeEnum();
            CreateReadingEventTypeEnum();
            CreatePaymentMethodEnum();
            CreateCheckStatusEnum();
            CreateInsuinTypeEnum();

            aBuilder.Save($"{aName.Name}.dll");

        }

        private void CreateInsuinTypeEnum()
        {
            EnumBuilder eBuilder = mBuilder.DefineEnum("InsulinType", TypeAttributes.Public, typeof(int));

            foreach (var ln in gHelp.GetAllInsulinTypes())
            {
                eBuilder.DefineLiteral(ln.Type.Replace(" ", ""), ln.InsulinTypeId);
            }

            Type UserTypeEnum = eBuilder.CreateType();
        }

        private void CreateCheckStatusEnum()
        {
            EnumBuilder eBuilder = mBuilder.DefineEnum("CheckStatus", TypeAttributes.Public, typeof(int));

            foreach (var ch in gHelp.GetAllCheckStatusTypes())
            {
                eBuilder.DefineLiteral(ch.Status.Replace(" ", ""), ch.StatusId);
            }

            Type UserTypeEnum = eBuilder.CreateType();
        }

        private void CreatePaymentMethodEnum()
        {
            EnumBuilder eBuilder = mBuilder.DefineEnum("PaymentMethod", TypeAttributes.Public, typeof(int));

            foreach (var pm in gHelp.GetAllPaymentMethods())
            {
                eBuilder.DefineLiteral(pm.MethodName.Replace(" ", ""), pm.MethodId);
            }

            Type UserTypeEnum = eBuilder.CreateType();
        }

        private void CreateReadingEventTypeEnum()
        {
            EnumBuilder eBuilder = mBuilder.DefineEnum("ReadingEventType", TypeAttributes.Public, typeof(int));

            foreach (var re in gHelp.GetAllReadingEventTypes())
            {
                eBuilder.DefineLiteral(re.EventName.Replace(" ", ""), re.EventId);
            }

            Type UserTypeEnum = eBuilder.CreateType();
        }

        private void CreateTherapyTypeEnum()
        {
            EnumBuilder eBuilder = mBuilder.DefineEnum("TherapyType", TypeAttributes.Public, typeof(int));

            foreach (var tt in gHelp.GetAllTherapyTypes())
            {
                eBuilder.DefineLiteral(tt.TypeName.Replace(" ", ""), tt.TypeId);
            }

            Type UserTypeEnum = eBuilder.CreateType();
        }

        private void CreateUserTypeEnum()
        {
            EnumBuilder eBuilder = mBuilder.DefineEnum("UserType", TypeAttributes.Public, typeof(int));

            foreach (var ut in gHelp.GetAllUserTypes())
            {
                eBuilder.DefineLiteral(ut.TypeName.Replace(" ", ""), ut.TypeId);
            }

            Type UserTypeEnum = eBuilder.CreateType();
        }
    }
}
