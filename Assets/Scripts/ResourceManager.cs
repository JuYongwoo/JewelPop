using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager
{

    public TextAsset mapJSON;
    public GameObject blockParentObjectPrefab;
    public GameObject blockPrefab;
    public GameObject blockBackPrefab;
    public BlockTextures blockTextures;

    public void OnAwake()
    {
        mapJSON = Addressables.LoadAssetAsync<TextAsset>("MapJSON").WaitForCompletion();
        blockParentObjectPrefab = Addressables.LoadAssetAsync<GameObject>("BlockParentObjectPrefab").WaitForCompletion();
        blockPrefab = Addressables.LoadAssetAsync<GameObject>("BlockPrefab").WaitForCompletion();
        blockBackPrefab = Addressables.LoadAssetAsync<GameObject>("BlockBackPrefab").WaitForCompletion();
        blockTextures = Addressables.LoadAssetAsync<BlockTextures>("BlockTextures").WaitForCompletion();
    }

}
