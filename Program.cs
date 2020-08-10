namespace Godbert
{
    using Newtonsoft.Json;
    using SaintCoinach;
    using SaintCoinach.Graphics;
    using SaintCoinach.Graphics.Viewer;
    using SaintCoinach.Graphics.Viewer.Interop;
    using SaintCoinach.IO;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public struct PrimaryConfig
    {
        public string GameDirectory;
    }

    public struct ConfigJson
    {
        public string SkeletonPath;
        public List<string> AnimationPaths;
    }

    public class main
    {
        private static ARealmReversed Realm;
        public static void Main(string[] args)
        {
            HavokInterop.InitializeSTA();


            var cwd = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            System.IO.Directory.SetCurrentDirectory(cwd);

            if (args.Length == 0)
            {
                args = new string[1];
                args[0] = cwd + "\\..\\settings\\default.json";
            }

            var gameDirectory = System.IO.File.ReadAllText(cwd + "\\..\\game_directory.config");
            gameDirectory = gameDirectory.Trim();
            Console.WriteLine("Using Game Directory: " + gameDirectory);

            var configBlob = System.IO.File.ReadAllText(args[0]);
            var config = JsonConvert.DeserializeObject<ConfigJson>(configBlob);




            Realm = new ARealmReversed(gameDirectory, SaintCoinach.Ex.Language.English); 

            var paps = new List<PapFile>();
            foreach(var anim in config.AnimationPaths)
            {
                paps.Add(InitPap(anim));

            }
            var model_skel = new Skeleton(InitSklb(config.SkeletonPath));

            var body = "chara/equipment/e0000/model/c0101e0000_top.mdl";

            ModelFile file = (ModelFile)Realm.Packs.GetFile(body);
            Model model = new Model(file.GetModelDefinition(), ModelQuality.High);

            var configName = Path.GetFileNameWithoutExtension(args[0]);
            System.IO.Directory.CreateDirectory(cwd + "\\..\\results");

            Console.WriteLine(paps.Count + " PAP File(s).");
            int anims = 0;
            for(int i = 0; i < paps.Count; i++)
            {
                anims += paps[i].Animations.Length;
            }
            Console.WriteLine(anims + " Total Animation(s).");

            FbxExport.ExportFbx(cwd + "\\..\\results\\" + configName + ".fbx", model.Meshes, model_skel, paps);

        }
        private static SklbFile InitSklb(string path)
        {
            SaintCoinach.IO.File file = Realm.Packs.GetFile(path);
            return new SklbFile(file);
        }

        private static PapFile InitPap(string path)
        {
            SaintCoinach.IO.File file = Realm.Packs.GetFile(path);
            return new PapFile(file);
        }
    }
}

