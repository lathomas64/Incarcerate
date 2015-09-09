using UnityEngine;
using System.Collections;

public class Prisoner : Person {
	protected int sentence_months;

	public Prisoner(){
		sentence_months = Random.Range (1, 1200);
	}

	public override void Tick(){
		sentence_months -= 1;
		Debug.Log (ToString ());
	}

	public override string ToString(){
		string result = name;
		result += "\nSentence:"+sentence_months;
		return result;
	}

    public bool SentenceOver()
    {
        return sentence_months <= 0;
    }
}
