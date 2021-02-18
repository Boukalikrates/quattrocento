using System.Collections;
using System.Collections.Generic;
using System;

using System.Runtime.Serialization;
using Newtonsoft.Json;

[Serializable]
public class Block
{

    [JsonIgnore]
    public string id;

    [JsonIgnore]
    public int ps;

    [JsonIgnore]
    public xy pd;


    public xy pb;
    public int p;
    public List<xy> pc;

    public Block(){

        this.id = "";
        this.pd = new xy();
        this.ps = 0;
        this.pb = new xy();
        this.pc = new List<xy>();
        this.p = 0;
    }
    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {

        int minx = Int32.MaxValue, miny = Int32.MaxValue, maxx = 0, maxy = 0;
        for (int i = 0; i < pc.Count; i++)
        {
            minx = Math.Min(minx, pc[i].x);
            maxx = Math.Max(maxx, pc[i].x);
            miny = Math.Min(miny, pc[i].y);
            maxy = Math.Max(maxy, pc[i].y);
        }
        this.pd = new xy(maxx - minx + 1, maxy - miny + 1);
        this.ps = pc.Count;
    }
    
    public string toString()
    {
        return this.id;
    }

}