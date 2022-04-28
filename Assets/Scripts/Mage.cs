using UnityEngine;


public class Mage : LivingEntity
{


    [SerializeField] public Ray rayFromMainCameraToMousePos;
    [SerializeField] public Plane r_groundPlane;

    public Transform tower;

    public float damage;

    public static float mageAttackDamage { get; set; }

    protected override void Start()
    {
        base.Start();
    }

    public void Update()
    { 
        mageAttackDamage = damage;
        //look input
        rayFromMainCameraToMousePos = Camera.main.ScreenPointToRay(Input.mousePosition); // create ray from main camera to player mouse position
        r_groundPlane = new Plane(Vector3.one, Vector3.forward); //create plane for the end of ray
        float rayDistance;

        if (r_groundPlane.Raycast(rayFromMainCameraToMousePos, out rayDistance))
        {
            //calculate mouse position
            Vector3 point = rayFromMainCameraToMousePos.GetPoint(rayDistance);
            Vector3 heightCorrectedPoint = new Vector3(point.x, point.y, transform.position.z);

            //change side regarding mouse position
            if (heightCorrectedPoint.x < 0)
            {
                transform.Find("Mage").rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.Find("Mage").rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }
}
