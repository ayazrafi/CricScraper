namespace BD.UtilityMethod
{
    public static class ExtensionMethod
    {
        public static double doubleTP(this string OrigVal)
        {

            double dblVal = 0;
            double.TryParse(OrigVal, out dblVal);
            return dblVal;
        }
        public static int intTP(this string OrigVal)
        {
            int dblVal = 0;
            int.TryParse(OrigVal, out dblVal);
            return dblVal;
        }
        public static DateTime dtTP(this string OrigVal)
        {
            DateTime dblVal = new DateTime();
            DateTime.TryParse(OrigVal, out dblVal);
            return dblVal;
        }
        public static long longTP(this string OrigVal)
        {
            long dblVal = 0;
            long.TryParse(OrigVal, out dblVal);
            return dblVal;
        }

        public static int GetStatus(this string OrigVal)
        {
            if (OrigVal.ToLower() == "pending")
            {
                return 2;
            }
            else if (OrigVal.ToLower() == "yes")
            {
                return 1;
            }
            else if (OrigVal.ToLower() == "no")
            {
                return 0;
            }
            return 2;
        }
    }
}
