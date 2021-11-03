using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace GAImp
{
    class ObjImporter : Importer
    {

#if DEBUG
        private List<Tuple<string, string, int>> locations;
        private int numMeshes = 0;
        private int numVertices = 0;
        private int numTextures = 0;
        private int numNormals = 0;
        private int numPVertices = 0;
        private int numFaces = 0;
        private int numGroups = 0;
        private int numLineElements = 0;
        private int numMaterialFiles = 0;
        private int numMaterialDefinitions = 0;
#endif

        private string[] _assetString;
        private Scene _scene;

        public ObjImporter()
        {
            _scene = new Scene();
        }
        /// <summary>
        /// Load Asset
        /// </summary>
        /// <param name="dir">The asset file location</param>
        public void LoadAsset(string dir)
        {
            _assetString = File.ReadAllLines(dir);
            
#if DEBUG
            PreRead();
#endif
        }

        public void ParseString()
        {
            Stopwatch stopWatch = new Stopwatch();
#if DEBUG
            stopWatch.Start();
#endif
            for (int i = 0; i < _assetString.Length; i++)
            {
                string[] line = _assetString[i].Split(" ");
                if (line[0] == "mtllib")
                {
                    _scene.Materials.Add(line[1]);
                }
                if (line[0] == "o")
                {
                    Mesh mesh = CreateMesh(ref i);
                    _scene.Meshes.Add(mesh);
                }
            }

            /*List<Face> face = _scene.Meshes[6].Groups[1].Faces;
            int last = face.Count - 1;
            face[last].Display();*/
#if DEBUG
            stopWatch.Stop();
            Console.WriteLine("Runtime:{0}, TotalLines: {1}\n", stopWatch.Elapsed, _assetString.Length);
            Debugger();
#endif
        }

        private Mesh CreateMesh(ref int index)
        {
            string[] line = _assetString[index].Split(" ");
            Mesh mesh = new Mesh(line[1]);
            index++;

            CreateVertices(ref index, ref mesh);
            CreateTextures(ref index, ref mesh);
            CreateNormals(ref index, ref mesh);
            CreatePVertices(ref index, ref mesh);

            /*line = _assetString[index].Split(" ");
            index++;
            while (line[0] == "g")
            {
                FaceGroup group = CreateGroup(ref index, ref mesh);
                mesh.Groups.Add(group);
                line = _assetString[index].Split(" ");
            }
            Console.WriteLine(_assetString[index]);
            CreateLines(ref index, ref mesh);*/

            return mesh;
        }

        private void CreateVertices(ref int index, ref Mesh mesh)
        {
            string[] line = _assetString[index].Split(" ");
            
            while (line[0] == "v")
            {
                //Console.WriteLine("Vertex: {0}, count: {1}", line[1], numVertices + 1);
                Vertex4 v4 = new Vertex4((float)Convert.ToDouble(line[1]), (float)Convert.ToDouble(line[2]), (float)Convert.ToDouble(line[3]));
                if (line.Length == 5)
                {
                    v4.W = (float)Convert.ToDouble(line[4]);
                }
                mesh.Vertices.Add(v4);

                index++;
                line = _assetString[index].Split(" ");
            }
        }

        private void CreateTextures(ref int index, ref Mesh mesh)
        {
            string[] line = _assetString[index].Split(" ");

            while (line[0] == "vt")
            {
                //Console.WriteLine("Texture: {0}, count: {1}", line[1], numTextures + 1);
                Vertex3 v3 = new Vertex3((float)Convert.ToDouble(line[1]));
                if (line.Length == 3)
                {
                    v3.Y = (float)Convert.ToDouble(line[2]);
                }

                if (line.Length == 4)
                {
                    v3.Z = (float)Convert.ToDouble(line[3]);
                }
                mesh.TexCoords.Add(v3);

                index++;
                line = _assetString[index].Split(" ");
            }
        }

        private void CreateNormals(ref int index, ref Mesh mesh)
        {
            string[] line = _assetString[index].Split(" ");

            while (line[0] == "vn")
            {
                //Console.WriteLine("Normal: {0}, count: {1}", line[1], numNormals + 1);
                Vertex3 v3 = new Vertex3((float)Convert.ToDouble(line[1]),
                    (float)Convert.ToDouble(line[2]), (float)Convert.ToDouble(line[3]));
                mesh.Normals.Add(v3);

                index++;
                line = _assetString[index].Split(" ");
            }
        }

        private void CreatePVertices(ref int index, ref Mesh mesh)
        {
            string[] line = _assetString[index].Split(" ");

            while (line[0] == "vp")
            {
                Vertex3 v3 = new Vertex3((float)Convert.ToDouble(line[1]));
                if (line.Length == 3)
                {
                    v3.Y = (float)Convert.ToDouble(line[2]);
                }

                if (line.Length == 4)
                {
                    v3.Z = (float)Convert.ToDouble(line[3]);
                }
                mesh.ParameterVertices.Add(v3);

                index++;
                line = _assetString[index].Split(" ");
            }
        }

        private FaceGroup CreateGroup(ref int index, ref Mesh mesh)
        {
            string[] line = _assetString[index].Split(" ");
            FaceGroup group = new FaceGroup(line[1]);

            if (line[0] == "usemtl")
            {
                group.MaterialID = line[1];
                index++;
                line = _assetString[index].Split(" ");
            }
            if (line[0] == "s")
            {
                group.SmoothShading = line[1];
                index++;
                line = _assetString[index].Split(" ");
            }
            while (line[0] == "f")
            {
                Face face = CreateElements(ref index);
                group.Faces.Add(face);
                line = _assetString[index].Split(" ");
            }

            return group;
        }

        private Face CreateElements(ref int index)
        {
            string[] line = _assetString[index].Split(" ");
            index++;
            Face face = new Face();

            foreach (string faceline in line.Skip(1))
            {
                FaceElement fe = new FaceElement();
                string[] vertices = faceline.Split("/");

                // vertex value for v1, v1/vt1, v1/vt1/vn1, v1//vn1
                fe.V = Convert.ToInt32(vertices[0]);

                // v1/vt1
                if (vertices.Length == 2)
                {
                    // texture value for v1/vt1
                    fe.T = Convert.ToInt32(vertices[1]);
                }

                // v1/vt1/vn1, v1//vn1
                if (vertices.Length == 3)
                {
                    // texture value for v1/vt1/vn1
                    if (vertices[1] != "")
                    {
                        fe.T = Convert.ToInt32(vertices[1]);
                    }
                    // normal value for v1/vt1/vn1 and v1//vn1
                    fe.N = Convert.ToInt32(vertices[2]);
                }

                face.Elements.Add(fe);
            }

            return face;
        }

        private void CreateLines(ref int index, ref Mesh mesh)
        {
            string[] line = _assetString[index].Split(" ");
            
            while (line[0] == "l")
            {
                LineElement le = new LineElement();
                foreach (string word in line.Skip(1))
                {
                    le.Indices.Add(Convert.ToInt32(word));
                }
                mesh.Lines.Add(le);

                index++;
                line = _assetString[index].Split(" ");
            }
        }

#if DEBUG
        private void PreRead()
        {
            string flag = "";
            locations = new List<Tuple<string, string, int>>();
            for (int i = 0; i < _assetString.Length; i++)
            {
                string[] line = _assetString[i].Split(" ");
                switch (line[0])
                {
                    case "mtllib":
                        numMaterialFiles++;
                        if (flag != "mttlib")
                            locations.Add(new Tuple<string, string, int>(line[0], line[1], i));
                        flag = "mttlib";
                        break;
                    case "o":
                        numMeshes++;
                        if (flag != "o")
                            locations.Add(new Tuple<string, string, int>(line[0], line[1], i));
                        flag = "o";
                        break;
                    case "v":
                        numVertices++;
                        if (flag != "v")
                            locations.Add(new Tuple<string, string, int>(line[0], line[0], i));
                        flag = "v";
                        break;
                    case "vt":
                        numTextures++;
                        if (flag != "vt")
                            locations.Add(new Tuple<string, string, int>(line[0], line[0], i));
                        flag = "vt";
                        break;
                    case "vn":
                        numNormals++;
                        if (flag != "vn")
                            locations.Add(new Tuple<string, string, int>(line[0], line[0], i));
                        flag = "vn";
                        break;
                    case "vp":
                        numPVertices++;
                        if (flag != "vp")
                            locations.Add(new Tuple<string, string, int>(line[0], line[0], i));
                        flag = "vp";
                        break;
                    case "g":
                        numGroups++;
                        if (flag != "g")
                            locations.Add(new Tuple<string, string, int>(line[0], line[1], i));
                        flag = "g";
                        break;
                    case "usemtl":
                        numMaterialDefinitions++;
                        if (flag != "usemtl")
                            locations.Add(new Tuple<string, string, int>(line[0], line[1], i));
                        flag = "usemtl";
                        break;
                    case "s":
                        if (flag != "s")
                            locations.Add(new Tuple<string, string, int>(line[0], line[1], i));
                        flag = "s";
                        break;
                    case "f":
                        numFaces++;
                        if (flag != "f")
                            locations.Add(new Tuple<string, string, int>(line[0], line[0], i));
                        flag = "f";
                        break;
                }
            }

            foreach (Tuple<string, string, int> item in locations)
            {
                Console.WriteLine(item);
            }
        }

        private void PreRead(bool loc)
        {
            string flag = "";
            for (int i = 0; i < _assetString.Length; i++)
            {
                string[] line = _assetString[i].Split(" ");
                if (line[0] == "v")
                {
                    if (flag != "v")
                    {
                        locations.Add(new Tuple<string, string, int>(line[0], line[0], i));
                    }
                    numVertices++;
                    flag = "v";
                }
            }
        }

        private void Debugger()
        {
            int vertices = 0;
            int textures = 0;
            int normals = 0;
            int pVertices = 0;
            int fElements = 0;
            int lElements = 0;
            int groups = 0;
            for (int i = 0; i < _scene.Meshes.Count; i++)
            {
                vertices += _scene.Meshes[i].Vertices.Count;
                textures += _scene.Meshes[i].TexCoords.Count;
                normals += _scene.Meshes[i].Normals.Count;
                pVertices += _scene.Meshes[i].ParameterVertices.Count;
                groups += _scene.Meshes[i].Groups.Count;
                foreach (FaceGroup group in _scene.Meshes[i].Groups)
                {
                    fElements += group.Faces.Count;
                }
                lElements += _scene.Meshes[i].Lines.Count;
            }
            Console.WriteLine("Meshes: {0} / {1}, Vertices: {2} / {3}", _scene.Meshes.Count, numMeshes, vertices, numVertices);
            Console.WriteLine("Textures: {0} / {1}, Normals: {2} / {3}", textures, numTextures, normals, numNormals);
            Console.WriteLine("pVertices: {0} / {1}, lElements: {2} / {3}", pVertices, numPVertices, lElements, numLineElements);
            Console.WriteLine("fElements: {0} / {1}, groups: {2} / {3}", fElements, numFaces, groups, numGroups);
            Console.WriteLine("\nMaterials: {0}, Definitions: {1}", numMaterialFiles, numMaterialDefinitions);
            foreach (string mat in _scene.Materials)
            {
                Console.WriteLine(" - {0}", mat);
            }
            foreach (Mesh mesh in _scene.Meshes)
            {
                Console.WriteLine("   - {0}", mesh.Name);
                foreach (FaceGroup fg in mesh.Groups)
                {
                    Console.WriteLine("     - {0}", fg.MaterialID);
                }
            }
        }
#endif
    }
}
