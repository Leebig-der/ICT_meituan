#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.Events;

public class RunningJumpSetup
{
    [MenuItem("RunningJump/Generate Main Scene")]
    public static void GenerateScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var camGO = Camera.main != null ? Camera.main.gameObject : new GameObject("Main Camera");
        var cam = camGO.GetComponent<Camera>();
        if (cam == null) cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5;
        cam.transform.position = new Vector3(0, 0, -10);

        var bg = new GameObject("Background");
        var sr = bg.AddComponent<SpriteRenderer>();
        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Background.png");
        if (bgSprite != null) sr.sprite = bgSprite;
        bg.transform.position = new Vector3(0, 0, 10);

        var ground = new GameObject("Ground");
        var gs = ground.AddComponent<SpriteRenderer>();
        var groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/GroundTile.png");
        if (groundSprite != null) gs.sprite = groundSprite;
        ground.transform.position = new Vector3(0, -3, 0);

        GameObject player1 = CreatePlayer("Player1", new Vector3(-2, -2, 0), 1, KeyCode.A, "Assets/Sprites/PlayerRed.png");
        GameObject player2 = CreatePlayer("Player2", new Vector3(-1, -2, 0), 2, KeyCode.LeftArrow, "Assets/Sprites/PlayerBlue.png");

        var spawnerGO = new GameObject("ObstacleSpawner");
        var spawner = spawnerGO.AddComponent<ObstacleSpawner>();

        var obstacleSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Obstacle.png");
        GameObject obstacle = new GameObject("ObstaclePrefab");
        var obsSr = obstacle.AddComponent<SpriteRenderer>();
        if (obstacleSprite != null) obsSr.sprite = obstacleSprite;
        obstacle.AddComponent<BoxCollider2D>();
        obstacle.AddComponent<ObstacleMover>();
        string prefabPath = "Assets/Prefabs/Obstacle.prefab";
        PrefabUtility.SaveAsPrefabAsset(obstacle, prefabPath);
        GameObject.DestroyImmediate(obstacle);
        spawner.obstaclePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        var gmGO = new GameObject("GameManager");
        var gm = gmGO.AddComponent<GameManager>();
        gm.spawner = spawner;

        var canvasGO = CreateCanvas();
        var uiManager = canvasGO.AddComponent<UIManager>();
        GameObject timerText = CreateUIText("TimerText", canvasGO.transform, "Time: 30s", new Vector2(0.0f, 180f));
        uiManager.timerText = timerText.GetComponent<UnityEngine.UI.Text>();
        GameObject p1Text = CreateUIText("P1ScoreText", canvasGO.transform, "P1: 0", new Vector2(-300f, 180f));
        uiManager.p1ScoreText = p1Text.GetComponent<UnityEngine.UI.Text>();
        GameObject p2Text = CreateUIText("P2ScoreText", canvasGO.transform, "P2: 0", new Vector2(-150f, 180f));
        uiManager.p2ScoreText = p2Text.GetComponent<UnityEngine.UI.Text>();
        GameObject totalText = CreateUIText("TotalScoreText", canvasGO.transform, "Total: 0", new Vector2(200f, 180f));
        uiManager.totalScoreText = totalText.GetComponent<UnityEngine.UI.Text>();
        GameObject resultText = CreateUIText("ResultText", canvasGO.transform, "", new Vector2(0f, -150f));
        uiManager.resultText = resultText.GetComponent<UnityEngine.UI.Text>();
        GameObject diffText = CreateUIText("DifficultyText", canvasGO.transform, gm.difficulty.ToString(), new Vector2(300f, 180f));
        uiManager.difficultyText = diffText.GetComponent<UnityEngine.UI.Text>();

        gm.uiManager = uiManager;

        if (GameObject.FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        var leftButton = CreateButton("P1_Jump_Button", canvasGO.transform, new Vector2(-200f, -140f), new Vector2(180, 180));
        var rightButton = CreateButton("P2_Jump_Button", canvasGO.transform, new Vector2(200f, -140f), new Vector2(180, 180));

        var p1controller = player1.GetComponent<PlayerController>();
        var p2controller = player2.GetComponent<PlayerController>();
        var leftBtnComp = leftButton.GetComponent<UnityEngine.UI.Button>();
        var rightBtnComp = rightButton.GetComponent<UnityEngine.UI.Button>();
        UnityEventTools.AddPersistentListener(leftBtnComp.onClick, new UnityEngine.Events.UnityAction(p1controller.Jump));
        UnityEventTools.AddPersistentListener(rightBtnComp.onClick, new UnityEngine.Events.UnityAction(p2controller.Jump));

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainScene.unity");
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("RunningJump", "MainScene generated and saved to Assets/Scenes/MainScene.unity", "OK");
    }

    static GameObject CreatePlayer(string name, Vector3 pos, int id, KeyCode key, string spritePath)
    {
        GameObject go = new GameObject(name);
        var sr = go.AddComponent<SpriteRenderer>();
        var spr = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (spr != null) sr.sprite = spr;
        go.transform.position = pos;
        var rb = go.AddComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        var col = go.AddComponent<BoxCollider2D>();
        var pc = go.AddComponent<PlayerController>();
        pc.playerId = id;
        pc.jumpKey = key;
        pc.groundLayer = LayerMask.GetMask("Default");
        go.tag = name;
        string prefabPath = $"Assets/Prefabs/{name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        return go;
    }

    static GameObject CreateCanvas()
    {
        GameObject canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<UnityEngine.Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        return canvasGO;
    }

    static GameObject CreateUIText(string name, Transform parent, string text, Vector2 anchoredPos)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        var txt = go.AddComponent<UnityEngine.UI.Text>();
        txt.text = text;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = 24;
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(400, 50);
        rt.anchoredPosition = anchoredPos;
        return go;
    }

    static GameObject CreateButton(string name, Transform parent, Vector2 anchoredPos, Vector2 size)
    {
        GameObject btnGO = new GameObject(name);
        btnGO.transform.SetParent(parent);
        var img = btnGO.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(1f,1f,1f,0.25f);
        var btn = btnGO.AddComponent<UnityEngine.UI.Button>();
        var rt = btnGO.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;
        return btnGO;
    }
}
#endif
