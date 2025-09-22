using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager
{
    public TextAsset mapJSON;

    public GameObject blockParentObjectPrefab;
    public Dictionary<string, GameObject> blockPrefabs = new Dictionary<string, GameObject>();

    public Dictionary<string, GameObject> blockCrushFxPrefabs;
    public GameObject jokerScoreFxPrefab;


    public void OnAwake()
    {
        mapJSON = Addressables.LoadAssetAsync<TextAsset>("MapJSON").WaitForCompletion();
        
        blockParentObjectPrefab = Addressables.LoadAssetAsync<GameObject>("BlockParentObjectPrefab").WaitForCompletion();
        blockPrefabs = Util.LoadDictByLabel<GameObject>("BlockChildPrefabs");

        blockCrushFxPrefabs = Util.LoadDictByLabel<GameObject>("BlockCrushFX");
        jokerScoreFxPrefab = Addressables.LoadAssetAsync<GameObject>("JokerScore").WaitForCompletion();
    
    }

}
