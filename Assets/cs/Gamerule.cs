using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.Serialization;
using Newtonsoft.Json;

[Serializable]
public class Gamerule
{

    [JsonIgnore]
    public List<int> counts;

    [JsonIgnore]
    public int length=0;

    [JsonIgnore]
    public int max=0;

    [JsonIgnore]
    public List<Block> list;
    

    [JsonIgnore]
    public int pages=1;

    public Dictionary<string,Block> data;
    public List<xy> starts;
    public int ubonus=0;
    public int clearbonus=0;
    public int boardsize=1;
    public Gamerule(){
        counts=new List<int>();
        starts=new List<xy>();
        data = new Dictionary<string,Block>();
        list=new List<Block>();

    }

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        int i = 0;
        int m = 0;
        int n = 0; 
        string s = ""; 
        int p = 0;
        foreach (KeyValuePair<string, Block>  x in data) {
            list.Add(x.Value);
            x.Value.id=x.Key;
            i++;
            p = Math.Max(p, x.Value.p);
            n = Math.Max(n, x.Value.pc.Count);
            m += x.Value.pc.Count;
            s += x.Value.id;
            }
        List<int> counts = new List<int>();
        for(int j=0;j<=n;j++){
            counts.Add(0);
        }
        foreach (KeyValuePair<string, Block> x in data) {
            int count=x.Value.pc.Count;
            counts[count]=counts[count]+1;
        }
        this.counts = counts;
        this.length = i;
        this.max = m;
        // this.list = s;
        this.pages = p + 1;
    }
}