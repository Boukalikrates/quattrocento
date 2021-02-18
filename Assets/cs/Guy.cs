
using System;
using System.Collections;
using System.Collections.Generic;

using System.Runtime.Serialization;
using Newtonsoft.Json;

[System.Serializable]
public class Guy
{

    [NonSerialized]
    public Game game;
    public int n;
     public int plate;
    public bool plays;
    public bool still;
    public int bot;
    public string name;
    string colour;

    public  Block last;
    public List<Block> blocks;

    private readonly Random random = new  Random();  
    public Guy(string a, int n, Game parent)
    {
        if (false)
        {
            ////  TODO  ////
        }
        else
        {
            game = parent;
            this.n = n;
            plate = n*5+1;
            name = a ?? "";
            plays = true;
            still = true;
            colour="hsl(" + (360 * n / game.players + 225) + ",100%,50%)";

            bot = getbottype(a);
            last = null;////  TODO  ////
            blocks=new List<Block>();
            foreach (var b in game.rule.data)
            {
                blocks.Add(b.Value);
            }
        }
    }
    int getbottype(string a){
        switch (a.ToLower())
        {
            case "_bot":
                return random.Next(1,4);
            case "_gregor":
                return 1;
            case "_martin":
                return 2;
            case "_joseph":
                return 3;
            default:
                return 0;
        }
    }
    
    public bool checkavailable(Block b=null, int? mark=null) {
        List<bool> ft = new List<bool>{false,true};
        for (int x = 0; x < this.game.rule.boardsize; x++) {
            for (int y = 0; y < this.game.rule.boardsize; y++) {
                if (this.empty(x, y) &&
                    this.border(x, y) &&
                    this.corner(x, y)
                ) {
                    int loop = b!=null ? 1 : this.blocks.Count;
                    for (int j = 0; j < loop; j++) {
                        Block act = b ?? this.blocks[j];
                        foreach (bool rx in ft) {
                            foreach (bool ry in ft) {
                                foreach (bool swap in ft) {
                                    for (int i = 0; i < act.ps; i++) {
                                        int nx;
                                        int ny;
                                        if (swap) {
                                            nx = ry ? (x - act.pd.y + act.pc[i].y + 1) : (x - act.pc[i].y);
                                            ny = rx ? (y - act.pd.x + act.pc[i].x + 1) : (y - act.pc[i].x);
                                        } else {
                                            nx = rx ? (x - act.pd.x + act.pc[i].x + 1) : (x - act.pc[i].x);
                                            ny = ry ? (y - act.pd.y + act.pc[i].y + 1) : (y - act.pc[i].y);
                                        }
                                        if (this.setblock(act, nx, ny, swap, rx, ry, false, mark) && mark==null) return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (mark!=null) this.game.paint(true);
        return false;
    }
    bool border(int x, int y) {
        
        return !(
            this.game.get(x - 1, y) == plate ||
            this.game.get(x + 1, y) == plate ||
            this.game.get(x, y - 1) == plate ||
            this.game.get(x, y + 1) == plate);
    }

    bool corner(int x, int y) {
        return (
            this.game.get(x - 1, y - 1) == plate ||
            this.game.get(x - 1, y + 1) == plate ||
            this.game.get(x + 1, y - 1) == plate ||
            this.game.get(x + 1, y + 1) == plate ||
            (
                this.game.rule.starts[n].x==x &&
                this.game.rule.starts[n].y==y 

            )
        );
    }

    bool empty(int x, int y) {
        return (this.game.get(x, y)%5 == 0);
    }


    public  bool hasblock(Block blk) {
        if (blk==null) return false;
        // UnityEngine.Debug.Log(blk.id);
        return blocks.Contains(blk);
    }

    Block remblock (Block a){
        if(!hasblock(a)) return null;
        blocks.Remove(a);

        return last=a;
    }


   public bool setblock(Block act, int x, int y, bool swap, bool rx, bool ry, bool draw, int? mark=null) {
        //if (!this.hasblock(act)) return false;
        int p = this.n;
        bool corner = false;
        int nx;
        int ny;

        for (int i = 0; i < act.ps; i++) {
            if (swap) {
                 nx = ry ? (x + act.pd.y - act.pc[i].y - 1) : (x + act.pc[i].y);
                 ny = rx ? (y + act.pd.x - act.pc[i].x - 1) : (y + act.pc[i].x);
            } else {
                 nx = rx ? (x + act.pd.x - act.pc[i].x - 1) : (x + act.pc[i].x);
                 ny = ry ? (y + act.pd.y - act.pc[i].y - 1) : (y + act.pc[i].y);
            }

            if (this.corner(nx, ny)) corner = true;
            if (!this.border(nx, ny)) return false;
            if (!this.empty(nx, ny)) return false;

        }
        if (corner && mark!=null) {
            for (int i = 0; i < act.ps; i++) {
                if (swap) {
                     nx = ry ? (x + act.pd.y - act.pc[i].y - 1) : (x + act.pc[i].y);
                     ny = rx ? (y + act.pd.x - act.pc[i].x - 1) : (y + act.pc[i].x);
                } else {
                     nx = rx ? (x + act.pd.x - act.pc[i].x - 1) : (x + act.pc[i].x);
                     ny = ry ? (y + act.pd.y - act.pc[i].y - 1) : (y + act.pc[i].y);
                }

                if (this.empty(nx, ny) &&
                    this.border(nx, ny) &&
                    this.corner(nx, ny)) this.game.tempset(nx, ny, (int)mark);

            }
        }
        if (corner && draw && p == this.game.player && this.hasblock(act)) {
            for (int i = 0; i < act.ps; i++) {
                if (swap) {
                     nx = ry ? (x + act.pd.y - act.pc[i].y - 1) : (x + act.pc[i].y);
                     ny = rx ? (y + act.pd.x - act.pc[i].x - 1) : (y + act.pc[i].x);
                } else {
                     nx = rx ? (x + act.pd.x - act.pc[i].x - 1) : (x + act.pc[i].x);
                     ny = ry ? (y + act.pd.y - act.pc[i].y - 1) : (y + act.pc[i].y);
                }
                this.game.set(nx, ny, plate);

            }
            this.remblock(act);
            this.game.nextplayer();
        }
        return corner;
    }

    void cputurn(int bottype) {
        // TODO
    }
    public  Block pickblock(float x, float y) {
        for (int j = 0; j < this.blocks.Count; j++) {
            Block b = this.blocks[j];
            for (int i = 0; i < b.ps; i++) {
                int bx = b.pc[i].x + b.pb.x;
                int by = b.pc[i].y + b.pb.y;
                if (bx - 0.5 < x && x < bx + 1.5 && by - 0.5 < y && y < by + 1.5 && b.p == this.game.layout.page) return b;
            }
        }
        return null;
    }

    public List<int> count() {
        List<int> pts0=new List<int>();
        for (int i = 0; i < this.game.rule.counts.Count; i++) {
            pts0.Add( i * this.game.rule.counts[i]);
        }
        foreach (Block i in this.blocks) {
            pts0[i.ps] -= i.ps;
        }
        return pts0;
    }
    public int countall() {
        bool isclearbonus = this.blocks.Count == 0;
        bool isubonus = isclearbonus ? (this.last.ps == 1) : (this.count()[1] == 0 && this.game.gone);
        int result=(isclearbonus ? this.game.rule.clearbonus : 0) + (isubonus ? this.game.rule.ubonus : 0);
        foreach (var i in count())
        {
            result+=i;
        }
        return result;
            
    }

    // public string ToString(){
    //     return name;
    // }


}