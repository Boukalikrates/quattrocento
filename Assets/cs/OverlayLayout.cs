using System.Collections;
using System.Collections.Generic;

using UnityEngine;    
using UnityEngine.UI;


public class OverlayLayout : MonoBehaviour
{
    public bool opened=false;
    public GameObject table;
    void Start(){
        table=GameObject.Find("table");
    }
    public void hideresults()
    {

    }
    public void alert(string messgae)
    {
        Debug.Log(messgae);

    }
    public void open(string what="settings")
    {

    }
    public void setscore(int index, int score1=69, int score2=420)
    {
            var rowt = table.transform.GetChild(index+1).gameObject.transform;
            rowt.Find("score").gameObject.GetComponent<Text>().text=score1.ToString();
            rowt.Find("rem").gameObject.GetComponent<Text>().text=score2.ToString();
        
    }
    public void gameover(Game game)
    {

    }
    public void setnames(List<Guy> guys, int max=0) {
        GameObject entry= table.transform.Find("entry").gameObject;
        for(int i = 0; i<table.transform.childCount;i++){
            var go = table.transform.GetChild(i).gameObject;
            if(go.name=="entry"){
                go.SetActive(false);
            }
            else{
                Destroy(go);
            }
        }
        foreach(Guy i in guys){
            GameObject row=Instantiate(entry);
            Transform rowt=row.transform;
            row.name=i.n+" "+i.name;
            rowt.SetParent(table.transform);
            rowt.localPosition = new Vector3(0f,i.n*-50f,0f);
            rowt.localScale = Vector3.one;
            var name=rowt.Find("name").gameObject.GetComponent<Text>();
            name.text=i.name;
            name.color=Color.HSVToRGB(1f*i.n/guys.Count%1,1,1);
            rowt.Find("score").gameObject.GetComponent<Text>().text="0";
            rowt.Find("rem").gameObject.GetComponent<Text>().text=max.ToString();

            row.SetActive(true);

        }
     }
}