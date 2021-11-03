using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAImp
{
    /// <summary>
    /// A single mesh
    /// </summary>
    class Mesh
    {
        public string        Name;
        public List<Vertex4> Vertices;
        public List<Vertex3> TexCoords;
        public List<Vertex3> Normals;
        public List<Vertex3> ParameterVertices;
        public List<FaceGroup>    Groups;
        public List<LineElement>     Lines;

        public Mesh()
        {
            Name = "default";
            Vertices = new List<Vertex4>();
            TexCoords = new List<Vertex3>();
            Normals = new List<Vertex3>();
            ParameterVertices = new List<Vertex3>();
            Groups = new List<FaceGroup>();
            Lines = new List<LineElement>();
        }

        public Mesh(string name)
        {
            Name = name;
            Vertices = new List<Vertex4>();
            TexCoords = new List<Vertex3>();
            Normals = new List<Vertex3>();
            ParameterVertices = new List<Vertex3>();
            Groups = new List<FaceGroup>();
            Lines = new List<LineElement>();
        }
    }
}
