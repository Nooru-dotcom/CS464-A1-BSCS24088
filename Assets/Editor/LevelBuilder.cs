using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LevelBuilder
{
    static GameObject floorPrefab;
    static GameObject wallPrefab;
    static GameObject coverPrefab;
    static GameObject rampPrefab;
    static GameObject markerPrefab;
    static GameObject goalPrefab;
    static GameObject spawnPrefab;
    static GameObject landmarkPrefab;
    static GameObject rewardPrefab;
    static GameObject playerPrefab;

    [MenuItem("CS464/Build All Levels")]
    static void BuildAll()
    {
        MakeFolder("Assets", "Prefabs");
        MakeFolder("Assets", "Materials");
        MakeFolder("Assets", "Scenes");

        floorPrefab = MakePrefab("Floor", PrimitiveType.Cube, new Color(0.55f, 0.55f, 0.55f));
        wallPrefab = MakePrefab("Wall", PrimitiveType.Cube, new Color(0.30f, 0.30f, 0.33f));
        coverPrefab = MakePrefab("Cover", PrimitiveType.Cube, new Color(0.65f, 0.50f, 0.35f));
        rampPrefab = MakePrefab("Ramp", PrimitiveType.Cube, new Color(0.45f, 0.55f, 0.65f));
        markerPrefab = MakePrefab("Marker", PrimitiveType.Cube, new Color(1.00f, 0.85f, 0.10f));
        goalPrefab = MakePrefab("Goal", PrimitiveType.Cylinder, new Color(0.10f, 0.90f, 0.30f));
        spawnPrefab = MakePrefab("SpawnPad", PrimitiveType.Cylinder, new Color(0.20f, 0.50f, 1.00f));
        landmarkPrefab = MakePrefab("Landmark", PrimitiveType.Cylinder, new Color(1.00f, 0.60f, 0.10f));
        rewardPrefab = MakePrefab("Reward", PrimitiveType.Cube, new Color(1.00f, 0.30f, 0.80f));
        playerPrefab = MakePrefab("Player", PrimitiveType.Capsule, new Color(0.90f, 0.15f, 0.15f));

        BuildLevel1();
        BuildLevel2();
        BuildLevel3();
        BuildLevel4();
        BuildLevel5();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("All five levels built. Look in Assets/Scenes.");
    }

    static void MakeFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + name))
        {
            AssetDatabase.CreateFolder(parent, name);
        }
    }

    static Material MakeMaterial(string name, Color color)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (existing != null)
        {
            return existing;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        Material material = new Material(shader);
        material.color = color;

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    static GameObject MakePrefab(string name, PrimitiveType type, Color color)
    {
        GameObject temp = GameObject.CreatePrimitive(type);
        temp.name = name;
        temp.GetComponent<Renderer>().sharedMaterial = MakeMaterial(name, color);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temp, "Assets/Prefabs/" + name + ".prefab");
        Object.DestroyImmediate(temp);

        return prefab;
    }

    static GameObject Place(GameObject prefab, string name, Vector3 position, Vector3 scale)
    {
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        return obj;
    }

    static void Floor(float x0, float x1, float z0, float z1, float top)
    {
        float width = x1 - x0;
        float depth = z1 - z0;
        float centerX = (x0 + x1) / 2f;
        float centerZ = (z0 + z1) / 2f;

        Place(floorPrefab, "Floor", new Vector3(centerX, top - 0.25f, centerZ), new Vector3(width, 0.5f, depth));
    }

    static void Wall(float x0, float x1, float z0, float z1, float baseY)
    {
        float width = x1 - x0;
        float depth = z1 - z0;
        float centerX = (x0 + x1) / 2f;
        float centerZ = (z0 + z1) / 2f;

        Place(wallPrefab, "Wall", new Vector3(centerX, baseY + 1.5f, centerZ), new Vector3(width, 3f, depth));
    }

    static void Cover(float x, float z, float width, float depth)
    {
        Place(coverPrefab, "LowCover", new Vector3(x, 0.55f, z), new Vector3(width, 1.1f, depth));
    }

    static void Marker(float x, float z, float y)
    {
        Place(markerPrefab, "Marker", new Vector3(x, y, z), new Vector3(0.5f, 0.1f, 0.5f));
    }

    static void Ramp(float x, float width, float z0, float z1, float y0, float y1)
    {
        float run = z1 - z0;
        float rise = y1 - y0;
        float length = Mathf.Sqrt(run * run + rise * rise);
        float angle = Mathf.Atan2(rise, run);
        float centerY = (y0 + y1) / 2f - 0.15f * Mathf.Cos(angle);
        float centerZ = (z0 + z1) / 2f;

        GameObject ramp = Place(rampPrefab, "Ramp", new Vector3(x, centerY, centerZ), new Vector3(width, 0.3f, length));
        ramp.transform.rotation = Quaternion.Euler(-angle * Mathf.Rad2Deg, 0f, 0f);
    }

    static void Goal(float x, float z, float top)
    {
        Place(goalPrefab, "Goal", new Vector3(x, top + 0.15f, z), new Vector3(3f, 0.15f, 3f));
    }

    static void Landmark(float x, float z, float top)
    {
        Place(landmarkPrefab, "Landmark", new Vector3(x, top + 5f, z), new Vector3(1.5f, 5f, 1.5f));
    }

    static void Reward(float x, float z)
    {
        Place(rewardPrefab, "Reward", new Vector3(x, 0.75f, z), new Vector3(1f, 1f, 1f));
    }

    static void SpawnAndPlayer(float x, float z, float top)
    {
        Place(spawnPrefab, "SpawnPad", new Vector3(x, top + 0.05f, z), new Vector3(3f, 0.05f, 3f));
        Place(playerPrefab, "Player", new Vector3(x, top + 1f, z), new Vector3(1f, 1f, 1f));
    }

    static void SpawnRoom(float doorHalf)
    {
        Floor(-5f, 5f, 0f, 10f, 0f);
        Wall(-5.5f, -5f, -0.5f, 10.5f, 0f);
        Wall(5f, 5.5f, -0.5f, 10.5f, 0f);
        Wall(-5f, 5f, -0.5f, 0f, 0f);
        Wall(-5f, -doorHalf, 10f, 10.5f, 0f);
        Wall(doorHalf, 5f, 10f, 10.5f, 0f);
        SpawnAndPlayer(0f, 3f, 0f);
    }

    static void NewLevel()
    {
        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
    }

    static void SetupCamera(float centerX, float centerZ, float size)
    {
        Camera cam = Camera.main;
        cam.orthographic = true;
        cam.orthographicSize = size;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.12f, 0.12f, 0.14f);
        cam.transform.position = new Vector3(centerX, 60f, centerZ);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    static void SaveLevel(string name)
    {
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "Assets/Scenes/" + name + ".unity");
    }

    static void BuildLevel1()
    {
        NewLevel();

        SpawnRoom(1.5f);

        Floor(-1.5f, 1.5f, 10f, 22f, 0f);
        Wall(-2f, -1.5f, 10f, 22f, 0f);
        Wall(1.5f, 2f, 10f, 22f, 0f);

        Floor(-8f, 8f, 25f, 48f, 0f);
        Wall(-8.5f, -8f, 25f, 48f, 0f);
        Wall(8f, 8.5f, 25f, 48f, 0f);
        Wall(-8.5f, 8.5f, 48f, 48.5f, 0f);

        Cover(-4f, 32f, 3f, 1f);
        Cover(4f, 36f, 3f, 1f);

        Goal(0f, 42f, 0f);
        Landmark(0f, 46f, 0f);

        SetupCamera(0f, 23.5f, 27f);
        SaveLevel("Level01");
    }

    static void BuildLevel2()
    {
        NewLevel();

        SpawnRoom(1.5f);
        Wall(-10f, -5f, 10f, 10.5f, 0f);
        Wall(5f, 10f, 10f, 10.5f, 0f);

        Floor(-10f, 10f, 10f, 30f, 0f);
        Wall(-10.5f, -10f, 10f, 30f, 0f);
        Wall(10f, 10.5f, 10f, 16f, 0f);
        Wall(10f, 10.5f, 24f, 30f, 0f);

        Wall(-1f, 1f, 14f, 26f, 0f);

        Cover(-6f, 18f, 3f, 1f);
        Cover(-4f, 23f, 3f, 1f);

        Floor(10f, 16f, 16f, 24f, 0f);
        Wall(16f, 16.5f, 15.5f, 24.5f, 0f);
        Wall(10f, 16.5f, 15.5f, 16f, 0f);
        Wall(10f, 16.5f, 24f, 24.5f, 0f);
        Reward(13f, 20f);

        for (float z = 12f; z <= 28f; z = z + 3f)
        {
            Marker(-7.5f, z, 0.05f);
        }

        Wall(-10f, -8f, 30f, 30.5f, 0f);
        Wall(8f, 10f, 30f, 30.5f, 0f);

        Floor(-8f, 8f, 30f, 46f, 0f);
        Wall(-8.5f, -8f, 30f, 46f, 0f);
        Wall(8f, 8.5f, 30f, 46f, 0f);
        Wall(-8.5f, 8.5f, 46f, 46.5f, 0f);

        Goal(0f, 42f, 0f);

        SetupCamera(3f, 22f, 26f);
        SaveLevel("Level02");
    }

    static void BuildLevel3()
    {
        NewLevel();

        SpawnRoom(2f);

        Ramp(0f, 4f, 10f, 16f, 0f, 3f);

        Floor(-4f, 4f, 16f, 24f, 3f);
        Floor(-0.75f, 0.75f, 24f, 36f, 3f);
        Floor(-5f, 5f, 36f, 46f, 3f);
        Wall(-5f, 5f, 46f, 46.5f, 3f);

        for (float z = 11f; z <= 15f; z = z + 2f)
        {
            Marker(0f, z, (z - 10f) * 0.5f + 0.2f);
        }

        for (float z = 18f; z <= 35f; z = z + 2f)
        {
            Marker(0f, z, 3.05f);
        }

        Goal(0f, 42f, 3f);

        SetupCamera(0f, 22.8f, 26f);
        SaveLevel("Level03");
    }

    static void BuildLevel4()
    {
        NewLevel();

        SpawnRoom(0.75f);

        Floor(-0.75f, 0.75f, 10f, 24f, 0f);
        Wall(-1.25f, -0.75f, 10f, 24f, 0f);
        Wall(0.75f, 1.25f, 10f, 24f, 0f);

        Floor(-15f, 15f, 24f, 50f, 0f);
        Wall(-15.5f, -15f, 24f, 50.5f, 0f);
        Wall(15f, 15.5f, 24f, 50.5f, 0f);
        Wall(-15f, 15f, 50f, 50.5f, 0f);
        Wall(-15f, -0.75f, 24f, 24.5f, 0f);
        Wall(0.75f, 15f, 24f, 24.5f, 0f);

        Cover(-6f, 30f, 4f, 1f);
        Cover(6f, 30f, 4f, 1f);
        Cover(0f, 35f, 6f, 1f);
        Cover(-8f, 40f, 3f, 1f);
        Cover(8f, 40f, 3f, 1f);

        Goal(0f, 46f, 0f);
        Landmark(0f, 49f, 0f);

        SetupCamera(0f, 25f, 27f);
        SaveLevel("Level04");
    }

    static void BuildLevel5()
    {
        NewLevel();

        SpawnRoom(1.5f);

        Floor(-1.5f, 1.5f, 10f, 20f, 0f);
        Wall(-2f, -1.5f, 10f, 20f, 0f);
        Wall(1.5f, 2f, 10f, 20f, 0f);

        Floor(-8f, 8f, 22.5f, 42f, 0f);
        Wall(-8.5f, -8f, 22.5f, 42.5f, 0f);
        Wall(8f, 8.5f, 22.5f, 42.5f, 0f);
        Wall(-8f, -6f, 42f, 42.5f, 0f);
        Wall(6f, 8f, 42f, 42.5f, 0f);

        Wall(-0.5f, 0.5f, 27f, 38f, 0f);

        Cover(-4f, 29f, 3f, 1f);
        Cover(-5f, 34f, 3f, 1f);

        Ramp(4.5f, 3f, 24f, 28f, 0f, 1.5f);
        Floor(3f, 6f, 28f, 36f, 1.5f);
        Ramp(4.5f, 3f, 36f, 40f, 1.5f, 0f);

        Floor(-6f, 6f, 42f, 56f, 0f);
        Wall(-6.5f, -6f, 42f, 56f, 0f);
        Wall(6f, 6.5f, 42f, 56f, 0f);
        Wall(-6.5f, 6.5f, 56f, 56.5f, 0f);

        Goal(0f, 53f, 0f);

        GameObject lightObject = new GameObject("GoalLight");
        lightObject.transform.position = new Vector3(0f, 4f, 53f);
        Light goalLight = lightObject.AddComponent<Light>();
        goalLight.type = LightType.Point;
        goalLight.color = Color.green;
        goalLight.range = 20f;
        goalLight.intensity = 30f;

        Light[] allLights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);

        for (int i = 0; i < allLights.Length; i++)
        {
            if (allLights[i].type == LightType.Directional)
            {
                allLights[i].intensity = 0.4f;
            }
        }

        SetupCamera(0f, 27.7f, 30f);
        SaveLevel("Level05");
    }
}
