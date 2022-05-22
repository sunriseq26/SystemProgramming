using System.Collections;
using UnityEngine;

public class RayShooter : FireAction
{
    private Camera camera;

    protected override void Start()
    {
        base.Start();
        camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shooting();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reloading();
        }

        if (Input.anyKey && !Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    protected override void Shooting()
    {
        base.Shooting();
        if (bullets.Count > 0)
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        if (reloading)
        {
            yield break;
        }
        
        var point = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0);
        var ray = camera.ScreenPointToRay(point);
        var hits = Physics.RaycastAll(ray);
        
        Debug.Log(hits.Length);

        for (int i = 0; i < hits.Length - 1; i++)
        {
            if (!hits[i].transform)
            {
                yield break;
            }


            var shoot = bullets.Dequeue();
            bulletCount = bullets.Count.ToString();
            ammunition.Enqueue(shoot);
            shoot.SetActive(true);
            shoot.transform.position = hits[i].point;
            shoot.transform.parent = hits[i].transform;

            yield return new WaitForSeconds(2.0f);
            shoot.SetActive(false);
        }
        
    }
}

