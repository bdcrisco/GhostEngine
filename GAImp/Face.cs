using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAImp
{
    class Face
    {
        public List<FaceElement> Elements;

        public Face()
        {
            Elements = new List<FaceElement>();
        }

        public void Display()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                Console.Write("Element {0}", i);
                Elements[i].Display();
                Console.Write("\n");
            }
        }
    }
}
