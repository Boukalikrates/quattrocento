using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
[System.Serializable]
public class Game {

    public BoardLayout layout;
    public OverlayLayout overlay;

     int[,] board;
     int[,] tempboard;
    public bool gone;
     bool timeless;
    public int player;
    public int players;
    //  int time;
     int id=100;
    public Gamerule rule;
     List<Guy> guys;

    public Game(BoardLayout layout, OverlayLayout overlay){
        this.layout=layout;
        this.overlay=overlay;
        // this.newgame(false, new string[] {} ,new Gamerule());
    }

    public Game newgame(bool timeless, string[] names, Gamerule gamerule){
        rule = gamerule;
        board=new int[rule.boardsize,rule.boardsize];
        tempboard=new int[rule.boardsize,rule.boardsize];
        this.timeless=timeless;
        player=0;
        // time=69;
        id ++;
        players=rule.starts.Count;

        gone=players==0;

        layout.setup(this);


        overlay.hideresults();
        this.guys=new List<Guy>();
        for (int i = 0; i < this.players; i++) {
            this.guys.Add(new Guy(names[i%names.Length], i, this));
        }
        this.player = this.players - 1;
        overlay.setnames(this.guys, this.rule.max);
        
        if(!gone) nextplayer();

        return this;
    }
    void resettemp(){
        tempboard=(int[,])board.Clone();
    }
    List<Guy> yallwhoplay(){
        List<Guy> result = new List<Guy>();
        for (int i = 0; i < this.guys.Count; i++) {
            if (this.guys[i].still) result.Add(this.guys[i]);
        }
        return result;
        ////  TODO  ////
    }
   public  Guy guy(){
        return guys[(int)player];
    }
    public int? get(int x, int y){
        if (x < 0 || y < 0 || x >= this.rule.boardsize || y >= this.rule.boardsize)
            return null;
        return this.board[x,y];
    }
    public int? set(int x, int y, int k){
        if (x < 0 || y < 0 || x >= this.rule.boardsize || y >= this.rule.boardsize)
            return null;
        return this.board[x,y] = k;
    }
    public int? tempset(int x, int y, int k) {
        if (x < 0 || y < 0 || x >= this.rule.boardsize || y >= this.rule.boardsize)
            return null;
        return this.tempboard[x,y] = k;
    }
    bool anybody(bool still=false) {
        bool result = false;
        foreach (Guy item in guys)
        {
            if(still?item.still:item.plays)result = true;
        }
        return result;
    }


    public void paint(bool temp=false) {

        ////  TODO  ////
        this.layout.paint(temp ? this.tempboard : this.board);
    }


    public void nextplayer(bool direct=false){

        bool  i = false;
        Guy nextguy=this.guy();
        this.layout.dismissBlock();
        this.resettemp();
        if (true) {
            this.overlay.setscore(this.player, this.guy().countall(), Math.Max(this.rule.max - this.guy().countall(), 0));
        }

        if (!direct) {
            if (!this.anybody()) {
                overlay.alert("'Nobody to play!'");
                this.overlay.open();
                return ;
            }




            while (!i) {
                List<Guy>  yallwhoplay = this.yallwhoplay();
                nextguy = (!yallwhoplay.Contains(this.guy()) || yallwhoplay.IndexOf(this.guy()) + 1 == yallwhoplay.Count) ? yallwhoplay[0] : yallwhoplay[yallwhoplay.IndexOf(this.guy()) + 1];
                // nextguy=yallwhoplay[0];
                i = nextguy.checkavailable();
                if (!i) {
                    nextguy.still = false;
                    if (nextguy.bot!=0) overlay.alert(nextguy.name + "'! You have no more possible moves. Game over'");
                    if (!this.anybody(true)) {
                        this.gameover();
                        return;
                    }
                }
            }
            this.id++; 
            
        }
        this.player = nextguy.n;

        this.layout.paintshapes(nextguy);
        nextguy.checkavailable(null, -2);
        if (this.guy().bot>0) {
            //    this.paint();
            //this.guy().cputurn(this.guy().bot);
            // setTimeout(function () {
            //     this.guy().cputurn(this.guy().bot);
            // }.bind(this), 100);
        } else {
            // maketime(this.id);
        }


    }

    public void gameover() {
        this.gone = true;
               this.paint();
        this.layout.paintshapes(this.guy());

        this.overlay.gameover(this);
    }

}