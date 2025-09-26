using System.Collections.Generic;
using System.Linq;
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
    public Dictionary<int, TextAsset> LevelDatasJSON;
    // ===== 비동기 프리로드 핸들 =====
    public AsyncOperationHandle<IList<TextAsset>> levelDatasJSONHandle;
    public AsyncOperationHandle<GameObject> blockParentObjectHandle;
    public Dictionary<BlockPrefabs, AsyncOperationHandle<GameObject>> blockPrefabsHandles;
    public Dictionary<BlockCrushFXPrefabs, AsyncOperationHandle<GameObject>> blockCrushFxPrefabsHandles;
    public AsyncOperationHandle<GameObject> jokerScoreFxHandle;
    public Dictionary<Sounds, AsyncOperationHandle<AudioClip>> gameSoundClipsHandles;


    // ===== 비동기 프리로드 시작(로비/스플래시 등에서 호출) =====
    public void StartPreload()
    {
        LevelDatasJSON = Util.LoadDictByLabel<int, TextAsset>("LevelDatasJSON");
        levelDatasJSONHandle = Addressables.LoadAssetsAsync<TextAsset>("LevelDatasJSON", null);
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

    // ===== 준비 상태 확인 =====
    public bool IsReady_Block(BlockPrefabs e)
    {
        return blockPrefabsHandles != null
            && blockPrefabsHandles.TryGetValue(e, out var h)
            && h.IsDone && h.Status == AsyncOperationStatus.Succeeded;
    }

    public bool IsReady_BlockCrush(BlockCrushFXPrefabs e)
    {
        return blockCrushFxPrefabsHandles != null
            && blockCrushFxPrefabsHandles.TryGetValue(e, out var h)
            && h.IsDone && h.Status == AsyncOperationStatus.Succeeded;
    }

    public bool IsReady_Sound(Sounds e)
    {
        return gameSoundClipsHandles != null
            && gameSoundClipsHandles.TryGetValue(e, out var h)
            && h.IsDone && h.Status == AsyncOperationStatus.Succeeded;
    }

    public bool IsReady_BlockParent() =>
        blockParentObjectHandle.IsValid() && blockParentObjectHandle.IsDone && blockParentObjectHandle.Status == AsyncOperationStatus.Succeeded;

    public bool IsReady_JokerScore() =>
        jokerScoreFxHandle.IsValid() && jokerScoreFxHandle.IsDone && jokerScoreFxHandle.Status == AsyncOperationStatus.Succeeded;

    public bool IsReady_LevelDatas() =>
        levelDatasJSONHandle.IsValid() && levelDatasJSONHandle.IsDone && levelDatasJSONHandle.Status == AsyncOperationStatus.Succeeded;

    // 전체 준비 확인(로딩 화면에서 폴링)
    public bool IsAllPreloaded()
    {
        bool blocks = blockPrefabsHandles != null && blockPrefabsHandles.Values.All(h => h.IsDone && h.Status == AsyncOperationStatus.Succeeded);
        bool crush = blockCrushFxPrefabsHandles != null && blockCrushFxPrefabsHandles.Values.All(h => h.IsDone && h.Status == AsyncOperationStatus.Succeeded);
        bool sounds = gameSoundClipsHandles != null && gameSoundClipsHandles.Values.All(h => h.IsDone && h.Status == AsyncOperationStatus.Succeeded);
        return IsReady_BlockParent() && IsReady_JokerScore() && IsReady_LevelDatas() && blocks && crush && sounds;
    }

    // ===== 사용 시 동기 한 줄 액세스 =====
    public GameObject GetBlockPrefab(BlockPrefabs e)
    {
        var h = blockPrefabsHandles[e];
        if (!h.IsDone) { Debug.LogError($"[ResourceManager] Block {e} not ready"); return null; }
        return h.Result;
    }

    public GameObject GetBlockCrushFx(BlockCrushFXPrefabs e)
    {
        var h = blockCrushFxPrefabsHandles[e];
        if (!h.IsDone) { Debug.LogError($"[ResourceManager] BlockCrushFX {e} not ready"); return null; }
        return h.Result;
    }

    public AudioClip GetSound(Sounds e)
    {
        var h = gameSoundClipsHandles[e];
        if (!h.IsDone) { Debug.LogError($"[ResourceManager] Sound {e} not ready"); return null; }
        return h.Result;
    }

    public GameObject GetBlockParent()
    {
        if (!IsReady_BlockParent()) { Debug.LogError("[ResourceManager] BlockParent not ready"); return null; }
        return blockParentObjectHandle.Result;
    }

    public GameObject GetJokerScoreFx()
    {
        if (!IsReady_JokerScore()) { Debug.LogError("[ResourceManager] JokerScore not ready"); return null; }
        return jokerScoreFxHandle.Result;
    }

    public Dictionary<int, TextAsset> GetLevelDatas()
    {
        if (!IsReady_LevelDatas()) { Debug.LogError("[ResourceManager] LevelDatas not ready"); return null; }
        if (LevelDatasJSON == null)
        {
            LevelDatasJSON = new Dictionary<int, TextAsset>();
            int idx = 0;
            foreach (var ta in levelDatasJSONHandle.Result) LevelDatasJSON[idx++] = ta;
        }
        return LevelDatasJSON;
    }
}

// ===== 사용 예시(동기 한 줄) =====
// var prefab = AppManager.instance.resourceManager.GetBlockPrefab(BlockPrefabs.r);
