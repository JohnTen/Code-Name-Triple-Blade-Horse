using System;
using UnityEngine;

namespace JTUtility
{
    public static class Maths
    {
        const double dErrorForCbrt = 0.0000001;

        const float halfDeg2Rad = 0.5f * Mathf.Deg2Rad;

        /// <summary>
        /// Included perfect cubic numbers from 0 to 127
        /// </summary>
        static readonly int[] perfectCubics =
            { 0, 1, 8, 27, 64, 125, 216, 343, 512, 729, 1000, 1331, 1728, 2197, 2744, 3375, 4096, 4913, 5832, 6859, 8000,
            9261, 10648, 12167, 13824, 15625, 17576, 19683, 21952, 24389, 27000, 29791, 32768, 35937, 39304, 42875, 46656,
            50653, 54872, 59319, 64000, 68921, 74088, 79507, 85184, 91125, 97336, 103823, 110592, 117649, 125000, 132651,
            140608, 148877, 157464, 166375, 175616, 185193, 195112, 205379, 216000, 226981, 238328, 250047, 262144, 274625,
            287496, 300763, 314432, 328509, 343000, 357911, 373248, 389017, 405224, 421875, 438976, 456533, 474552, 493039,
            512000, 531441, 551368, 571787, 592704, 614125, 636056, 658503, 681472, 704969, 729000, 753571, 778688, 804357,
            830584, 857375, 884736, 912673, 941192, 970299, 1000000, 1030301, 1061208, 1092727, 1124864, 1157625, 1191016,
            1225043, 1259712, 1295029, 1331000, 1367631, 1404928, 1442897, 1481544, 1520875, 1560896, 1601613, 1643032,
            1685159, 1728000, 1771561, 1815848, 1860867, 1906624, 1953125, 2000376, 2048383};

        /// <summary>
        /// Solve standard quadratic formula(no complex number roots)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="answers"></param>
        public static void SolveQuadraticEquation(double a, double b, double c, out double[] answers)
        {
            answers = new double[2];
            if (a == 0)
            {
                answers[0] = c / -b;
                answers[1] = double.NaN;
                return;
            }

            var discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
            {
                answers[0] = double.NaN;
                answers[1] = double.NaN;
                return;
            }

            discriminant = Math.Sqrt(discriminant);

            answers[0] = (-b + discriminant) / (2 * a);
            answers[1] = (-b - discriminant) / (2 * a);

            if (answers[0] == double.NaN)
            {
                answers[0] = answers[1];
                answers[1] = double.NaN;
            }
        }

        /// <summary>
        /// Solve standard quadratic formula(no complex number roots)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="answers"></param>
        public static void SolveQuadraticEquation(float a, float b, float c, out float[] answers)
        {
            answers = new float[2];

            if (a == 0)
            {
                answers[0] = c / -b;
                answers[1] = float.NaN;
                return;
            }

            var discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
            {
                answers[0] = float.NaN;
                answers[1] = float.NaN;
                return;
            }

            discriminant = Mathf.Sqrt(discriminant);

            answers[0] = (-b + discriminant) / (2 * a);
            answers[1] = (-b - discriminant) / (2 * a);

            if (answers[0] == float.NaN)
            {
                answers[0] = answers[1];
                answers[1] = float.NaN;
            }
        }

        /// <summary>
        /// Solve standard cubic equation(no complex number roots)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="answers"></param>
        /// <remarks>
        /// (NATURAL SCIENCE JOURNAL OF HAINAN TEACHERES COLLEGE,Hainan Province,China. Vol. 2,No. 2；Dec，1989)，A new extracting formula and a new distinguishing means on the one variable cubic equation.， Fan Shengjin. PP·91—98 .
        /// </remarks>
        public static void SolveCubicEquation(double a, double b, double c, double d, out double[] answers)
        {
            if (a == 0)
            {
                SolveQuadraticEquation(b, c, d, out answers);
                return;
            }

            answers = new double[3];

            var A = b * b - 3 * a * c;
            var B = b * c - 9 * a * d;
            var C = c * c - 3 * b * d;
            var D = B * B - 4 * A * C;

            if (A == 0 && B == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    answers[i] = -c / b;
                }
                return;
            }

            if (D > 0)
            {
                var Ab = A * b;
                var a3 = 3 * a;
                var a3d2 = a3 * 0.5;
                var sqrtD = Math.Sqrt(D);

                var Y1 = Ab + a3d2 * (-B + sqrtD);
                var Y2 = Ab + a3d2 * (-B - sqrtD);

                answers[0] = (-b - (Cbrt1(Y1) + Cbrt1(Y2))) / a3;
                answers[1] = double.NaN;
                answers[2] = double.NaN;
                return;
            }

            if (D == 0)
            {
                var K = B / A;
                answers[0] = -b / a + K;
                answers[1] = -K * 0.5;
                answers[2] = answers[1];
                return;
            }

