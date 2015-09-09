using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml.Serialization;
using IncrementalGame;

public class Editor : MonoBehaviour {
	[XmlArray("ResourceList"), XmlArrayItem(typeof(Resource), ElementName = "Resource")]
	List<Resource> resources;

	[XmlArray("ButtonList"), XmlArrayItem(typeof(IncrementalGame.Button), ElementName = "Button")]
	List<IncrementalGame.Button> buttons;

	[XmlArray("DisplayList"), XmlArrayItem(typeof(IncrementalGame.Display), ElementName = "Display")]
	List<IncrementalGame.Display> displays;

	public Text newResourceName;
	public Text newResourceCapacity;

	// Use this for initialization
	void Start () {
		resources = new List<Resource> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void addResource() {
		string name = newResourceName.text;
		float capacity = float.Parse(newResourceCapacity.text);
		Debug.Log ("AddResource pressed::" + name +"::"+ capacity);
		Resource temp = new Resource ();
		temp.name = name;
		temp.capacity = capacity;
		resources.Add (temp);
		Debug.Log (resources);
		System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer (resources.GetType ());
		System.IO.StreamWriter outfile = new System.IO.StreamWriter(
			@"resources.xml");
		x.Serialize(outfile, resources);
		outfile.Close();
		//System.IO.StreamReader infile = new System.IO.StreamReader (@"test.xml");
		//Resource read = (Resource)x.Deserialize (infile);
		//Debug.Log (read.capacity);
		//infile.Close ();
	}

	public void loadResources() {
		Debug.Log ("load resources pressed");
		Debug.Log (resources);
		Debug.Log (resources.Count);
		System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer (resources.GetType ());
		System.IO.StreamReader infile = new System.IO.StreamReader (@"resources.xml");
		resources = (List<Resource>)x.Deserialize (infile);
		Debug.Log (resources);
		Debug.Log (resources.Count);
		infile.Close ();
	}
}
