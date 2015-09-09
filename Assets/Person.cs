using UnityEngine;
using System.Collections;

public abstract class Person {

	private static string[] names = {"Alex", "Jack", "Casey", "Jessie", "Jamie", "Rory", "Logan"};
	private static string[] surnames = {"Jones", "Lecter", "Bush", "Romanov", "Smith"};
	
	protected string _name;
    public string name { get { return _name; } }

	public Person(){
		GenerateRandomName ();
	}

	protected void GenerateRandomName(){
		string first = Person.names[Random.Range(0, Person.names.Length-1)];
		string last = Person.surnames [Random.Range (0, Person.surnames.Length - 1)];
		_name = first + " " + last;
	}

	public override string ToString(){
		return name;
	}

	public abstract void Tick ();

}
