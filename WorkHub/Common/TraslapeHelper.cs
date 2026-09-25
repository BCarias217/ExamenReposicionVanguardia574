namespace WorkHub.Common
{
    public static class TraslapeHelper
    {
        public static bool HayTraslape(DateTime inicio1, int duracionHoras1, DateTime inicio2, int duracionHoras2)
        {
            var fin1 = inicio1.AddHours(duracionHoras1);
            var fin2 = inicio2.AddHours(duracionHoras2);
            return inicio1 < fin2 && inicio2 < fin1;
        }
    }
}