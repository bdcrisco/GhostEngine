using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GAImp
{
    class Vertex4
    {
        public float X;
        public float Y;
        public float Z;
        public float W = 1.0f;

        public Vertex4(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
