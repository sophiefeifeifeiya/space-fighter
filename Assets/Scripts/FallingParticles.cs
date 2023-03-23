using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType{
    Red = 1,
    Green = 2,
    Blue = 3,
    Gray = 0

}

public class FallingParticles : MonoBehaviour
{
    // Start is called before the first frame update
    public float fallingSpeedMax = -2.0f;
    private float speedDecreaseRate = 0.95f;
    private float diameter = 1.0f;
    public Rigidbody2D rigidBody;
    public FallingParticles instance;
    public ParticleType particleType;

    public bool touchPlayer = false;

    // private float fix_X = 5.0f;
    // private bool active = true;

    // private Vector2 lastPosition;

    void Awake() {
        instance = this;
        rigidBody = GetComponent<Rigidbody2D>();
        while (setParticleType() == ParticalGenerator.instance.lastParticalTypeGenerated);
        // lastPosition = transform.position;
    }
    void Start()
    {
        rigidBody.velocity = new Vector2(0, fallingSpeedMax);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        //limit the maxspeed

        var v = new Vector2(rigidBody.velocity.x * speedDecreaseRate, fallingSpeedMax);
        rigidBody.velocity = Vector2.ClampMagnitude(v, -fallingSpeedMax);
        
        
    }


    private void OnCollisionEnter2D(Collision2D other) {
        // if the space fighter touches the debris on the top and then destory it
        if (other.gameObject.tag == "Player"){
            if (particleType == ParticleType.Gray){
                Vector2 thisPosition = rigidBody.position;
                Vector2 otherPosition = other.gameObject.transform.position;
                if (thisPosition.y > otherPosition.y && Mathf.Abs(thisPosition.x - otherPosition.x) <= Mathf.Abs(thisPosition.y - otherPosition.y)){
                    GameManager.instance.DHAdded();
                    ParticalGenerator.instance.destroyPartical(instance);
                }
            }
            touchPlayer = true;
        }else if (other.gameObject.tag == "GameElement"){
            if (other.gameObject.GetComponent<FallingParticles>().touchPlayer){
                // Opps! Space Fighter has been destroyed! in Red
                GameManager.instance.GameOver(false, "Opps! Space Fighter has been destroyed!", Color.red);
            }
        }

        if (other.gameObject.tag == "Bottom" && gameObject.tag != "Bottom"){
            gameObject.tag = "Bottom";
            // rigidBody.velocity = Vector2.zero;
            if (particleType == ParticleType.Gray){
                GameManager.instance.DebrisAdded();
            }else{
                checkDestroy();
            }
            
            if (rigidBody.position.y > -1.0f){
                // the storage area is filled with non-lined ionised particles; "Opps! Ionised particles are not successfully lined in the storage! " in Orange
                GameManager.instance.GameOver(false, "Opps! Ionised particles are not successfully lined in the storage!", new Color(1.0f, 0.5f, 0.0f));
            }
        }

    }

    private void OnCollisionExit2D(Collision2D other) {
        if (touchPlayer){
            touchPlayer = false;
        }
    }

    private ParticleType setParticleType(){
        switch (Random.Range(0,4)){
            case 0:
                particleType = ParticleType.Gray;
                gameObject.GetComponent<SpriteRenderer>().material.color = new Color32(128,128,128,255);
                break;
            case 1:
                particleType = ParticleType.Red;
                gameObject.GetComponent<SpriteRenderer>().material.color = Color.red;
                break;
            case 2:
                particleType = ParticleType.Green;
                gameObject.GetComponent<SpriteRenderer>().material.color = Color.green;
                break;
            case 3:
                particleType = ParticleType.Blue;
                gameObject.GetComponent<SpriteRenderer>().material.color = Color.blue;
                break;
            default:
                break;
        }
        return particleType;
    }

        private FallingParticles GetNeighbour(Vector2 startPoint, Vector2 direction, float length){
        // get the FallingPartical object for target direction with limited length.
        FallingParticles result = null;
        RaycastHit2D[] hitList= Physics2D.RaycastAll(startPoint + direction * length, direction, 0.1f);
        foreach (var i in hitList)
        {
            if (GameObject.ReferenceEquals(i.collider.gameObject, this)){
                continue;
            }
            FallingParticles script =  i.collider.gameObject.GetComponent<FallingParticles>();
            if (script != null){
                result = script;
            }
        }
        return result;
    }

