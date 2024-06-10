using StarterAssets;
using UnityEngine;

public class Firearm : MonoBehaviour, IShooting
{
    //The angle that makes a cone shape from barrel into the space that shows possible bullet spread(in radians)
    public float AccuracyAngle { get; set;}

// public Vector3 PlayerPosition
// {

// }

    public Vector3 CursorPosition
    {
        get{
            return Input.mousePosition;
        }
    }
    
    public bool Ads {
        get{
            if (Input.GetMouseButtonDown(1))
                return true;
            else
                return false;
        }
    }



    public void fire()
    {
      //make a line from barrel into the space. line is generated randomly according to the gun's AccuracyAngle  
      //Ax + By + Cz + D = 0
     //get point from which the damaging line begins(character point) 
     //point that character aims is the CursorPosition
    }

    public void hipfire()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
