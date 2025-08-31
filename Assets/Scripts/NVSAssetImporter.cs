using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;
using UnityEngine;

[ScriptedImporter(1, "vns", AllowCaching =true)]
public class NVSAssetImporter : ScriptedImporter
{

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var streamReader = new StreamReader(ctx.assetPath);
        var fileName = Path.GetFileName(ctx.assetPath);
        var asset = ScriptableObject.CreateInstance<VNSAsset>();
        asset.name = fileName;
        asset.Instructions = streamReader.ReadToEnd().Split('\n');
        streamReader.Close();
        ctx.AddObjectToAsset("main obj", asset);
        ctx.SetMainObject(asset);
        //AssetDatabase.SaveAssets();
    }

}
