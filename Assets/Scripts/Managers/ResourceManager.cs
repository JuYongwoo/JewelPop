using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager
{
    public TextAsset mapJSON;
    public GameObject blockParentObjectPrefab;
    public GameObject blockCrush;
    public Dictionary<BlockType, GameObject> blockPrefabs;

    public void OnAwake()
    {
        mapJSON = Addressables.LoadAssetAsync<TextAsset>("MapJSON").WaitForCompletion();
        blockParentObjectPrefab = Addressables.LoadAssetAsync<GameObject>("BlockParentObjectPrefab").WaitForCompletion();
        blockCrush = Addressables.LoadAssetAsync<GameObject>("BlockCrush").WaitForCompletion();
        blockPrefabs = Util.MapEnumToObjectWithEnumKey<BlockType, GameObject>();
    }

}
