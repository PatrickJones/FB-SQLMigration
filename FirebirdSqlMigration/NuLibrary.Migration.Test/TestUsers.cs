using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.Test
{
    public static class TestUsers
    {
        public static int TestInsuletSite = 21002;
        public static int TestCliniProSite = 218;
        public static int FakeSite = 12345;
        public static Guid UserA { get { return Guid.Parse("D1CE5A77-F77E-4812-BED6-B187DCA91386"); } }
        public static Guid UserB { get { return Guid.Parse("8D4C6229-4903-46F8-B0A4-B6C02500D5E3"); } }
        public static Guid UserC { get { return Guid.Parse("5E436C8A-9BE0-45B3-B6E5-741641DC73CD"); } }
        public static Guid UserFake { get { return Guid.Parse("1E436C8A-9BE0-45B3-B6E5-741641DC73CD");}}

        public static ICollection<Guid> TestUsersCollection = new List<Guid> { UserA, UserB, UserC, UserFake };

        public static Tuple<string, Guid> FakePatient = Tuple.Create<string, Guid>("117835.20155", Guid.Parse("7E5EB2EE-CBF1-40C6-BA3A-A567E3F101AB"));
        public static Tuple<string, Guid> TestPatient = Tuple.Create<string, Guid>("114505.31184", Guid.Parse("BC30CAAB-3726-49E4-A08F-00A0FB5FC5EE"));
    }
}
