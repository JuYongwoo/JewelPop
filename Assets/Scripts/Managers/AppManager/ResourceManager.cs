using System.Collections.Generic;
using UnityEngine;


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
    BGM1
}

public class ResourceManager
{
    public Dictionary<int, TextAsset> LevelDatasJSON;

    public Dictionary<BlockPrefabs, GameObject> blockPrefabs;
    public GameObject blockParentObjectPrefab;

    public Dictionary<BlockCrushFXPrefabs, GameObject> blockCrushFxPrefabs;
    public GameObject jokerScoreFxPrefab;

    public Dictionary<Sounds, AudioClip> gameSoundClips;


    public void Init()
    {
        LevelDatasJSON = Util.LoadDictByLabel<int, TextAsset>("LevelDatasJSON");

        blockParentObjectPrefab = Util.Load<GameObject>("BlockParentObjectPrefab");
        blockPrefabs = Util.LoadDictWithEnum<BlockPrefabs, GameObject>();

        blockCrushFxPrefabs = Util.LoadDictWithEnum<BlockCrushFXPrefabs, GameObject>();
        jokerScoreFxPrefab = Util.Load<GameObject>("JokerScore");

        gameSoundClips = Util.LoadDictWithEnum<Sounds, AudioClip>();

    }

}
