using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Enums
{
    public class EnumCollection
    {

        /// <summary>
        /// The Function To Get The Description Of Any Value In Enum Based On The Key You Send
        /// </summary>
        /// <param name="value">The Key Like 1 or 2 from the Constant Integer Came From Enum</param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public enum ClientType
        {
            [Description("شركة")]
            Company = 1,

            [Description("فرد")]
            Personal = 2
        }

        public enum MailType
        {
            [Description("مرسل")]
            Sender = 1,

            [Description("متلقى")]
            Receiver = 2
        }
    }
}
