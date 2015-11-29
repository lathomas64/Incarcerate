using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class Events : MonoBehaviour {

	public GameObject EventPanel;
    public GameObject PrisonerView;
    public GameObject PrisonerButtonPrefab;
	public Text EventText;
	public List<Prisoner> population;
	public Queue<string> queuedEvents;
	public Dictionary<string,GameObject> buttons;
	public float SECONDS_PER_TICK;
    private float next_tick;
    private bool leasing_unlock = false;
	//private float 

	// Use this for initialization
	void Start () {
        next_tick = SECONDS_PER_TICK;
		population = new List<Prisoner> ();
		queuedEvents = new Queue<string> ();
		buttons = new Dictionary<string,GameObject > ();
		GameObject leaseButton = GameObject.Find ("Lease Button");
		leaseButton.SetActive (false);
		buttons.Add ("Lease", leaseButton);
		Invoke ("HideEvent", 5);
	}
	
	// Update is called once per frame
	void Update () {
		if (!EventPanel.activeInHierarchy && queuedEvents.Count > 0) {
			string eventText = queuedEvents.Dequeue();
			ShowEvent(eventText);
		}
        next_tick -= Time.deltaTime;
        if (next_tick <= 0)
        {
            simulation_tick();
            next_tick = SECONDS_PER_TICK;
        }
	}

    private void simulation_tick()
    {
        List<Prisoner> released = new List<Prisoner>();
        foreach (Prisoner p in population)
        {
            p.Tick();
            if (p.SentenceOver())
            {
                released.Add(p);
            }
        }
        foreach (Prisoner p in released)
        {
            population.Remove(p);
            GameObject button = GameObject.Find(p.name);
            button.SetActive(false);
            QueueEvent("Prisoner " + p.name + " has served their sentence and has been released.");
        }
    }

	public void Arrest()
	{
		Prisoner inmate = new Prisoner ();
		population.Add (inmate);
        GameObject newButtonObject = (GameObject)Instantiate(PrisonerButtonPrefab);
        newButtonObject.transform.SetParent(PrisonerView.transform);
        newButtonObject.transform.localPosition = new Vector3(80, -30 * population.Count, 0);
        newButtonObject.name = inmate.name;
        Button newButton = newButtonObject.GetComponent<Button>();
        Text newButtonText = newButtonObject.GetComponentInChildren<Text>();
        newButtonText.text = inmate.name;
        newButton.onClick.AddListener(() => ShowPrisoner(inmate));

        PrisonerView.AddComponent<Button>();
		ShowEvent("New Prisoner #:"+population.Count+inmate.ToString());
		if (!leasing_unlock && population.Count >= 5) {
			Debug.Log ("5 prisoners");
			GameObject leaseButton;
			buttons.TryGetValue("Lease", out leaseButton);
			leaseButton.SetActive(true);
			queuedEvents.Enqueue("Convict Leasing unlock text goes here");
            leasing_unlock = true;
            Analytics.CustomEvent("unlock", new Dictionary<string, object>
              {
                { "target", "Convict Leasing" }
              });
        }
	}

	public void Lease()
	{
		ShowEvent ("This isn't implemented yet Sadly");
	}

	private void ShowEvent(string eventInfo)
	{
		EventText.text = eventInfo;
		EventPanel.SetActive (true);
	}

	public void HideEvent()
	{
		EventPanel.SetActive (false);
		EventText.text = "";
	}

    public void QueueEvent(string eventInfo)
    {
        queuedEvents.Enqueue(eventInfo);
    }

    private void ShowPrisoner(Prisoner inmate)
    {
        ShowEvent(inmate.ToString());
    }
}
