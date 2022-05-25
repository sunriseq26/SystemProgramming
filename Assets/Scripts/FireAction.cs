using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;

public abstract class FireAction : NetworkBehaviour
{
    public const int DAMAGE = 10;
    
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private int startAmmunition = 20;

    public string bulletCount { get; protected internal set; } = string.Empty;
    public Queue<GameObject> Bullets { get; private set; } = new Queue<GameObject>();
    public Queue<GameObject> Ammunition { get; private set; } = new Queue<GameObject>();
    protected bool reloading = false;

    protected PlayerCharacter character;

    protected virtual void Start()
    {
        for (var i = 0; i < startAmmunition; i++)
        {
            GameObject bullet;
            if (bulletPrefab == null)
            {
                bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            else
            {
                bullet = Instantiate(bulletPrefab);
            }
            bullet.SetActive(false);
            Ammunition.Enqueue(bullet);
        }
    }

    public void Init(PlayerCharacter playerCharacter)
    {
        character = playerCharacter;
    }

    public virtual async void Reloading()
    {
        Bullets = await Reload();
    }

    protected virtual void Shooting()
    {
        if (Bullets.Count == 0)
        {
            Reloading();
        }
    }
    
    private async Task<Queue<GameObject>> Reload()
    {
        if (!reloading)
        {
            reloading = true;
            StartCoroutine(ReloadingAnim());
            return await Task.Run(delegate
            {
                var cage = 10;
                if (Bullets.Count < cage)
                {
                    Thread.Sleep(3000);
                    var bullets = this.Bullets;
                    while (bullets.Count > 0)
                    {
                        Ammunition.Enqueue(bullets.Dequeue());
                    }
                    cage = Mathf.Min(cage, Ammunition.Count);
                    if (cage > 0)
                    {
                        for (var i = 0; i < cage; i++)
                        {
                            var sphere = Ammunition.Dequeue();
                            bullets.Enqueue(sphere);
                        }
                    }
                }
                reloading = false;
                return Bullets;
            });
        }
        else
        {
            return Bullets;
        }
    }

    private IEnumerator ReloadingAnim()
    {
        while (reloading)
        {
            bulletCount = " | ";
            yield return new WaitForSeconds(0.01f);
            bulletCount = @" \ ";
            yield return new WaitForSeconds(0.01f);
            bulletCount = "---";
            yield return new WaitForSeconds(0.01f);
            bulletCount = " / ";
            yield return new WaitForSeconds(0.01f);
        }
        bulletCount = Bullets.Count.ToString();
        yield return null;
    }
}

