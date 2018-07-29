using UnityEngine;
using System.Collections;

public class ParticleSystemAutoDestroy : MonoBehaviour
{

    [SerializeField] private UnityEngine.Light Light;
    [SerializeField] private UnityEngine.ParticleSystem ParticleSystem;

    public void Start()
    {
        ParticleSystem = GetComponent<UnityEngine.ParticleSystem>();
        StartCoroutine("CloseLight");
    }

    public void Update()
    {
        if (ParticleSystem)
        {
            if (!ParticleSystem.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator CloseLight()
    {
        float waitCounter = 0;
        float _intensity = Light.intensity;
        while (waitCounter < ParticleSystem.main.duration)
        {
            waitCounter += Time.fixedDeltaTime;
            Light.intensity = _intensity - _intensity * (waitCounter / ParticleSystem.main.duration);
            //Yield until the next frame
            yield return null;
        }
    }

}