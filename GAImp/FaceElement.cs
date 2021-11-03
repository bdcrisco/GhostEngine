
using System;
using System.Runtime.InteropServices;

namespace GAImp
{
    class FaceElement
    {
        public int V = 0;
        public int T = 0;
        public int N = 0;

        public void Display()
        {
            Console.Write("vertex: {0}, texture: {1}, normal: {2}", V, T, N);
        }
    }

}
