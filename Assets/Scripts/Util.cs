using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting.ReorderableList.Internal;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Node : IComparable<Node>
{
    public Vector2Int pos;
    public float priority; // fScore

    public Node(Vector2Int p, float f)
    {
        pos = p;
        priority = f;
    }

    public int CompareTo(Node other)
    {
        // priority가 낮을수록 우선순위가 높도록 //AStar 계산 시 휴리스틱 계산값이 들어갈 것임
        return -priority.CompareTo(other.priority);
    }
}
public class Util
{
    // ====== 기존 유틸 메서드들 (원본 유지) ======
    public static Dictionary<T, T2> mapDictionaryInChildren<T, T2>(GameObject go) where T : Enum where T2 : UnityEngine.Object
    {
        Dictionary<T, T2> dict = new Dictionary<T, T2>();
        Transform[] children = go.GetComponentsInChildren<Transform>();

        foreach (T enumName in Enum.GetValues(typeof(T)))
        {
            foreach (Transform child in children)
            {
                if (child.name == enumName.ToString())
                {
                    if (typeof(T2) == typeof(GameObject))
                        dict[enumName] = (T2)(object)child.gameObject;
                    else
                        dict[enumName] = child.GetComponent<T2>();
                    break;
                }
            }
        }
        return dict;
    }

    public static GameObject getObjectInChildren(GameObject go, string childName)
    {
        Transform[] children = go.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
            if (child.name == childName)
                return child.gameObject;
        return null;
    }

    // deprecated: Resources → Addressables 권장 (원본 유지)
    public static Dictionary<T, T2> mapEumToObjectDEPRECATED<T, T2>(string filePath) where T : Enum where T2 : UnityEngine.Object
    {
        Dictionary<T, T2> dict = new Dictionary<T, T2>();
        foreach (T s in Enum.GetValues(typeof(T)))
            dict[s] = Resources.Load<T2>(filePath + "/" + s.ToString());
        return dict;
    }

    public static Dictionary<T, T2> mapEnumToObjectWithEnumKey<T, T2>() where T : Enum where T2 : UnityEngine.Object
    {
        Dictionary<T, T2> dict = new Dictionary<T, T2>();
        foreach (T s in Enum.GetValues(typeof(T)))
            dict[s] = Addressables.LoadAssetAsync<T2>(s.ToString()).WaitForCompletion();
        return dict;
    }

    public static Dictionary<TEnum, TObject> MapEnumToObjectWithLabelAndEnumKey<TEnum, TObject>(string label) where TEnum : Enum where TObject : UnityEngine.Object
    {
        var dict = new Dictionary<TEnum, TObject>();
        var locHandle = Addressables.LoadResourceLocationsAsync(label, typeof(TObject));
        var addressList = locHandle.WaitForCompletion();

        if (addressList == null || addressList.Count == 0)
        {
            Debug.LogWarning($"[Util] No Addressable assets found for label {label}");
            return dict;
        }

        foreach (TEnum e in Enum.GetValues(typeof(TEnum)))
        {
            string enumName = e.ToString();
            foreach (var addr in addressList)
            {
                if (addr.PrimaryKey.Equals(enumName, StringComparison.OrdinalIgnoreCase))
                {
                    var assetHandle = Addressables.LoadAssetAsync<TObject>(addr);
                    var asset = assetHandle.WaitForCompletion();
                    dict[e] = asset;
                    break;
                }
            }
            if (!dict.ContainsKey(e))
            {
                Debug.LogWarning($"[Util] No matching asset for Enum {enumName} in label {label}");
                dict[e] = null;
            }
        }
        return dict;
    }

    public static Dictionary<TEnum, TObject> MapEnumToObjectWithOnlyLabels<TEnum, TObject>(string commonLabel) where TEnum : Enum where TObject : UnityEngine.Object
    {
        var dict = new Dictionary<TEnum, TObject>();
        foreach (TEnum e in Enum.GetValues(typeof(TEnum)))
        {
            IEnumerable keys = new[] { commonLabel, e.ToString() };
            var locHandle = Addressables.LoadResourceLocationsAsync(keys, Addressables.MergeMode.Intersection, typeof(TObject));
            var locations = locHandle.WaitForCompletion();

            if (locations == null || locations.Count == 0)
            {
                Debug.LogWarning($"[MapEnumToAddressablesByLabels] No asset found for Enum {e} with label '{commonLabel}'");
                continue;
            }
            var assetHandle = Addressables.LoadAssetAsync<TObject>(locations[0]);
            var asset = assetHandle.WaitForCompletion();
            dict[e] = asset;

            Addressables.Release(locHandle);
            Addressables.Release(assetHandle);
        }
        return dict;
    }

