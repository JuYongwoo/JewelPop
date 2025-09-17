using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager
{

    public TextAsset mapJSON;
    public GameObject blockPrefab;
    public BlockTextures blockTextures;

    public void OnAwake()
    {
        mapJSON = Addressables.LoadAssetAsync<TextAsset>("MapJSON").WaitForCompletion();
        blockPrefab = Addressables.LoadAssetAsync<GameObject>("BlockPrefab").WaitForCompletion();
        blockTextures = Addressables.LoadAssetAsync<BlockTextures>("BlockTextures").WaitForCompletion();
    }

}
