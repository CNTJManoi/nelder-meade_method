using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nelder_meade_method.Models
{
    class Vector
    {
        private float _x;
        private float _y;
        public Vector(float x, float y)
        {
            _x = x;
            _y = y;
        }
        public static Vector operator +(Vector vector1, Vector vector2)
        {
            return new Vector (vector1.X + vector2.X, vector1.Y + vector2.Y);
        }
        public static Vector operator -(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.X - vector2.X, vector1.Y - vector2.Y);
        }
        public static Vector operator *(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.X * vector2.X, vector1.Y * vector2.Y);
        }
        public static Vector operator *(Vector vector1, float num)
        {
            return new Vector(vector1.X * num, vector1.Y * num);
        }
        public static Vector operator /(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.X / vector2.X, vector1.Y / vector2.Y);
        }
        public static Vector operator /(Vector vector1, int num)
        {
            return new Vector(vector1.X / num, vector1.Y / num);
        }
        public float X { get { return _x; } }
        public float Y { get { return _y; } }
    }
}
