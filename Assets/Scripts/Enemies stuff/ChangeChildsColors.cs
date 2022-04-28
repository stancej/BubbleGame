using UnityEngine;

public class ChangeChildsColors : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] arrOfChildsRenderes;
    [SerializeField] private SpriteRenderer ownRendererComp;
    private void Update()
    {

        if (arrOfChildsRenderes.Length > 0)
        {
            if (ownRendererComp.material.color != arrOfChildsRenderes[0].material.color)
            {
                foreach(SpriteRenderer sprite in arrOfChildsRenderes)
                {
                    sprite.material.color = ownRendererComp.material.color;
                }
            }
        }
    }
}