    public static Dictionary<string, TObject> MapStringKeyToObjectWithLabel<TObject>(string commonLabel)
    where TObject : UnityEngine.Object
    {
        var dict = new Dictionary<string, TObject>();
        IEnumerable keys = new[] { commonLabel }; //라벨로 전부 불러와서
        var locHandle = Addressables.LoadResourceLocationsAsync(keys, Addressables.MergeMode.Intersection, typeof(TObject));
        var locations = locHandle.WaitForCompletion();

        foreach (var i in locations)
        {

            var assetHandle = Addressables.LoadAssetAsync<TObject>(i);
            var asset = assetHandle.WaitForCompletion();
            dict.Add(i.PrimaryKey, asset); //불러온 오브젝트들을 맵에 <키값, 오브젝트> 로 대응
            Addressables.Release(assetHandle);
        }
        Addressables.Release(locHandle);

        return dict;
    }

    public static T LoadOneResourceInFolderDEPRECATED<T>(string filePath) where T : UnityEngine.Object
    {
        T[] resources = Resources.LoadAll<T>(filePath);
        if (resources.Length == 0)
        {
            Debug.LogError($"No resources found at {filePath}");
            return null;
        }
        if (resources.Length > 1)
            Debug.LogWarning($"Multiple resources found at {filePath}, returning the first one.");
        return resources[0];
    }

    public static Dictionary<TEnum, TAsset> MapEnumToChildFileDEPRECATED<TEnum, TAsset>(string basePath, string fileName)
        where TEnum : Enum where TAsset : UnityEngine.Object
    {
        var dict = new Dictionary<TEnum, TAsset>();
        foreach (TEnum e in Enum.GetValues(typeof(TEnum)))
            dict[e] = Resources.Load<TAsset>($"{basePath}/{e}/{fileName}");
        return dict;
    }

    public static T AddOrGetComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null) component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        return transform == null ? null : transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null) return null;

        if (!recursive)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform t = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || t.name == name)
                {
                    T comp = t.GetComponent<T>();
                    if (comp != null) return comp;
                }
            }
        }
        else
        {
            foreach (T comp in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || comp.name == name)
                    return comp;
            }
        }
        return null;
    }

    public static List<List<string>> LoadGrid(string AddressableFileKey)
    {
        var textAsset = Addressables.LoadAssetAsync<TextAsset>(AddressableFileKey).WaitForCompletion();
        if (textAsset == null)
        {
            Debug.LogError($"[Util] Addressable '{AddressableFileKey}'을(를) 찾을 수 없습니다.");
            return new List<List<string>>();
        }

        string[] lines = textAsset.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        if (lines.Length == 0)
        {
            Debug.LogWarning($"[Util] Addressable '{AddressableFileKey}'의 내용이 비어 있습니다.");
            return new List<List<string>>();
        }

        string[] firstLine = lines[0].Trim().Split(',');
        int cols = firstLine.Length;
        List<List<string>> gridvalue = new List<List<string>>();

        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Trim().Split(',');
            var rowList = new List<string>();
            for (int j = 0; j < cols && j < row.Length; j++)
            {
                var cell = row[j].Trim();
                if (!string.IsNullOrEmpty(cell))
                    rowList.Add(cell);
            }
            gridvalue.Add(rowList);
        }
        return gridvalue;
    }

    public static void SaveCsv(string[,] map)
    {
        if (map == null)
            return;

        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        StringBuilder sb = new StringBuilder();

        for (int r = 0; r < rows; r++)
        {
            string[] row = new string[cols];
            for (int c = 0; c < cols; c++)
            {
                string value = map[r, c] ?? "";
                if (value.Contains(",") || value.Contains("\""))
                {
                    value = "\"" + value.Replace("\"", "\"\"") + "\"";
                }
                row[c] = value;
            }
            sb.AppendLine(string.Join(",", row));
        }

        File.WriteAllText(Application.dataPath + "/SavedMap.csv", sb.ToString(), Encoding.UTF8);
    }


}