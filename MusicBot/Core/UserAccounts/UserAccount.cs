using System;
using System.Collections.Generic;
using System.Text;

namespace MusicBot.Core.UserAccounts
{
    public class UserAccount
    {
        public ulong ID { get; set; }
        public ulong RoleID { get; set; }
        public uint XP { get; set; }
        public uint Reputation { get; set; }

        public uint Level
        {
            get
            {
                return (uint)Math.Sqrt(XP / 50);
            }
        }
    }
}