    private void checkDestroy(){
            if (particleType == ParticleType.Red){
            // see if could destory
            Vector2 thisPosition = rigidBody.position;
            FallingParticles particleRight1 = GetNeighbour(thisPosition, Vector2.right, diameter);
            FallingParticles particleRight2 = GetNeighbour(thisPosition, Vector2.right, diameter*2);
            FallingParticles particleLeft1 = GetNeighbour(thisPosition, Vector2.left, diameter);
            FallingParticles particleLeft2 = GetNeighbour(thisPosition, Vector2.left, diameter*2);
            FallingParticles particleDown1 = GetNeighbour(thisPosition, Vector2.down, diameter);
            FallingParticles particleDown2 = GetNeighbour(thisPosition, Vector2.down, diameter*2);

            bool verticalRVanish = false;
            bool horizontalRL2L1Vanish = false;
            bool horizontalRR1L1Vanish = false;
            bool horizontalRR1R2Vanish = false;
            bool horizontalRGBVanish = false;
            

            if (particleDown1 != null && particleDown2 != null){
                if (particleDown1.particleType == ParticleType.Red && particleDown2.particleType == ParticleType.Red){
                    verticalRVanish = true;
                }
            }
            if (particleRight1 != null && particleRight2 != null){
                if (particleRight1.particleType == ParticleType.Green && particleRight2.particleType == ParticleType.Blue){
                    horizontalRGBVanish = true;
                }
                if (particleRight1.particleType == ParticleType.Red && particleRight2.particleType == ParticleType.Red){
                    horizontalRR1R2Vanish = true;
                }
            }
            if(particleRight1 != null && particleLeft1 != null){
                if (particleRight1.particleType == ParticleType.Red && particleLeft1.particleType == ParticleType.Red){
                    horizontalRR1L1Vanish = true;
                }
            }
            if(particleLeft2 != null && particleLeft1 != null){
                if (particleLeft2.particleType == ParticleType.Red && particleLeft1.particleType == ParticleType.Red){
                    horizontalRL2L1Vanish = true;
                }
            }
            // order: verticalRVanish,horizontalRL2L1Vanish,horizontalRR1L1Vanish,horizontalRR1R2Vanish,horizontalRGBVanish
            if(verticalRVanish){
                GameManager.instance.RAdded();
                ParticalGenerator.instance.destroyPartical(particleDown1);
                ParticalGenerator.instance.destroyPartical(particleDown2);
                ParticalGenerator.instance.destroyPartical(instance);
            }
            else if(horizontalRL2L1Vanish){
                GameManager.instance.RAdded();
                ParticalGenerator.instance.destroyPartical(particleLeft2);
                ParticalGenerator.instance.destroyPartical(particleLeft1);
                ParticalGenerator.instance.destroyPartical(instance);
            }
            else if(horizontalRR1L1Vanish){
                GameManager.instance.RAdded();
                ParticalGenerator.instance.destroyPartical(particleRight1);
                ParticalGenerator.instance.destroyPartical(particleLeft1);
                ParticalGenerator.instance.destroyPartical(instance);
            }
            else if (horizontalRR1R2Vanish){
                GameManager.instance.RAdded();
                ParticalGenerator.instance.destroyPartical(particleRight1);
                ParticalGenerator.instance.destroyPartical(particleRight2);
                ParticalGenerator.instance.destroyPartical(instance);
            }else if (horizontalRGBVanish){
                GameManager.instance.RGBAdded();
                ParticalGenerator.instance.destroyPartical(particleRight1);
                ParticalGenerator.instance.destroyPartical(particleRight2);
                ParticalGenerator.instance.destroyPartical(instance);
            }

        }
        else if (particleType == ParticleType.Green){
            // see if could destory
            Vector2 thisPosition = rigidBody.position;
            FallingParticles particleRight1 = GetNeighbour(thisPosition, Vector2.right, diameter);
            FallingParticles particleRight2 = GetNeighbour(thisPosition, Vector2.right, diameter*2);
            FallingParticles particleLeft1 = GetNeighbour(thisPosition, Vector2.left, diameter);
            FallingParticles particleLeft2 = GetNeighbour(thisPosition, Vector2.left, diameter*2);
            FallingParticles particleDown1 = GetNeighbour(thisPosition, Vector2.down, diameter);
            FallingParticles particleDown2 = GetNeighbour(thisPosition, Vector2.down, diameter*2);

            bool verticalGVanish = false;
            bool horizontalRGBVanish = false;
            bool horizontalGL1R1Vanish = false;
            bool horizontalGL1L2Vanish = false;
            bool horizontalGR1R2Vanish = false;


            if (particleDown1 != null && particleDown2 != null){
                if (particleDown1.particleType == ParticleType.Green && particleDown2.particleType == ParticleType.Green){
                    verticalGVanish = true;
                }
            }
            if(particleLeft2 != null && particleLeft1 != null){
                if (particleLeft2.particleType == ParticleType.Green && particleLeft1.particleType == ParticleType.Green){
                    horizontalGL1L2Vanish = true;
                }
            }
            if (particleLeft1 != null && particleRight1 != null){
                if (particleLeft1.particleType == ParticleType.Red && particleRight1.particleType == ParticleType.Blue){
                    horizontalRGBVanish = true;
                }
                if (particleLeft1.particleType == ParticleType.Green && particleRight1.particleType == ParticleType.Green){
                    horizontalGL1R1Vanish = true;
                }
            }

            if(particleRight1 != null && particleRight2 != null){
                if (particleRight1.particleType == ParticleType.Green && particleRight2.particleType == ParticleType.Green){
                    horizontalGR1R2Vanish = true;
                }
            }

            if (verticalGVanish){
                GameManager.instance.GAdded();
                ParticalGenerator.instance.destroyPartical(particleDown1);
                ParticalGenerator.instance.destroyPartical(particleDown2);
                ParticalGenerator.instance.destroyPartical(instance);
            }else if(horizontalRGBVanish){
                GameManager.instance.RGBAdded();
                ParticalGenerator.instance.destroyPartical(particleLeft1);
                ParticalGenerator.instance.destroyPartical(particleRight1);
                ParticalGenerator.instance.destroyPartical(instance);
            }else if(horizontalGL1L2Vanish){
                GameManager.instance.GAdded();
                ParticalGenerator.instance.destroyPartical(particleLeft2);
                ParticalGenerator.instance.destroyPartical(particleLeft1);
                ParticalGenerator.instance.destroyPartical(instance);
            }else if(horizontalGL1R1Vanish){
                GameManager.instance.GAdded();
                ParticalGenerator.instance.destroyPartical(particleLeft1);
                ParticalGenerator.instance.destroyPartical(particleRight1);
                ParticalGenerator.instance.destroyPartical(instance);
            }else if(horizontalGR1R2Vanish){
                GameManager.instance.GAdded();
                ParticalGenerator.instance.destroyPartical(particleRight1);
                ParticalGenerator.instance.destroyPartical(particleRight2);
                ParticalGenerator.instance.destroyPartical(instance);
            }


        }
        else if (particleType == ParticleType.Blue){
            // see if could destory
            Vector2 thisPosition = rigidBody.position;
            FallingParticles particleRight1 = GetNeighbour(thisPosition, Vector2.right, diameter);
            FallingParticles particleRight2 = GetNeighbour(thisPosition, Vector2.right, diameter*2);
            FallingParticles particleLeft1 = GetNeighbour(thisPosition, Vector2.left, diameter);
            FallingParticles particleLeft2 = GetNeighbour(thisPosition, Vector2.left, diameter*2);
            FallingParticles particleDown1 = GetNeighbour(thisPosition, Vector2.down, diameter);
            FallingParticles particleDown2 = GetNeighbour(thisPosition, Vector2.down, diameter*2);

            bool verticalBVanish = false;
            bool horizontalRGBVanish = false;
            bool horizontalBL1L2Vanish = false;
            bool horizontalBL1R1Vanish = false;
            bool horizontalBR1R2Vanish = false;

            if (particleDown1 != null && particleDown2 != null){
                if (particleDown1.particleType == ParticleType.Blue && particleDown2.particleType == ParticleType.Blue){
                    verticalBVanish = true;
                }
            }
            if(particleLeft2 != null && particleLeft1 != null){
                if (particleLeft2.particleType == ParticleType.Red && particleLeft1.particleType == ParticleType.Green){
                    horizontalRGBVanish = true;
                }
                if (particleLeft2.particleType == ParticleType.Blue && particleLeft1.particleType == ParticleType.Blue){
                    horizontalBL1L2Vanish = true;
                }
            }
            if (particleLeft1 != null && particleRight1 != null){

                if (particleLeft1.particleType == ParticleType.Blue && particleRight1.particleType == ParticleType.Blue){
                    horizontalBL1R1Vanish = true;
                }
            }

            if(particleRight1 != null && particleRight2 != null){
                if (particleRight1.particleType == ParticleType.Blue && particleRight2.particleType == ParticleType.Blue){
                    horizontalBR1R2Vanish = true;
                }
            }

            if (verticalBVanish){
                GameManager.instance.BAdded();
                ParticalGenerator.instance.destroyPartical(particleDown1);
                ParticalGenerator.instance.destroyPartical(particleDown2);
                ParticalGenerator.instance.destroyPartical(instance);
            }else if(horizontalRGBVanish){
                GameManager.instance.RGBAdded();
                ParticalGenerator.instance.destroyPartical(particleLeft2);
                ParticalGenerator.instance.destroyPartical(particleLeft1);
                ParticalGenerator.instance.destroyPartical(instance);
            }else if(horizontalBL1L2Vanish){
                GameManager.instance.BAdded();
                ParticalGenerator.instance.destroyPartical(particleLeft2);
                ParticalGenerator.instance.destroyPartical(particleLeft1);
                ParticalGenerator.instance.destroyPartical(instance);
            }else if(horizontalBL1R1Vanish){
                GameManager.instance.BAdded();
                ParticalGenerator.instance.destroyPartical(particleLeft1);
                ParticalGenerator.instance.destroyPartical(particleRight1);
                ParticalGenerator.instance.destroyPartical(instance);
            }else if(horizontalBR1R2Vanish){
                GameManager.instance.BAdded();
                ParticalGenerator.instance.destroyPartical(particleRight1);
                ParticalGenerator.instance.destroyPartical(particleRight2);
                ParticalGenerator.instance.destroyPartical(instance);
            }   
        }
    }


}
