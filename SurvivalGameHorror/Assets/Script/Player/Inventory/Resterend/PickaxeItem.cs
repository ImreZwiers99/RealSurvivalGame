using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeItem : MonoBehaviour
{
    [SerializeField] private int rockDamage = 3;
    [SerializeField] private float usageCooldown = 1f;
    private float lastUsageTime = 0f;
    public static bool canSwing = false;
    private bool fix = false;
    public Animator animator;
    public AudioSource hitSound;

    private void Update()
    {
        if (Time.time - lastUsageTime >= usageCooldown && canSwing && FirstPersonController.isMenuActive == false
            && Inventory.isOpen == false && !fix && FirstPersonController.isCrouching == false)
        {
            FirstPersonController.canMove = true;
            animator.SetBool("PickSwing", false);
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 2f))
                {
                    RockHealth rockHealth = hit.collider.GetComponent<RockHealth>();
                    if (rockHealth != null)
                    {
                        FirstPersonController.canMove = false;
                        animator.SetBool("PickSwing", true);
                        StartCoroutine(DelayedDamage(rockHealth));
                        lastUsageTime = Time.time;
                    }
                    else
                    {
                        animator.SetBool("PickAttack", true);
                        StartCoroutine(AxeAttackDelay());
                        fix = true;
                    }
                }
                else
                {
                    animator.SetBool("PickAttack", true);
                    StartCoroutine(AxeAttackDelay());
                    fix = true;
                }
            }
        }
    }

    private IEnumerator AxeAttackDelay()
    {
        yield return new WaitForSeconds(1);
        animator.SetBool("PickAttack", false);
        fix = false;
    }

    private IEnumerator DelayedDamage(RockHealth rock)
    {
        hitSound.Play();
        yield return new WaitForSeconds(0.4f);
        //particle.SetActive(true);
        rock.TakeDamage(rockDamage, transform.root.gameObject);
        Debug.Log("Rock took damage");
    }
}
