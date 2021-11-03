
using System.Collections.Generic;

namespace GAImp
{
    class FaceGroup
    {
        public string Name;
        public string MaterialID;
        public string SmoothShading;
        public List<Face> Faces;

        public FaceGroup()
        {
            Name = "default";
            MaterialID = "default";
            SmoothShading = "off";
            Faces = new List<Face>();
        }

        public FaceGroup(string name)
        {
            Name = name;
            Faces = new List<Face>();
        }

        public FaceGroup(string name, string id, string ss)
        {
            Name = name;
            MaterialID = id;
            SmoothShading = ss;
            Faces = new List<Face>();
        }
    }
}
