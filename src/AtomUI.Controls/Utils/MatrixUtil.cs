using Avalonia;

namespace AtomUI.Controls.Utils
{
    public static class MatrixUtil
    {
        public static void ScaleAt(ref this Matrix matrix, double scaleX, double scaleY, double centerX, double centerY)
        {
            matrix *= CreateScaling(scaleX, scaleY, centerX, centerY);
        }
        
        public static void Translate(ref this Matrix matrix, double offsetX, double offsetY)
        {
            matrix = new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.M31 + offsetX, matrix.M32 + offsetY);
        }

        public static void RotateAt(ref this Matrix matrix, double angle, double centerX, double centerY)
        {
            matrix *= CreateRotationRadians(angle, centerX, centerY);
        }
        
        

        public static Matrix CreateRotationRadians(double angle, double centerX, double centerY)
        {
            var m12 = Math.Sin(angle);
            var num = Math.Cos(angle);
            var offsetX = centerX * (1.0 - num) + centerY * m12;
            var offsetY = centerY * (1.0 - num) - centerX * m12;

            return new Matrix(num, m12, -m12, num, offsetX, offsetY);
        }

        private static Matrix CreateScaling(double scaleX, double scaleY, double centerX, double centerY)
        {
            return new Matrix(scaleX, 0.0, 0.0, scaleY, centerX - scaleX * centerX, centerY - scaleY * centerY);
        }

        private static Matrix CreateScaling(double scaleX, double scaleY)
        {
            return new Matrix(scaleX, 0.0, 0.0, scaleY, 0.0, 0.0);
        }

        private static Matrix CreateSkewRadians(double skewX, double skewY)
        {
            return new Matrix(1.0, Math.Tan(skewY), Math.Tan(skewX), 1.0, 0.0, 0.0);
        }

        private static Matrix CreateTranslation(double offsetX, double offsetY)
        {
            return new Matrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
        }

        private static Matrix CreateIdentity()
        {
            return new Matrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
        }
    }
}
