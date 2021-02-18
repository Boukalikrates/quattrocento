using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class BoardLayout : MonoBehaviour
{
    public Grid grid;

    public Tilemap mainboard;
    Dictionary<string,Tilemap> shapecontainer=new Dictionary<string,Tilemap>();
    
    public Tile[] tilebase;
    public Tile empty;
    public Tile dot;
    public Tile placeholder;
    public Game current;
    // int[,] matrix ;

    

        //presetup
    //  var positions ;
      public  int boardsize = 20;
       public double tilesize = 0.5f;
        double px = 1;
       public int page = 0;
       public int pages = 1;
       int players=1;
        Block active=null ;
        // string hover;
        public double translationX = 0;
        public double translationY = 0;
      public   int rotation = 0;
      public   int scale = 1;
    // Start is called before the first frame update

    private Tilemap CreateTilemap(string tilemapName)
    {
        var go = new GameObject(tilemapName);
        var tm = go.AddComponent<Tilemap>();
        var tr = go.AddComponent<TilemapRenderer>();
        tm.tileAnchor = new Vector3(0.5f, 0.5f, 0);
        go.transform.SetParent(grid.transform);
go.transform.position = grid.transform.position;
go.transform.rotation = grid.transform.rotation;
go.transform.localScale = new Vector3(20f/boardsize, 20f/boardsize, 1f);
// go.transform.scale = 1;
        
        tr.sortingLayerName = "Main";

        return tm;
    }
    
   public bool isActive(){
       return active!=null;
   }
   public Block getActive(){
       return active;
   }

    void Start()
    {


        // Tilemap temp = CreateTilemap("temp");


    }



    public void setup(Game game){

        this.boardsize = game.rule.boardsize;
        this.tilesize = 10.0f / this.boardsize;
        this.px = this.tilesize / 20;
        this.page = 0;
        this.pages = game.rule.pages;
        this.translationX = this.boardsize / 2;
        this.translationY = this.boardsize / 2;
        this.rotation = 0;
        this.scale = 1;
        this.players= game.players;
        mainboard.transform.localScale=new Vector3(20f/boardsize, 20f/boardsize, 1f);
    }
    void makeblock(Block b,Guy guy, Tilemap map){
        bool e = guy.blocks.Contains(b);
        bool f = guy.checkavailable(b);
        bool g = guy.last==b;
        int n = guy.plate;
        int colour = e ? (f ? n : n-1000) : (g ? n + 1 : n-999);
        double anchorX=(1-b.pd.x)/2.0f;
        double anchorY=(1-b.pd.y)/2.0f;
        map.tileAnchor = new Vector3((float)anchorX, (float)anchorY, 0);
        clearBlock(b);
        foreach (var pc in b.pc)
        {
            placeelem(map, pc.x,pc.y,colour);
        }
    }
    void makeshapes(Guy guy){
        
        foreach ( KeyValuePair<string, Tilemap> i in shapecontainer) {
            Destroy(i.Value.gameObject);
        }
        shapecontainer=new Dictionary<string,Tilemap>();
        foreach (Block b in guy.game.rule.list)
        {
            Tilemap map=CreateTilemap(b.id);
            shapecontainer.Add(b.id,map);
            makeblock(b,guy,map);
            // makeblock();

        }
    }
    void makepageindicator (Guy guy){}

    public void paintshapes(Guy guy){

        makeshapes(guy);
        changePage(page,true);
    }

    void placeelem(Tilemap map,int x,int y, int type){
        Tile tile=empty;
        int tiletype=((type-1)/5%4+4)%4;
        Color color=new Color(0.3f,0.3f,0.3f);
        if(type>0){
            int c=type/5;
            color=Color.HSVToRGB(1f*c/players%1,1,1);
        }

        switch (type%5)
        {
            case 1:
            case -4:
                tile = tilebase[tiletype];
                break;
            case 2:
            case -3:
                tile=placeholder;
                break;
            case 3:
            case -2:
                tile=dot;
                break;
            default:
                break;
        }
        Vector3Int pos=new Vector3Int(x,y,0);
        map.SetTile(pos,tile);
        map.SetTileFlags(pos, TileFlags.None);
        map.SetColor( pos, color);
        
    }

    public void paint(int[,] array){
        mainboard.ClearAllTiles();
        for (int x = 0; x < array.GetLength(0); x += 1) {
            for (int y = 0; y < array.GetLength(1); y += 1) {
                int item = array[x, y]; 
                {
                   placeelem(mainboard,x,y,item);
                }
            }
        }
    }

    //MODYFYING
    public void raiseBlock(Block id) {
        active=id;
        updateSubBlock();
    }

    public void dismissBlock(){
        if(active!= null){
            rotation=0;
            scale=1;
            updateSubBlock();
            clearBlock(active);
            active=null;
        }
    }

    
    public void changePage(int p,bool force=false){
        int from=page;
        if (p < 0) p = 0;
        else if (p >= pages) p = pages - 1;


        if (force || p != page) {
            page = p;
            double change=(from-p)*(boardsize+1)*tilesize;
            foreach ( KeyValuePair<string, Tilemap> i in shapecontainer) {
                   i.Value.transform.position += new Vector3(0f,(float)change,0f);
            }
        }
    }
    
    void clearBlock(Block b, bool instant=true){
        Tilemap map =  shapecontainer[b.id];
        double xx=b.pb.x*tilesize+b.pd.x*tilesize/2.0;
        double yy=b.pb.y*tilesize+b.pd.y*tilesize/2.0 + (b.p - page) * tilesize * (boardsize + 1)-5;
        map.transform.position = new Vector3((float)xx,(float)yy,-1f);
    }
    void updateBlock(bool instant=false){
        if(active!=null){
            double xx=(translationX- boardsize) * tilesize    ;
            double yy=(translationY - boardsize*0.5)* tilesize;
            if(shapecontainer.ContainsKey(active.id)){

            Tilemap map =  shapecontainer[active.id];
            map.transform.position = new Vector3((float)xx,(float)yy,-2f);
            }
        }
        
    }

    void updateSubBlock(){
        if(active!=null){
            Tilemap map =  shapecontainer[active.id];
            map.transform.eulerAngles=new Vector3(scale*90f-90f,0f,rotation);
        }
    }



    // TRANSFORMING ACTIVE BLOCK

 public  void  alignBlock(int x = 0, int y = 0) {
       double cx=active.pd.x * tilesize / 2;
       double cy=active.pd.y * tilesize / 2;
        translationX = Math.Round(translationX / tilesize + x) * tilesize - (rotation % 180 == 0 ? cx : cy) % tilesize;
        translationY = Math.Round(translationY / tilesize + y) * tilesize - (rotation % 180 == 0 ? cy : cx) %tilesize;
        updateBlock();
    }
public  void    glideBlock(double x, double y  , bool instant = false) {
        translationX = x;//- this.positions[id].cx;
        translationY = y;//- this.positions[id].cy;
        updateBlock(instant);
    }

public void    rotateBlock(int c = 0) {
        rotation += c * 90 * scale * -1;
        updateSubBlock();
    }
public void    swapBlock() {
        scale *= -1;
        updateSubBlock();
    }

}
