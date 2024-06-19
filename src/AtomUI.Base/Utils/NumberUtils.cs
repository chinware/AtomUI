namespace AtomUI.Utils;

public static class NumberUtils
{
   public static int Clamp(int value, int min, int max)
   {
      return (value < min) ? min : (value > max) ? max : value;
   }

   public static float Clamp(float value, float min, float max)
   {
      return (value < min) ? min : (value > max) ? max : value;
   }
   
   public static double Clamp(double value, double min, double max)
   {
      return (value < min) ? min : (value > max) ? max : value;
   }
   
   public static decimal Clamp(decimal value, decimal min, decimal max)
   {
      return (value < min) ? min : (value > max) ? max : value;
   }

   public static double RoundToFixedPoint(double value, int fixedCount)
   {
      var factor = Math.Pow(10, fixedCount);
      return Math.Round(value * factor) / factor;
   }

   public static bool FuzzyCompare(double p1, double p2)
   {
      return Math.Abs(p1 - p2) * 1000000000000d <= Math.Min(Math.Abs(p1), Math.Abs(p2));
   }
   
   public static bool FuzzyCompare(float p1, float p2)
   {
      return Math.Abs(p1 - p2) * 100000f <= Math.Min(Math.Abs(p1), Math.Abs(p2));
   }

   public static bool FuzzyIsNull(double d)
   {
      return Math.Abs(d) <= 0.000000000001d;
   }

   public static bool FuzzyIsNull(float d)
   {
      return Math.Abs(d) <= 0.00001f;
   }
}