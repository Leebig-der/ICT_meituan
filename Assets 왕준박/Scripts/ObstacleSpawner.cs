
using UnityEngine;
public class ObstacleSpawner : MonoBehaviour
{
    float speed = 8f, rate = 1.2f, timer = 0f;
    public void Begin(float spd, float spawnRate){ speed = spd; rate = spawnRate; timer = 0f; }
    void Update()
    {
        if (Time.timeScale == 0f) return;
        timer -= Time.deltaTime;
        if (timer <= 0f){ Spawn(); timer = rate + Random.Range(-0.3f, 0.3f); }
    }
    void Spawn()
    {
        GameObject g = new GameObject("Obstacle");
        var sr = g.AddComponent<SpriteRenderer>();
        sr.sprite = MakeSprite(40, 60, new Color(0.12f, 0.6f, 0.13f));
        g.AddComponent<BoxCollider2D>();
        var o = g.AddComponent<Obstacle>(); o.Init(speed);
        float y = Random.value > 0.7f ? -1.2f : -2.9f;
        g.transform.position = new Vector3(10.5f, y, 0);
    }
    Sprite MakeSprite(int w,int h,Color c){
        Texture2D t=new Texture2D(w,h); Color[]px=new Color[w*h];
        for(int i=0;i<px.Length;i++) px[i]=c; t.SetPixels(px); t.Apply();
        return Sprite.Create(t,new Rect(0,0,w,h),new Vector2(.5f,.1f),100);
    }
}
