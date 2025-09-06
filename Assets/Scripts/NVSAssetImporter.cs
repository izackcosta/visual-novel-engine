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
        streamReader.Close();
        for (int i = 0; i < lines.Length; i++)
        {
            var trimmedLine = lines[i].Trim();
            lines[i] = trimmedLine;
        }
        asset.Instructions.AddRange(lines);
        ctx.AddObjectToAsset("main obj", asset);
        ctx.SetMainObject(asset);
        //AssetDatabase.SaveAssets();
    }

}
