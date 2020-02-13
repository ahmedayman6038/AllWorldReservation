using System;

namespace AllWorldReservation.BL.Utils
{
    public class IdUtils
    {
        public static string generateSampleId()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 10);
        }
    }
}