            if (D < 0)
            {
                var sqrtA = Math.Sqrt(A);
                var sqrt3 = Math.Sqrt(3);
                var theta = Math.Acos((2 * A * b - 3 * a * B) / (2 * Math.Sqrt(A * A * A)));
                var cosThetad3 = Math.Cos(theta / 3);
                var sinThetad3TSqrt3 = Math.Sin(theta / 3) * Math.Sqrt(3);
                var a3 = 3 * a;

                answers[0] = (-b - 2 * sqrtA * cosThetad3) / a3;
                answers[1] = (-b + sqrtA * (cosThetad3 + sinThetad3TSqrt3)) / a3;
                answers[2] = (-b + sqrtA * (cosThetad3 - sinThetad3TSqrt3)) / a3;
                return;
            }
        }

        /// <summary>
        /// Solve standard cubic equation(no complex number roots)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="answers"></param>
        /// <remarks>
        /// (NATURAL SCIENCE JOURNAL OF HAINAN TEACHERES COLLEGE,Hainan Province,China. Vol. 2,No. 2；Dec，1989)，A new extracting formula and a new distinguishing means on the one variable cubic equation.， Fan Shengjin. PP·91—98 .
        /// </remarks>
        public static void SolveCubicEquation(float a, float b, float c, float d, out float[] answers)
        {
            if (a == 0)
            {
                SolveQuadraticEquation(b, c, d, out answers);
                return;
            }

            answers = new float[3];

            var A = b * b - 3 * a * c;
            var B = b * c - 9 * a * d;
            var C = c * c - 3 * b * d;
            var D = B * B - 4 * A * C;

            if (A == 0 && B == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    answers[i] = -c / b;
                }
                return;
            }

            if (D > 0)
            {
                var Ab = A * b;
                var a3 = 3 * a;
                var a3d2 = a3 * 0.5f;
                var sqrtD = Mathf.Sqrt(D);

                var Y1 = Ab + a3d2 * (-B + sqrtD);
                var Y2 = Ab + a3d2 * (-B - sqrtD);

                answers[0] = (-b - ((float)Cbrt1(Y1) + (float)Cbrt1(Y2))) / a3;
                answers[1] = float.NaN;
                answers[2] = float.NaN;
                return;
            }

            if (D == 0)
            {
                var K = B / A;
                answers[0] = -b / a + K;
                answers[1] = -K * 0.5f;
                answers[2] = answers[1];
                return;
            }

            if (D < 0)
            {
                var sqrtA = Mathf.Sqrt(A);
                var sqrt3 = Mathf.Sqrt(3);
                var theta = Mathf.Acos((2 * A * b - 3 * a * B) / (2 * Mathf.Sqrt(A * A * A)));
                var cosThetad3 = Mathf.Cos(theta / 3);
                var sinThetad3TSqrt3 = Mathf.Sin(theta / 3) * Mathf.Sqrt(3);
                var a3 = 3 * a;

                answers[0] = (-b - 2 * sqrtA * cosThetad3) / a3;
                answers[1] = (-b + sqrtA * (cosThetad3 + sinThetad3TSqrt3)) / a3;
                answers[2] = (-b + sqrtA * (cosThetad3 - sinThetad3TSqrt3)) / a3;
                return;
            }
        }

        /// <summary>
        /// Find the cubic root of an number, a tiny bit faster than Cbrt2
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        /// <remarks>
        /// Halley's method
        /// https://en.wikipedia.org/wiki/Cube_root
        /// </remarks>
        public static double Cbrt1(double d)
        {
            bool sign = d > 0;
            d *= sign ? 1 : -1;

            int head = 0;
            int tail = perfectCubics.Length - 1;
            int mid = tail / 2;

            // Find a initial approximation
            while (tail - head > 1)
            {
                if (perfectCubics[mid] < d)
                {
                    head = mid;
                }
                else
                {
                    tail = mid;
                }
                mid = (tail - head) / 2 + head;
            }

            if (perfectCubics[head] == d) return sign ? head : -head;
            if (perfectCubics[tail] == d) return sign ? tail : -tail;

            double approx = (d - perfectCubics[head]) / (perfectCubics[tail] - perfectCubics[head]) + head;
            double last = -1;
            double approx3;
            while (Math.Abs(approx - last) > dErrorForCbrt)
            {
                last = approx;
                approx3 = approx * approx * approx;
                approx = approx * (approx3 + 2 * d) / (2 * approx3 + d);
            }

            return sign ? approx : -approx;
        }

        /// <summary>
        /// Find the cubic root of an number, a tiny bit more accurate than Cbrt1
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        /// <remarks>
        /// Newton's method
        /// https://en.wikipedia.org/wiki/Cube_root
        /// </remarks>
        public static double Cbrt2(double d)
        {
            bool sign = d > 0;
            d *= sign ? 1 : -1;

            int head = 0;
            int tail = perfectCubics.Length - 1;
            int mid = tail / 2;

            // Find a initial approximation
            while (tail - head > 1)
            {
                if (perfectCubics[mid] < d)
                {
                    head = mid;
                }
                else
                {
                    tail = mid;
                }
                mid = (tail - head) / 2 + head;
            }

            if (perfectCubics[head] == d) return sign ? head : -head;
            if (perfectCubics[tail] == d) return sign ? tail : -tail;

            double approx = (d - perfectCubics[head]) / (perfectCubics[tail] - perfectCubics[head]) + head;
            double last = -1;
            while (Math.Abs(approx - last) > dErrorForCbrt)
            {
                last = approx;
                approx = (d / (approx * approx) + 2 * approx) / 3;
            }

            return sign ? approx : -approx;
        }
    }
}
