using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;


public class Controller : MonoBehaviour
{
    public BoardLayout  layout;
    public OverlayLayout overlay;
    public new Camera camera; 
    Game current;
    // Start is called before the first frame update
    void Start()
    {
        string gamerulesJson=System.IO.File.ReadAllText("gamerules.json");
        Dictionary<string,Gamerule> gamerules = JsonConvert.DeserializeObject<Dictionary<string,Gamerule>>(gamerulesJson);

        current = new Game(layout,overlay);
        current.newgame(false,new string[]{"Player 1","Player 2","Player 3","Player 4"},gamerules["Classic"]);
    }

    // Update is called once per frame
    void Update()
    {
        int s = this.current.rule.boardsize;
        double pageX= 2*Input.mousePosition.x*s/Screen.width;
        double pageY=Input.mousePosition.y*s/Screen.height;
        float aspect=1f*Screen.height/Screen.width;
        if(Screen.width<Screen.height*2){
            //tall
            camera.orthographicSize=10*aspect;
            pageY*=2*aspect;
            // pageY+=1 *s*(Screen.width/2.0-Screen.height/1.0)/Screen.width;
            pageY+=s*(0.5-aspect);
        }else{
            //wide
            camera.orthographicSize=5f;
            pageX/=2*aspect;
            pageX-=s*(0.5/aspect-1);
        }



        if (this.layout.isActive()) {
            this.layout.glideBlock(pageX, pageY, true);
        }
        //click(e)
        if( Input.GetMouseButtonDown(0)) {
            if (this.overlay.opened) {
                // close overlay if needed
            } else {
                if (this.layout.isActive()) {
                    this.put();
                } else {
                    if (this.current.guy() != null) {
                        if (pageX >= s && pageX < s+1) {
                            // pagechange
                            int pa = this.layout.pages;
                            int dest = (int)Math.Floor(pageY - s / 2 + pa / 2);
                            if (dest >= 0 && dest < pa)
                                this.layout.changePage(dest);
                        } else if (pageX >= 1+s && s * 1.6 > pageX && pageY >= 0 && s > pageY) {
                            Block demand = this.current.guy().pickblock((float)pageX-s, (float)pageY);
                            if (!current.gone && this.current.guy().hasblock(demand)) {
                                // Debug.Log(pageX+", "+pageY);
                                // double px = pageX / this.base * 2000;
                                // double py = pageY / this.base * 2000;
                                this.layout.raiseBlock(demand);
                                this.layout.glideBlock(pageX, pageY, true);
                                this.current.guy().checkavailable(demand, this.current.guy().plate + 2);


                            }
                        }
                    }
                }
            }
        }
        //middleclick(e)
        if( Input.GetMouseButtonDown(1)) {

            if (this.layout.isActive() ) {
                this.layout.rotateBlock(1);
            }
        }
        if( Input.GetMouseButtonDown(2)) {

            if (this.layout.isActive() ) {
                this.layout.swapBlock();
            }
        }
        // wheel(e) 
        if (this.overlay.opened) {
        } else {
            if (!this.layout.isActive()) {
                if (pageX < s) {
                    // if ($("#rotateboard").is(":checked")) {
                    //     board -= Input.mouseScrollDelta.y > 0 ? 90 : Input.mouseScrollDelta.y < 0 ? -90 : 0;
                    //     $("#elemcontainer").css("transform", "rotate(" + (board) + "deg)");
                    // }
                } else if (pageX < s * 1.6) {
                    int nextpage = (Input.mouseScrollDelta.y > 0 ? 1 : Input.mouseScrollDelta.y < 0 ? -1 : 0) + this.layout.page;
                    this.layout.changePage(nextpage);
                }

            } else {
                this.layout.rotateBlock(Input.mouseScrollDelta.y > 0 ? 1 : Input.mouseScrollDelta.y < 0 ? -1 : 0);
            }
        }
    
    }   
    void put(){
        double gtx = layout.translationX;
        double gty = layout.translationY;
        if (gtx > layout.boardsize || gty > layout.boardsize || gtx < 0 || gty < 0) {
            this.layout.dismissBlock();
            this.current.guy().checkavailable(null, -2);
        } else {
            Block acti =this.layout.getActive();
            int wd=0;
            int hd=0;
            if ((this.layout.rotation / 90) % 2 == 0) {
                hd = acti.pd.y;
                wd = acti.pd.x;
            } else {
                hd = acti.pd.x;
                wd = acti.pd.y;
            }
            int xd = (int)Math.Round(1.0 * gtx  - wd / 2.0);
            int yd = (int)Math.Round(1.0 * gty  - hd / 2.0);


            int rotac = (int)((this.layout.rotation / 90) % 4);
            rotac = rotac < 0 ? rotac + 4 : rotac;
            bool sw = this.layout.scale < 0;
            bool swap = rotac % 2==1;
            bool rx = sw ? rotac == 1 || rotac == 2 : rotac == 2 || rotac == 3;
            bool ry = sw ? rotac == 0 || rotac == 1 : rotac == 1 || rotac == 2;
            
            // Debug.Log(  acti.id+", "+ xd+", "+  yd+", "+  swap+", "+  rx+", "+  ry+", "+  true );
            this.current.guy().setblock(acti, xd, yd, swap, rx, ry, true);
        }
    }
    
}
