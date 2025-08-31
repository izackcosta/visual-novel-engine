using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;
using UnityEngine;
using System.Linq;

[ScriptedImporter(1, "vns", AllowCaching =true)]
public class NVSAssetImporter : ScriptedImporter
{

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var streamReader = new StreamReader(ctx.assetPath);
        var fileName = Path.GetFileName(ctx.assetPath);
        var asset = ScriptableObject.CreateInstance<VNSAsset>();
        asset.name = fileName;
        var lines = streamReader.ReadToEnd().Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            var trimmedLine = lines[i].Trim();
            lines[i] = trimmedLine;
        }
        var instructions = new string[lines.Length][];
        for (int i = 0; i < lines.Length; i++)
        {
            instructions[i] = lines[i].Split(' ').Where(word => word != "").ToArray();
            foreach (var s in instructions[i])
            {
                Debug.Log(s);
            }
        }
        asset.Instructions = instructions;
        streamReader.Close();
        ctx.AddObjectToAsset("main obj", asset);
        ctx.SetMainObject(asset);
        //AssetDatabase.SaveAssets();
    }

}
