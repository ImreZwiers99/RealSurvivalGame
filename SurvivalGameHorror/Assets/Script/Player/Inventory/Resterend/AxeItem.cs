using System.Collections;
using UnityEngine;

public class AxeItem : MonoBehaviour
{
    [SerializeField] private int axeDamage = 3;
    [SerializeField] private float usageCooldown = 1f;
    private float lastUsageTime = 0f;
    public static bool canSwing = false;
    private bool fix = false;
    //public GameObject particle;
    public Animator animator;
    public AudioSource chopSound;

    private void Update()
    {
        if (Time.time - lastUsageTime >= usageCooldown && canSwing && FirstPersonController.isMenuActive == false
            && Inventory.isOpen == false && !fix && FirstPersonController.isCrouching == false)
        {
            //particle.SetActive(false);
            FirstPersonController.canMove = true;
            animator.SetBool("AxeSwing", false);
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 2f))
                {
                    TreeHealth treeHealth = hit.collider.GetComponent<TreeHealth>();
                    if (treeHealth != null)
                    {
                        
                        FirstPersonController.canMove = false;
                        animator.SetBool("AxeSwing", true);
                        StartCoroutine(DelayedDamage(treeHealth));
                        lastUsageTime = Time.time;
                    }
                    else
                    {
                        animator.SetBool("AxeAttack", true);
                        StartCoroutine(AxeAttackDelay());
                        fix = true;
                    }
                }
                else
                {
                    animator.SetBool("AxeAttack", true);
                    StartCoroutine(AxeAttackDelay());
                    fix = true;
                }
            }
        }
    }

    private IEnumerator AxeAttackDelay()
    {
        yield return new WaitForSeconds(1);
        animator.SetBool("AxeAttack", false);
        fix = false;
    }

    private IEnumerator DelayedDamage(TreeHealth tree)
    {
        chopSound.Play();
        yield return new WaitForSeconds(0.4f);
        //particle.SetActive(true);
        tree.TakeDamage(axeDamage, transform.root.gameObject);
        Debug.Log("Tree took damage");
    }
}
