using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;


public class LoadMesh : MonoBehaviour
{
    public GameObject target;
    public TMP_Dropdown levelDropdown;
    private List<string> _levels;
    private int _selectedLevel;
    private WorldEventsScript _worldEventsScript;
    public GameObject selectionCirclePrefab;

    public SelectionRectangleBehavior selectionRectangleBehavior;

    public string folderPath = "Assets/GameAssets"; // Default to the Assets folder

    [DllImport("UnityGlue")]
    private static extern float SayHello();

    [DllImport("UnityGlue")]
    private static extern void GluLoadMesh(char[] levelName, char[] meshName, int instanceNum, float[] vertices, float[] norms, float[] uvs, int[] tris);
    
    [DllImport("UnityGlue")]
    private static extern int GluMeshVerticesNumber(char[] levelName, char[] meshName);
    [DllImport("UnityGlue")]
    private static extern int GluMeshNormalsNumber(char[] levelName, char[] meshName);
    [DllImport("UnityGlue")]
    private static extern int GluMeshUVNumber(char[] levelName, char[] meshName);
    [DllImport("UnityGlue")]
    private static extern int GluMeshTrianglesNumber(char[] levelName, char[] meshName);
    
    [DllImport("UnityGlue")]
    private static extern bool GluHasMeshTerrain(char[] levelName, char[] meshName);
    [DllImport("UnityGlue")]
    private static extern int GluGetLevelInstancesCount(char[] levelName);
    [DllImport("UnityGlue")]
    private static extern void GluGetLevelInstanceName(char[] levelName, int index, byte[] instName);
    [DllImport("UnityGlue")]
    private static extern int GluGetLevelInstanceNameCount(char[] levelName, char[] instName);
    
    void Start()
    {
        SayHello();
        
        _worldEventsScript = FindObjectOfType<WorldEventsScript>();
        ListAssetsWithTypes(folderPath);
        _levels = GetLoadableMeshes(folderPath);
        Debug.Log(_levels.Count);
        
        levelDropdown.ClearOptions();
        levelDropdown.AddOptions(_levels);
        
        foreach (var mesh in _levels)
        {
            Debug.Log(mesh);
        }
    }
    
    public void OnSelectLevel(int levelId)
    {
        _selectedLevel = levelDropdown.value;
    }

    private void ListAssetsWithTypes(string path)
    {
        // Get an array of all asset paths at the specified folder path
        string[] assetPaths = AssetDatabase.FindAssets("", new[] { path });

        // Loop through each asset path
        foreach (string assetPath in assetPaths)
        {
            // Convert the asset path to a relative path
            string relativePath = AssetDatabase.GUIDToAssetPath(assetPath);

            // Get the type of the asset at the relative path
            Type assetType = AssetDatabase.GetMainAssetTypeAtPath(relativePath);

            // Output the name of the asset and its type
            Debug.Log("Asset: " + Path.GetFileName(relativePath) + " | Type: " + assetType);
        }
    }

    private static List<string> GetLoadableMeshes(string path)
    {
        var meshes = new List<string>();
        var objects = AssetDatabase.FindAssets("t:GameObject", new []{path});
        Debug.Log(objects);
        // string pattern = @".+/(.+?).obj$";
        foreach (var obj in objects)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(obj);
            if (assetPath.EndsWith(".obj"))
            {
                var level = Path.GetFileNameWithoutExtension(assetPath);
                if (GluHasMeshTerrain(level.ToCharArray(), "Terrain".ToCharArray()))
                {
                    meshes.Add(level);
                }
            }
        }
        
