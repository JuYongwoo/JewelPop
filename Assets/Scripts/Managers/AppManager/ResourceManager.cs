using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum BlockPrefabs
{
    r,
    g,
    p,
    pp,
    y,
    o,
    j
}

public enum BlockCrushFXPrefabs
{
    BlockCrush_r,
    BlockCrush_g,
    BlockCrush_p,
    BlockCrush_pp,
    BlockCrush_y,
    BlockCrush_o
}

public enum Sounds
{
    Block4SFX,
    Block3SFX,
    ScoreGetSFX,
    Victory,
    BGM1
}

public class ResourceManager
{
    public AsyncOperationHandle<TextAsset> levelDatasJSONHandle;
    public AsyncOperationHandle<GameObject> blockParentObjectHandle;
    public Dictionary<BlockPrefabs, AsyncOperationHandle<GameObject>> blockPrefabsHandles;
    public Dictionary<BlockCrushFXPrefabs, AsyncOperationHandle<GameObject>> blockCrushFxPrefabsHandles;
    public AsyncOperationHandle<GameObject> jokerScoreFxHandle;
    public Dictionary<Sounds, AsyncOperationHandle<AudioClip>> gameSoundClipsHandles;


    public void StartPreload()
    {
        levelDatasJSONHandle = Addressables.LoadAssetAsync<TextAsset>("1");
        blockParentObjectHandle = Addressables.LoadAssetAsync<GameObject>("BlockParentObjectPrefab");
        jokerScoreFxHandle = Addressables.LoadAssetAsync<GameObject>("JokerScore");
        
        blockPrefabsHandles = new Dictionary<BlockPrefabs, AsyncOperationHandle<GameObject>>();
        foreach (BlockPrefabs e in System.Enum.GetValues(typeof(BlockPrefabs)))
        {
            blockPrefabsHandles[e] = Addressables.LoadAssetAsync<GameObject>(e.ToString());
        }

        blockCrushFxPrefabsHandles = new Dictionary<BlockCrushFXPrefabs, AsyncOperationHandle<GameObject>>();
        foreach (BlockCrushFXPrefabs e in System.Enum.GetValues(typeof(BlockCrushFXPrefabs)))
        {
            blockCrushFxPrefabsHandles[e] = Addressables.LoadAssetAsync<GameObject>(e.ToString());
        }

        gameSoundClipsHandles = new Dictionary<Sounds, AsyncOperationHandle<AudioClip>>();
        foreach (Sounds e in System.Enum.GetValues(typeof(Sounds)))
        {
            gameSoundClipsHandles[e] = Addressables.LoadAssetAsync<AudioClip>(e.ToString());
        }
    }
}

// ===== 사용 예시(동기 한 줄) =====
// var prefab = AppManager.instance.resourceManager.GetBlockPrefab(BlockPrefabs.r);
