using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager
{

    public TextAsset mapJSON;
    public GameObject blockParentObjectPrefab;
    public Dictionary<string, GameObject> blockPrefabs;

    public void OnAwake()
    {
        mapJSON = Addressables.LoadAssetAsync<TextAsset>("MapJSON").WaitForCompletion();
        blockParentObjectPrefab = Addressables.LoadAssetAsync<GameObject>("BlockParentObjectPrefab").WaitForCompletion();
        blockPrefabs = Util.MapStringKeyToObjectWithLabel<GameObject>("BlockPrefabs");
    }

}
