using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public Dictionary<string, TextAsset> LevelDatasJSON;

    public GameObject blockParentObjectPrefab;
    public Dictionary<string, GameObject> blockPrefabs = new Dictionary<string, GameObject>();

    public Dictionary<string, GameObject> blockCrushFxPrefabs;
    public GameObject jokerScoreFxPrefab;


    public void OnAwake()
    {
        LevelDatasJSON = Util.LoadDictByLabel<TextAsset>("LevelDatasJSON");

        blockParentObjectPrefab = Util.Load<GameObject>("BlockParentObjectPrefab");
        blockPrefabs = Util.LoadDictByLabel<GameObject>("BlockChildPrefabs");

        blockCrushFxPrefabs = Util.LoadDictByLabel<GameObject>("BlockCrushFX");
        jokerScoreFxPrefab = Util.Load<GameObject>("JokerScore");
    
    }

}
