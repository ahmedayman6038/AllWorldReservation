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

        public enum PhotoType
        {
            [Description("Hotel")]
            Hotel = 1,

            [Description("Tour")]
            Tour = 2,

            [Description("Post")]
            Post = 3
        }

        public enum MailType
        {
            [Description("Sender")]
            Sender = 1,

            [Description("Reciver")]
            Receiver = 2
        }

        public enum PayType
        {
            [Description("Online")]
            Online = 1,

            [Description("Offline")]
            Offline = 2
        }

        public enum ReservationType
        {
            [Description("Hotel")]
            Hotel = 1,

            [Description("Sun Hotel")]
            SunHotel = 2,

           [Description("Tour")]
            Tour = 3
        }

        public enum GuestType
        {
            [Description("Adult")]
            Adult = 1,

            [Description("Teenager")]
            Teenager = 2,

            [Description("Children")]
            Children = 3,

            [Description("Infant")]
            Infant = 4
        }
    }
}
