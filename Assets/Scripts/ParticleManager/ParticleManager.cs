using UnityEngine;

public class ParticleManager : MonoBehaviour {
    public static ParticleManager Instance { get; private set; }

    [SerializeField] public GameObject impactParticle;
    [SerializeField] public GameObject dustParticle;
    [SerializeField] public GameObject tetherParticle;
    [SerializeField] public GameObject infernoParticle;
    [SerializeField] public GameObject terminatorParticle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null) {
            Instance = this;
        }else {
            Destroy(gameObject);
        }
    }

    public void spawnParticle(GameObject particle, Vector3 pos, Quaternion rot) {
        Instantiate(particle, pos, rot, transform);
    }
}
