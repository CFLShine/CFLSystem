

using System;

namespace MSTD
{
    public static class DoubleHelper
    {
        /// <summary>
        /// Retourne true si d1 plus grand que d2.
        /// double.NaN est considéré comme 0.
        /// La précision par défaut 1E-15 est la précision d'un double, 15 décimales.
        /// </summary>
        public static bool Bigger(double d1, double d2, double precision = 1E-15)
        {
            if(double.IsNaN(d1))
                d1 = 0;
            if(double.IsNaN(d2))
                d2 = 0;
            
            if(double.IsPositiveInfinity(d1))
                return !double.IsPositiveInfinity(d2);

            if(double.IsPositiveInfinity(d2))
                return false;

            if(double.IsNegativeInfinity(d1))
                return false;

            if(double.IsNegativeInfinity(d2))
                return !double.IsNegativeInfinity(d1);  
            
            return GT(d1, d2, new Epsilon(precision));
        }
            
        /// <summary>
        /// Retourne true si d1 plus petit que d2.
        /// double.NaN est considéré comme 0.
        /// La précision par défaut 1E-15 est la précision d'un double, 15 décimales.
        /// </summary>
        public static bool Smaller(double d1, double d2, double precision = 1E-15)
        {
            if(double.IsNaN(d1))
                d1 = 0;
            if(double.IsNaN(d2))
                d2 = 0;
            
            if(double.IsPositiveInfinity(d1))
                return false;

            if(double.IsPositiveInfinity(d2))
                return !double.IsPositiveInfinity(d1);

            if(double.IsNegativeInfinity(d1))
                return !double.IsPositiveInfinity(d2);

            if(double.IsNegativeInfinity(d2))
                return false;  
            
            return LT(d1, d2, new Epsilon(precision));
        }

        /// <summary>
        /// Retourne true si d1 et d2 sont égaux.
        /// double.NaN est considéré comme 0.
        /// La précision par défaut 1E-15 est la précision d'un double, 15 décimales.
        /// </summary>
        public static bool Equal(double d1, double d2, double precision = 1E-15)
        {
            if(double.IsNaN(d1))
                d1 = 0;
            if(double.IsNaN(d2))
                d2 = 0;
            
            if(double.IsPositiveInfinity(d1))
                return double.IsPositiveInfinity(d2);

            if(double.IsPositiveInfinity(d2))
                return double.IsPositiveInfinity(d1);

            if(double.IsNegativeInfinity(d1))
                return double.IsPositiveInfinity(d2);

            if(double.IsNegativeInfinity(d2))
                return double.IsNegativeInfinity(d1);  
            
            return EQ(d1, d2, new Epsilon(precision));
        }

        /// <summary>
        /// Retourne true si d1 plus grand ou égal que d2.
        /// double.NaN est considéré comme 0.
        /// La précision par défaut 1E-15 est la précision d'un double, 15 décimales.
        /// </summary>
        public static bool BiggerEqual(double d1, double d2, double precision = 1E-15)
        {
            return Equal(d1, d2, precision) || Bigger(d1, d2, precision);
        }

        /// <summary>
        /// Retourne true si d1 plus petit ou égal que d2.
        /// double.NaN est considéré comme 0.
        /// La précision par défaut 1E-15 est la précision d'un double, 15 décimales.
        /// </summary>
        public static bool SmallerEqual(double d1, double d2, double precision = 1E-15)
        {
            return Equal(d1, d2, precision) || Smaller(d1, d2, precision);
        }

        #region private

        private struct Epsilon
        {
            public Epsilon(double value = 1E-3) { _value = value; }
            private double _value;
            internal bool IsEqual   (double a, double b) { return (a == b) ||  (Math.Abs(a - b) < _value); }
            internal bool IsNotEqual(double a, double b) { return (a != b) && !(Math.Abs(a - b) < _value); }
        }
        private static bool EQ(this double a, double b, Epsilon e) { return e.IsEqual   (a, b); }
        private static bool LT(this double a, double b, Epsilon e) { return e.IsNotEqual(a, b) && (a < b); }
        private static bool GT(this double a, double b, Epsilon e) { return e.IsNotEqual(a, b) && (a > b); }

        #endregion private

    }
}
