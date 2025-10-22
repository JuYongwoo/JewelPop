using JYW.JewelPop.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JYW.JewelPop.Managers
{
    public enum GoalType
    {
        Joker

    }
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
            levelDatasJSONHandle = Util.AsyncLoad<TextAsset>("1");
            blockParentObjectHandle = Util.AsyncLoad<GameObject>("BlockParentObjectPrefab");
            jokerScoreFxHandle = Util.AsyncLoad<GameObject>("JokerScore");

            blockPrefabsHandles = Util.LoadDictWithEnum<BlockPrefabs, GameObject>();
            blockCrushFxPrefabsHandles = Util.LoadDictWithEnum<BlockCrushFXPrefabs, GameObject>();
            gameSoundClipsHandles = Util.LoadDictWithEnum<Sounds, AudioClip>();
        }
    }


}