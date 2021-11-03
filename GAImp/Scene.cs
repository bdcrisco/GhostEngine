using System;
using System.Collections.Generic;

namespace GAImp
{
    class Scene
    {
        public List<string>      Materials;
        public List<Mesh>        Meshes;
        public List<FaceGroup>   Groups;
        public List<LineElement> Lines;

        public Scene()
        {
            Materials = new List<string>();
            Meshes = new List<Mesh>();
            Groups = new List<FaceGroup>();
            Lines = new List<LineElement>();
        }
    }
}
