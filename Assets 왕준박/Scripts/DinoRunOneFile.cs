using UnityEngine;

public class DinoRunOneFile : MonoBehaviour
{
    public float gameDuration = 30f;
    public bool autoStartIfInput = true;
    public bool failSafeAutoStart = true;
    public float failSafeDelay = 1.0f;

    enum State { Menu, Playing, GameOver }
    State state = State.Menu;
    float timeLeft, distance; int coins, level = 1;
    GameObject player; Rigidbody2D rb;
    int jumpCount; const int maxJump =999;
    float moveSpeed = 6f, gravityScale = 4f, obstacleRate = 1.2f, coinRate = 1.0f;
    float failSafeTimer = 0f; Camera cam;
    float obstacleTimer = 0f, coinTimer = 0f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (FindObjectOfType<DinoRunOneFile>() == null)
        {
            var go = new GameObject("DinoRun_OneFile");
            go.AddComponent<DinoRunOneFile>();
        }
    }

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
        var ground = GameObject.Find("Ground");
        if (ground == null)
        {
            ground = new GameObject("Ground");
            var sr = ground.AddComponent<SpriteRenderer>();
            sr.sprite = MakeSprite(1024, 64, new Color(0.36f, 0.8f, 0.45f));
            ground.transform.position = new Vector3(0, -3.5f, 0);
            ground.AddComponent<BoxCollider2D>();
        }

        var exist = GameObject.Find("Player");
        if (exist != null) Destroy(exist);
        player = new GameObject("Player");
        player.AddComponent<SpriteRenderer>().sprite = MakeDinoSprite();
        player.AddComponent<BoxCollider2D>();
        rb = player.AddComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = gravityScale;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        player.layer = LayerMask.NameToLayer("Default");
        player.transform.position = new Vector3(-4, -2.8f, 0);


        if (GameObject.Find("ObstacleHost") == null) new GameObject("ObstacleHost");
        if (GameObject.Find("CoinHost") == null) new GameObject("CoinHost");
    }

    void SetDifficulty(int lv)
    {
        float diff = Mathf.Clamp(lv, 1, 20);
        moveSpeed = 6f + (diff - 1f) * 0.6f;
        obstacleRate = Mathf.Max(1.6f - (diff - 1f) * 0.1f, 0.5f);
        coinRate = Mathf.Max(1.2f - (diff - 1f) * 0.05f, 0.4f);
        gravityScale = 3.8f + (diff - 1f) * 0.12f;
    }

    void GoMenu() { state = State.Menu; Time.timeScale = 0f; failSafeTimer = 0f; }

    void StartGame()
    {
        var oh = GameObject.Find("ObstacleHost");
        var ch = GameObject.Find("CoinHost");
        if (oh != null) foreach (Transform t in oh.transform) Destroy(t.gameObject);
        if (ch != null) foreach (Transform t in ch.transform) Destroy(t.gameObject);

        SetDifficulty(level);
        coins = 0; distance = 0f; timeLeft = gameDuration;
        state = State.Playing; Time.timeScale = 1f;
        if (player == null) BuildWorld();
        rb = player.GetComponent<Rigidbody2D>();
        player.transform.position = new Vector3(-4, -2.8f, 0);
        rb.velocity = Vector2.zero; rb.gravityScale = gravityScale; jumpCount = 0;
        obstacleTimer = 0f; coinTimer = 0f;
    }

    public void GameOver()
    {
        if (state != State.Playing) return;
        state = State.GameOver;
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (state == State.Menu)
        {
            if (autoStartIfInput && (Input.GetMouseButtonDown(0) || Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space)))
            {
                StartGame(); return;
            }
            if (failSafeAutoStart)
            {
                failSafeTimer += Time.unscaledDeltaTime;
                if (failSafeTimer >= failSafeDelay) { StartGame(); return; }
            }
        }
        else if (state == State.Playing)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f) GameOver();
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space))
                TryJump();
            if (rb.velocity.y > 0 && (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space)))
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            if (player.transform.position.y < -6f) GameOver();
            distance += moveSpeed * Time.deltaTime;
            SpawnUpdate();
        }
    }

    void TryJump()
    {
        if (jumpCount < maxJump)
        {
            float force = (jumpCount == 0) ? 9.5f : 9.5f * 0.85f;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // 只有撞到地面（Y方向）才重置跳跃次数
        if (!other.collider.isTrigger && other.contacts[0].normal.y > 0.5f)
        {
            jumpCount = 0;   // ✅ 落地后才能再次起跳
        }

        if (other.collider.GetComponent<ObstacleMarker>() != null)
        {
            GameOver();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var c = col.GetComponent<CoinMarker>();
        if (c != null)
        {
            coins += 1;
            Destroy(c.gameObject);
        }
    }

    void SpawnUpdate()
    {
        obstacleTimer -= Time.deltaTime;
        coinTimer -= Time.deltaTime;
        if (obstacleTimer <= 0f) { SpawnObstacle(); obstacleTimer = obstacleRate + Random.Range(-0.25f, 0.25f); }
        if (coinTimer <= 0f) { SpawnCoin(); coinTimer = coinRate + Random.Range(-0.2f, 0.2f); }
    }

    void SpawnObstacle()
    {
        var host = GameObject.Find("ObstacleHost");
        var g = new GameObject("Obstacle");
        if (host) g.transform.SetParent(host.transform);
        var sr = g.AddComponent<SpriteRenderer>();
        sr.sprite = MakeSprite(40, 60, new Color(0.12f, 0.6f, 0.13f));
        g.AddComponent<BoxCollider2D>();
        g.AddComponent<ObstacleMarker>();
        float y = Random.value > 0.7f ? -1.2f : -2.9f;
        g.transform.position = new Vector3(10.5f, y, 0);
        g.AddComponent<MoveLeft>().Init(moveSpeed);
    }

    void SpawnCoin()
    {
        var host = GameObject.Find("CoinHost");
        var g = new GameObject("Coin");
        if (host) g.transform.SetParent(host.transform);
        var sr = g.AddComponent<SpriteRenderer>();
        sr.sprite = MakeSprite(24, 24, new Color(1f, 0.84f, 0.1f));
        var c = g.AddComponent<CircleCollider2D>();
        c.isTrigger = true;
        g.AddComponent<CoinMarker>();

        // ✅ 加入 Rigidbody2D 以启用触发检测
        g.AddComponent<Rigidbody2D>().gravityScale = 0;

        float y = Random.Range(-2.4f, 1.5f);
        g.transform.position = new Vector3(10.5f, y, 0);
        g.AddComponent<MoveLeft>().Init(moveSpeed);
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle title = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = (int)(h * 0.08f),
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.black }
        };

        GUIStyle small = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = (int)(h * 0.04f),
            normal = { textColor = Color.black }
        };

        GUIStyle center = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = (int)(h * 0.05f),
            normal = { textColor = Color.black }
        };

        // --- MENU ---
        if (state == State.Menu)
        {
            GUI.Label(new Rect(0, h * 0.12f, w, h * 0.12f), "Dino Run", title);
            GUI.Label(new Rect(0, h * 0.24f, w, h * 0.05f), "Tap Start or press Space", small);
            GUI.Label(new Rect(0, h * 0.29f, w, h * 0.05f), "Double Jump: press twice", small);
            if (GUI.Button(new Rect(w * 0.35f, h * 0.45f, w * 0.3f, h * 0.1f), "Start Game"))
            {
                StartGame();
            }
        }

        // --- PLAYING ---
        else if (state == State.Playing)
        {
            // 左上角显示时间
            GUI.Label(new Rect(20, 20, 200, 40), $"⏱ Time: {timeLeft:0.0}", small);
            // 右上角显示得分与金币
            GUI.Label(new Rect(w - 220, 20, 200, 40), $"🏃 {distance:0}   💰 {coins}", small);
            // 居中显示关卡
            GUI.Label(new Rect(0, 60, w, 30), $"Level {level}", small);
        }

        // --- GAME OVER ---
        else if (state == State.GameOver)
        {
            GUI.Box(new Rect(w * 0.25f, h * 0.28f, w * 0.5f, h * 0.4f), "Game Over");
            string result = $"Distance: {distance:0}\nCoins: {coins}\nToken: {(distance * 0.01f + coins * 0.5f):0.0}";
            GUI.Label(new Rect(0, h * 0.36f, w, 100), result, center);

            if (GUI.Button(new Rect(w * 0.3f, h * 0.60f, w * 0.18f, h * 0.08f), "Retry"))
            {
                level = 1;
                StartGame();
            }

            if (GUI.Button(new Rect(w * 0.52f, h * 0.60f, w * 0.18f, h * 0.08f), "Next Level"))
            {
                level++;
                StartGame();
            }

            if (GUI.Button(new Rect(w * 0.4f, h * 0.70f, w * 0.2f, h * 0.08f), "Main Menu"))
            {
                GoMenu();
            }
        }
    }

    // --- 辅助类与绘制 ---
    Sprite MakeSprite(int w, int h, Color c)
    {
        Texture2D tex = new Texture2D(w, h);
        Color[] px = new Color[w * h];
        for (int i = 0; i < px.Length; i++) px[i] = c;
        tex.SetPixels(px); tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(.5f, .5f), 100);
    }

    Sprite MakeDinoSprite()
    {
        int w = 64, h = 64;
        Texture2D tex = new Texture2D(w, h);
        Color bg = new Color(0, 0, 0, 0), body = new Color(0.15f, 0.15f, 0.15f);
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) tex.SetPixel(x, y, bg);
        for (int y = 20; y < 40; y++) for (int x = 10; x < 34; x++) tex.SetPixel(x, y, body);
        for (int y = 34; y < 50; y++) for (int x = 28; x < 46; x++) tex.SetPixel(x, y, body);
        for (int y = 12; y < 20; y++) for (int x = 8; x < 24; x++) tex.SetPixel(x, y, body);
        for (int y = 0; y < 12; y++) for (int x = 14; x < 20; x++) tex.SetPixel(x, y, body);
        for (int y = 0; y < 12; y++) for (int x = 24; x < 30; x++) tex.SetPixel(x, y, body);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(.5f, .25f), 64);
    }

    class MoveLeft : MonoBehaviour
    {
        float speed = 8f;
        public void Init(float s) { speed = s; }
        void Update()
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
            if (transform.position.x < -12f) Destroy(gameObject);
        }
    }

    class ObstacleMarker : MonoBehaviour { }
    class CoinMarker : MonoBehaviour { }
}
