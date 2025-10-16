
using UnityEngine;

public enum GameState { Menu, Playing, GameOver }

public class GameManager : MonoBehaviour
{
    public GameState State = GameState.Menu;
    public float GameDuration = 30f;

    float timeLeft;
    int coins;
    float distance;
    int level = 1;

    PlayerController player;
    ObstacleSpawner obstacleSpawner;
    CoinSpawner coinSpawner;
    Camera cam;

    float baseSpeed, obstacleRate, coinRate, gravityScale;

    void Awake()
    {
        BuildCamera();
        BuildWorld();
        GoMenu();
    }

    void BuildCamera()
    {
        if (Camera.main == null)
        {
            var camGO = new GameObject("Main Camera");
            cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.86f, 0.94f, 1f);
            camGO.tag = "MainCamera";
            camGO.transform.position = new Vector3(0, 0, -10);
        }
        else cam = Camera.main;
    }

    void BuildWorld()
    {
        // Ground
        var ground = GameObject.Find("Ground");
        if (ground == null)
        {
            ground = new GameObject("Ground");
            var sr = ground.AddComponent<SpriteRenderer>();
            sr.sprite = MakeSprite(1024, 64, new Color(0.36f, 0.8f, 0.45f));
            ground.transform.position = new Vector3(0, -3.5f, 0);
            ground.AddComponent<BoxCollider2D>();
        }

        // Player (single instance)
        var playerGO = GameObject.Find("Player");
        if (playerGO == null)
        {
            playerGO = new GameObject("Player");
            player = playerGO.AddComponent<PlayerController>();
            var sr = playerGO.AddComponent<SpriteRenderer>();
            sr.sprite = MakeDinoSprite();
            var rb = playerGO.AddComponent<Rigidbody2D>();
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 4f;
            playerGO.AddComponent<BoxCollider2D>();
            playerGO.transform.position = new Vector3(-4, -2.8f, 0);
        }
        else player = playerGO.GetComponent<PlayerController>();

        // Spawners (auto-create if missing)
        var obs = GameObject.Find("ObstacleSpawner");
        if (obs == null) obs = new GameObject("ObstacleSpawner");
        obstacleSpawner = obs.GetComponent<ObstacleSpawner>() ?? obs.AddComponent<ObstacleSpawner>();

        var coin = GameObject.Find("CoinSpawner");
        if (coin == null) coin = new GameObject("CoinSpawner");
        coinSpawner = coin.GetComponent<CoinSpawner>() ?? coin.AddComponent<CoinSpawner>();
    }

    Sprite MakeSprite(int w,int h,Color c)
    {
        Texture2D tex = new Texture2D(w, h);
        Color[] px = new Color[w*h];
        for (int i = 0; i < px.Length; i++) px[i] = c;
        tex.SetPixels(px); tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(.5f, .5f), 100);
    }

    Sprite MakeDinoSprite()
    {
        int w = 64, h = 64;
        Texture2D tex = new Texture2D(w, h);
        Color bg = new Color(0,0,0,0);
        Color body = new Color(0.15f, 0.15f, 0.15f);
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) tex.SetPixel(x, y, bg);
        for (int y = 20; y < 40; y++) for (int x = 10; x < 34; x++) tex.SetPixel(x, y, body);
        for (int y = 34; y < 50; y++) for (int x = 28; x < 46; x++) tex.SetPixel(x, y, body);
        for (int y = 12; y < 20; y++) for (int x = 8; x < 24; x++) tex.SetPixel(x, y, body);
        for (int y = 0; y < 12; y++) for (int x = 14; x < 20; x++) tex.SetPixel(x, y, body);
        for (int y = 0; y < 12; y++) for (int x = 24; x < 30; x++) tex.SetPixel(x, y, body);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0,0,w,h), new Vector2(.5f, .25f), 64);
    }

    void SetDifficulty(int lv)
    {
        float diff = Mathf.Clamp(lv, 1, 20);
        baseSpeed = 6f + (diff - 1f) * 0.6f;
        obstacleRate = Mathf.Max(1.6f - (diff - 1f) * 0.1f, 0.5f);
        coinRate = Mathf.Max(1.2f - (diff - 1f) * 0.05f, 0.4f);
        gravityScale = 3.8f + (diff - 1f) * 0.12f;
        if (player != null) player.SetMoveSpeed(baseSpeed);
    }

    void GoMenu() { State = GameState.Menu; Time.timeScale = 0f; }

    void StartGame()
    {
        // Clear old items
        foreach (var o in GameObject.FindObjectsOfType<Obstacle>()) Destroy(o.gameObject);
        foreach (var c in GameObject.FindObjectsOfType<Coin>()) Destroy(c.gameObject);

        SetDifficulty(level);
        coins = 0; distance = 0f; timeLeft = GameDuration;
        State = GameState.Playing; Time.timeScale = 1f;

        if (player == null) player = FindObjectOfType<PlayerController>();
        player.ResetPlayer(new Vector3(-4, -2.8f, 0), gravityScale);

        obstacleSpawner.Begin(baseSpeed, obstacleRate);
        coinSpawner.Begin(baseSpeed, coinRate);
    }

    public void GameOver()
    {
        if (State != GameState.Playing) return;
        State = GameState.GameOver;
        Time.timeScale = 0f;
    }

    public void AddCoin(int n = 1) { coins += n; }

    void Update()
    {
        // Start from menu on any press
        if (State == GameState.Menu && (Input.GetMouseButtonDown(0) || Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space)))
        {
            StartGame(); return;
        }

        if (State == GameState.Playing)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f) GameOver();
            distance += player.CurrentSpeed * Time.deltaTime;
        }
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        GUIStyle title = new GUIStyle(GUI.skin.label){ alignment = TextAnchor.MiddleCenter, fontSize = (int)(h*0.08f), fontStyle = FontStyle.Bold };
        GUIStyle center = new GUIStyle(GUI.skin.label){ alignment = TextAnchor.MiddleCenter, fontSize = (int)(h*0.05f) };
        GUIStyle small = new GUIStyle(GUI.skin.label){ alignment = TextAnchor.MiddleCenter, fontSize = (int)(h*0.04f) };

        if (State == GameState.Menu)
        {
            GUI.Label(new Rect(0, h*0.12f, w, h*0.12f), "Dino Run", title);
            GUI.Label(new Rect(0, h*0.24f, w, h*0.05f), "Tap Start or press Space", small);
            GUI.Label(new Rect(0, h*0.29f, w, h*0.05f), "Double Jump: press twice", small);
            if (GUI.Button(new Rect(w*0.35f, h*0.45f, w*0.3f, h*0.1f), "Start Game")) StartGame();
        }
        else if (State == GameState.Playing)
        {
            GUI.Label(new Rect(10, 10, 200, 40), "Time: " + timeLeft.ToString("0.0"), small);
            GUI.Label(new Rect(w-220, 10, 200, 40), "Score: " + ((int)distance) + "  C:" + coins, small);
            GUI.Label(new Rect(0, 50, w, 30), "Level " + level, small);
        }
        else if (State == GameState.GameOver)
        {
            string res = "Distance: " + distance.ToString("0") + "\nCoins: " + coins + "\nToken: " + (distance * 0.01f + coins * 0.5f).ToString("0.0");
            GUI.Box(new Rect(w*0.25f, h*0.28f, w*0.5f, h*0.4f), "Game Over");
            GUI.Label(new Rect(0, h*0.36f, w, 100), res, center);
            if (GUI.Button(new Rect(w*0.3f, h*0.60f, w*0.18f, h*0.08f), "Retry")) { level = 1; StartGame(); }
            if (GUI.Button(new Rect(w*0.52f, h*0.60f, w*0.18f, h*0.08f), "Next Level")) { level++; StartGame(); }
            if (GUI.Button(new Rect(w*0.4f, h*0.70f, w*0.2f, h*0.08f), "Main Menu")) { GoMenu(); }
        }
    }
}
