using UnityEngine;

using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class ImpactReceiver : MonoBehaviour
{
    float mass = 1.0F; // defines the character mass
    public Vector3 impact = Vector3.zero;
    private CharacterController character;

    Vector3 velocity = Vector3.zero;
    [SerializeField] float smoothTime = 1f;

    // Use this for initialization
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // apply the impact force:
        if (impact.magnitude > 0.2F)
        {
            character.Move(impact * Time.deltaTime);
        }
        else
        {
            impact = Vector3.zero;
        }
        // consumes the impact energy each cycle:
        //impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);


        //alternativa
        impact = Vector3.SmoothDamp(impact, Vector3.zero, ref velocity, smoothTime);

    }
    // call this function to add an impact force:
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

}
