using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public void destroyHeart() {
        animator.Play("Destroy");
    }

    public void onHeartAnimationComplete() {
        Destroy(gameObject);
    }
}
