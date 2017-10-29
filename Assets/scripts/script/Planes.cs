using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planes : MonoBehaviour {
    //整个星球的脚本，有三种属性，isPlayer,isEmeny,isNone,每种属性不同方法
    public static Transform FirePosition;
    public static bool isNavigationCanDispear = true;//作为判断是否UI可以被删除
    public int planeHp=100;
    public Transform shellPosition;
    public Transform haloPosition;
    public Transform airshipPosition;
    public Transform protectPosition;
    public GameObject ShellPre;
    public GameObject HaloPre;
    public GameObject ProtectPre;
    public GameObject AirshipPre;
    public bool isInput = false;//作为判断是否产生了UI
    public RaycastHit2D hit;
    private GameObject shell, halo, protect, airship;
    public int id;//id
    public bool isPlayer, isEmeny, isNone;//作为是否是玩家，敌人，还是无主星球的判定

    // Use this for initialization
    void Start () {
      
        FirePosition = transform.Find("FirePosition");
    }
    private void OnMouseDown()
    {
        if (isInput == false && isPlayer==true)
        {
            shell = Instantiate(ShellPre, shellPosition.position, shellPosition.rotation);//发射按钮，上面挂载发射子弹相关的脚本
            halo = Instantiate(HaloPre, haloPosition.position, haloPosition.rotation);//光环效果
            airship = Instantiate(AirshipPre, airshipPosition.position, airshipPosition.rotation);//飞船按钮，上面挂载发射飞船相关脚本
            protect = Instantiate(ProtectPre, protectPosition.position, protectPosition.rotation);//保护按钮，上面挂载防护层相关按钮
            isInput = true;

            //作为星球的子物体存在
            shell.transform.parent = this.gameObject.transform;
            halo.transform.parent = this.gameObject.transform;
            airship.transform.parent = this.gameObject.transform;
            protect.transform.parent = this.gameObject.transform;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isPlayer == true)//是玩家星球时
        {//射线检测
            this.gameObject.tag = "Player";
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            //删除UI的逻辑                                                                                             
            if (hit.collider == null && isInput == true)
            {
                if (Input.GetMouseButtonDown(0) && isNavigationCanDispear)
                {
                    DestroyUI();
                }
            }
            //如果子弹发射，立即删除UI
            if (ShellMove.isShellCanMove == true)
            {
                DestroyUI();
                ShellMove.isShellCanMove = false;
            }
            //

        }
        if (isEmeny == true)//是敌人星球时
        {
            this.gameObject.tag = "Emeny";
        }
        if (isNone == true)//是无主星球时
        {
            this.gameObject.tag = "None";
            //添加判断条件（飞船的功能）
            //ChangeToplayer();

        }


    }

    //删除UI
    public void DestroyUI()
    {
        Destroy(shell);
        Destroy(halo);
        Destroy(protect);
        Destroy(airship);
        isInput = false;
    }
    //无主星球变成玩家星球
    public void ChangeToplayer()
    {
        if(isNone==true)
        {
            isNone = false;
            isPlayer = true;
        }

    }
    //碰撞检测，子弹与陨石的伤害
    public void TakeDamage()
    {
        if (planeHp <= 0) return;
        planeHp -= 10;
        if (planeHp <= 10)
        {
            Destroy(this.gameObject);
        }
    }
    


}
