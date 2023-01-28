using Mirror;
using UnityEngine;

public class gunshooter : MonoBehaviour
{

    [SerializeField]private GameObject bulletPrefab;
    // Start is called before the first frame update


    // Update is called once per frame

    public void onShoot(PlayerBody instigator)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.LogError("SHOT");
            GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 target = transform.position + transform.forward * 50f;
            RaycastHit hit;
            b.GetComponent<BulletScript>().direction = target;
            Physics.Raycast(ray, out hit);
            if (hit.transform != null)
            {
                Debug.Log("Hit object -> " + hit.transform.name);

                PlayerBody p = hit.transform.GetComponent<PlayerBody>();
                if (p != null)
                {
                    Debug.Log("hit object was played. applying damage");
                }
            }
            else
            {
                Debug.Log("Skill Issue: Hit nothing.");
            }
        }
    }
}
