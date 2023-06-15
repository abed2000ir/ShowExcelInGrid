namespace GeoLocation.Utilities
{
    public class Convertors
    {

        public static string ByteToKByte(float fbyte)
        {

            if (fbyte < 0)
            {

                return "0";
            }
            else if (fbyte < 1024)
            {

                return fbyte.ToString();
            }
            else if (fbyte > 1024)
            {

                return (fbyte / 1024).ToString();
            }

            return "";
        }

        public static string KbyteToMeg(float KByte)
        {
            if (KByte < 0)
            {
                return "0";
            }
            else if (KByte < 1024 * 1024)
            {
                return KByte.ToString()+" Kb";
            }
            else if (KByte > 1024 * 1024)
            {
                return (KByte / (1024 * 1024)).ToString() + " Mb";
            }

            return "";
        }

        public static string MegToGig(float MByte)
        {
            if (MByte < 0)
            {
                return "0";
            }
            else if (MByte < 1024 * 1024 * 1024)
            {
                return MByte.ToString()+" MB";
            }
            else if (MByte > 1024 * 1024)
            {
                return (MByte / (1024 * 1024)).ToString()+" Gb";
            }

            return "";
        }

    }
}
