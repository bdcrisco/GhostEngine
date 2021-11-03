using System;

namespace GAImp
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjImporter importer = new ObjImporter();
            importer.LoadAsset("../../../ExampleAssets/Woman.obj");
            importer.ParseString();
        }
    }
}