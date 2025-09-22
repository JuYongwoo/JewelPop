using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager
{
    public TextAsset mapJSON;

    public GameObject blockParentObjectPrefab;
    public Dictionary<string, GameObject> blockPrefabs = new Dictionary<string, GameObject>();

    public GameObject blockCrushFxPrefab;
    public GameObject jokerScoreFxPrefab;


    public void OnAwake()
    {
        mapJSON = Addressables.LoadAssetAsync<TextAsset>("MapJSON").WaitForCompletion();
        
        blockParentObjectPrefab = Addressables.LoadAssetAsync<GameObject>("BlockParentObjectPrefab").WaitForCompletion();
        blockPrefabs = Util.LoadDictByLabel<GameObject>("BlockChildPrefabs");

        blockCrushFxPrefab = Addressables.LoadAssetAsync<GameObject>("BlockCrush").WaitForCompletion();
        jokerScoreFxPrefab = Addressables.LoadAssetAsync<GameObject>("JokerScore").WaitForCompletion();
    
    }

}
