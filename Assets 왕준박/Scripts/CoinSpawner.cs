
using UnityEngine;
public class Coin : MonoBehaviour
{
    float speed; public void Init(float s){ speed = s; }
    void Update(){ transform.position += Vector3.left * speed * Time.deltaTime;
        if (transform.position.x < -12f) Destroy(gameObject); }
}
public class CoinSpawner : MonoBehaviour
{
    float speed = 8f, rate = 1.0f, timer = 0f;
    public void Begin(float spd, float spawnRate){ speed = spd; rate = spawnRate; timer = 0f; }
    void Update()
    {
        if (Time.timeScale == 0f) return;
        timer -= Time.deltaTime;
        if (timer <= 0f){ Spawn(); timer = rate + Random.Range(-0.2f, 0.2f); }
    }
    void Spawn()
    {
        GameObject g = new GameObject("Coin");
        var sr = g.AddComponent<SpriteRenderer>();
        sr.sprite = MakeSprite(24, 24, new Color(1f, 0.84f, 0.1f));
        g.AddComponent<CircleCollider2D>().isTrigger = true;
        var c = g.AddComponent<Coin>(); c.Init(speed);
        float y = Random.Range(-2.4f, 1.5f);
        g.transform.position = new Vector3(10.5f, y, 0);
    }
    Sprite MakeSprite(int w,int h,Color c){
        Texture2D t=new Texture2D(w,h); Color[]px=new Color[w*h];
        for(int i=0;i<px.Length;i++) px[i]=c; t.SetPixels(px); t.Apply();
        return Sprite.Create(t,new Rect(0,0,w,h),new Vector2(.5f,.5f),100);
    }
}