        return meshes;
    }
    
    public void OnButtonClick()
    {
        var levelName = _levels[_selectedLevel];
        Debug.Log("Click");
        
        LoadLevel(levelName:levelName);
    }

    private void LoadLevel(string levelName)
    {
        GameObject goTerrain = new GameObject(levelName);
        var meshFilter = goTerrain.AddComponent<MeshFilter>();
        meshFilter.mesh = GetMesh(levelName, "Terrain");
        meshFilter.mesh.name = levelName;

        goTerrain.AddComponent<SelectedUnitsHolderBehavior>();

        selectionRectangleBehavior.selectedUnitsHolderBehavior = goTerrain.GetComponent<SelectedUnitsHolderBehavior>();
        
        MeshRenderer meshRenderer = goTerrain.AddComponent<MeshRenderer>();
        
        // Assign the default material to the MeshRenderer
        meshRenderer.material = new Material(Shader.Find("Diffuse"));
        
        // give a collider to terrain
        goTerrain.AddComponent<MeshCollider>();
        goTerrain.GetComponent<MeshCollider>().providesContacts = true;
    
        foreach (Transform child in target.transform)
        {
            Destroy(child.gameObject);
        }
        
        goTerrain.transform.parent = target.transform;
        //go.transform.localScale *= 10;
        
        _worldEventsScript.EventLevelChanged.Invoke(meshFilter);

        if (!Directory.Exists("Assets/DynamicPrefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "DynamicPrefabs");
        }

        AssetDatabase.DeleteAsset("Assets/DynamicPrefabs/" + levelName);
        AssetDatabase.CreateFolder("Assets/DynamicPrefabs", levelName);

        var count = GluGetLevelInstancesCount(levelName.ToCharArray());

        for (int i = 0; i < count; i++)
        {
            var instName = new byte[256];

            GluGetLevelInstanceName(levelName.ToCharArray(), i, instName);
            string prefName = Encoding.UTF8.GetString(instName);
            
            GameObject goIn = new GameObject(prefName);
            var inMeshFilter = goIn.AddComponent<MeshFilter>();
            inMeshFilter.mesh = GetMesh(levelName, prefName);
            goIn.AddComponent<MeshRenderer>();
            
            string localPath = "Assets/DynamicPrefabs/" +levelName+"/"+ goIn.name + ".prefab";

            // Make sure the file name is unique, in case an existing Prefab has the same name.
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            var prefab = PrefabUtility.SaveAsPrefabAsset(goIn, localPath, out var prefabSuccess);
            if (prefabSuccess)
                Debug.Log("Prefab was saved successfully");
            else 
                Debug.Log("Prefab failed to save" + localPath);
            
            int iCount = GluGetLevelInstanceNameCount(levelName.ToCharArray(), prefName.ToCharArray());

            Debug.Log(iCount);

            for (int j = 0; j < iCount; j++)
            {
                var mesh = GetMesh(levelName: levelName, meshName: prefName, instanceNum: j);

                var inst = Instantiate(prefab, mesh.vertices[0], Quaternion.identity);
                inst.name = $"{prefName}_(clone)_{i}";
                inst.GetComponent<MeshFilter>().mesh = inMeshFilter.mesh;
                inst.transform.parent = goTerrain.transform;
                
                inst.AddComponent<BoxCollider>();
                
                // pour collisionner avec le terrain
                inst.GetComponent<BoxCollider>().providesContacts = true;

                inst.AddComponent<Rigidbody>();
                
                inst.AddComponent<MouseSelectableBehavior>();
                var sel = inst.GetComponent<MouseSelectableBehavior>();
                sel.selectionCirclePrefab = selectionCirclePrefab;
                sel.selectedUnitsHolderBehavior =
                    goTerrain.GetComponent<SelectedUnitsHolderBehavior>();

                selectionRectangleBehavior.selectable.Add(new WeakReference<GameObject>(inst));
                

            }
            
            Destroy(goIn);
        }
    }
    
    private Mesh GetMesh(string levelName, string meshName, int instanceNum = -1)
    {
        Debug.Log( GluMeshVerticesNumber(levelName.ToCharArray(), meshName.ToCharArray()));
        
        int vc = GluMeshVerticesNumber(levelName.ToCharArray(), meshName.ToCharArray());
        int nc = GluMeshNormalsNumber(levelName.ToCharArray(), meshName.ToCharArray());
        int uvc = GluMeshUVNumber(levelName.ToCharArray(), meshName.ToCharArray());
        int tc = GluMeshTrianglesNumber(levelName.ToCharArray(), meshName.ToCharArray());

        var vertices = new float[vc*3];
        var norms = new float[nc*3];
        var uvs = new float[uvc * 2];
        var tris = new int[tc];

        GluLoadMesh(levelName.ToCharArray(), meshName.ToCharArray(), instanceNum, vertices, norms, uvs, tris);
        
        var mesh = new Mesh
        {
            name = meshName,
        };

        var verticesP = new Vector3[vc];
        for (int i = 0; i < vertices.Length / 3; i++)
        {
            verticesP[i] = new Vector3(vertices[i * 3 + 0], vertices[i * 3 + 1], vertices[i * 3 + 2]);
        }

        var normalsP = new Vector3[nc];
        for (int i = 0; i < norms.Length / 3; i++)
        {
            normalsP[i] = new Vector3(
                norms[i * 3 + 0], norms[i * 3 + 1], norms[i * 3 + 2]
            );
        }
        
        mesh.Clear();
        mesh.vertices = verticesP;
        mesh.normals = normalsP;
        mesh.triangles = tris;
        
        Debug.Log($"vertices {vertices.Length} {norms.Length}  {tris.Length}");

        return mesh;
    }
}
