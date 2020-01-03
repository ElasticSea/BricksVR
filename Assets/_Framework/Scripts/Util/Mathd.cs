namespace _Framework.Scripts.Util
{
    public struct Mathd
    {
        /// <summary>
        ///   <para>Degrees-to-radians conversion constant (Read Only).</para>
        /// </summary>
        public const double Deg2Rad = 0.017453292519943295769236907684886127134428718885417254560;
        /// <summary>
        ///   <para>Radians-to-degrees conversion constant (Read Only).</para>
        /// </summary>
        public const double Rad2Deg = 57.29577951308232087679815481410517033240547246656432154916;
        
        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * Clamp01(t);
        }

        public static double Clamp01(double value)
        {
            if (value < 0.0) return 0.0;
            if (value > 1.0) return 1;
            return value;
        }
    }
}