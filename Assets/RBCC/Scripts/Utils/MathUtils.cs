using UnityEngine;

namespace RBCC.Scripts.Utils
{
    public class MathUtils
    {
        public static Quaternion ShortestRotation(Quaternion from, Quaternion to)
        {
            if (Quaternion.Dot(to, from) < 0f)
            {
                return to * Quaternion.Inverse(MultiplyQuaternion(from, -1f));
            }

            else return to * Quaternion.Inverse(from);
        }

        public static Quaternion MultiplyQuaternion(Quaternion input, float scalar)
        {
            return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
        }
    
        /// <summary>
        /// Return the multiple of 'to' closest to number
        /// </summary>
        /// <param name="number"> number to round </param>
        /// <param name="to"> nearest multiple </param>
        /// <returns></returns>
        public static float RoundFloatClosestToInt(float number, int to)
        {
            if (number == 0f)
            {
                return 0f;
            }

            float sign = Mathf.Sign(number);
            number = Mathf.Abs(number);
            int quotient = (int)(number / to);
            float left = number % 2;

            if (left > to - left)
            {
                return sign * (quotient + 1) * to;
            }
            else
            {
                return sign * quotient * to;
            }
        }

        /// <summary>
        /// Check that a is less than b and that the two numbers are different enough with the approx variable.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="approx"></param>
        /// <returns></returns>
        public static bool LessThanApprox(float a, float b, float approx = 0.01f)
        {
            if (a < b && (Mathf.Abs(a - b) > approx))
            {
                return true;
            }
        
            return false;
        }
    
        /// <summary>
        /// Check that a > b and that the two numbers are different enough with the approx variable.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="approx"></param>
        /// <returns></returns>
        public static bool MoreThanApprox(float a, float b, float approx = 0.01f)
        {
            if (a > b && (Mathf.Abs(a - b) > approx))
            {
                return true;
            }
        
            return false;
        }
    
        /// <summary>
        /// Check that a and b or not different than the approx.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="approx"></param>
        /// <returns></returns>
        public static bool EqualApprox(float a, float b, float approx = 0.01f)
        {
            if (Mathf.Abs(a - b) < approx)
            {
                return true;
            }
        
            return false;
        }
    }
}
