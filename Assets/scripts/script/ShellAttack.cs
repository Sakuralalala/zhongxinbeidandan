using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//发射导弹，脚本挂在发射按钮上
public class ShellAttack :MonoBehaviour{
    public static Transform FirePosition;
    public static bool isNavigationCanDispear = true;
    public GameObject Nbomb;
    private GameObject Nbombpre;
    public static bool isNbomb=false;
    public ShellMove shellmove;
    public RaycastHit2D hit;

    public static Rigidbody2D shellRigidbody2D;

    // Use this for initialization
    void Start () {
        FirePosition = this.transform.parent.Find("FirePosition");
      
       
    }
    
    //实例化核弹
    private void OnMouseDown()
    {
        if (isNbomb == false)
        {
            Nbombpre = Instantiate(Nbomb, FirePosition.position, FirePosition.rotation);
            isNbomb = true;
            Planes.isNavigationCanDispear = false;
           
        }
    }

    // Update is called once per frame
    void Update () {
        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        
        //没有点击敌方星球而是点击了空白背景，删除实例化出来的核弹
		 if(hit.collider==null &&isNbomb !=false)
        {
            if (Input.GetMouseButtonDown(0)&&isNavigationCanDispear)
            {
                Destroy(Nbombpre);
                isNbomb = false;
                Planes.isNavigationCanDispear = true;
            }
        }
        //如果点击了其他星球，则发射核弹      
        if(hit.collider !=null && isNbomb != false)
        {
            if(hit.collider.tag=="Emeny" && Input.GetMouseButtonDown(0))
            {
                Nbombpre.GetComponent<ShellMove>().ShellCanMove();
                ShellMove.isShellCanMove = true;
                isNbomb = false;
                isNavigationCanDispear = true;
                Planes.isNavigationCanDispear = true;
                

            }

            

        }



    }
    

}
