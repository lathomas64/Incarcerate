using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour {

    public Text PrisonerLabel;
    public Text MoneyLabel;
    public Text PoliceForceLabel;
    public Text GuardsLabel;
    public Button ArrestButton;
    public Button CellBlockButton;
    public Text EventLabel;


    public decimal MONEY_PER_PRISONER = 0.5m;
    public int PRISONERS_PER_CELLBLOCK = 10;
    public float CELL_BLOCK_BASE_COST = 50;
    public float ROOKIE_BASE_COST = 1;
    public float ROOKIE_ARRESTS_PER_SECOND = .1f;
    public float GUARD_BASE_COST = 5;
    public float GUARD_WORKSHIFT_PER_SECOND = .5f;

    private decimal money = 10;
    private int prisoners = 0;
    private int cellBlocks = 1;
    private int guards = 0;
    private int rookies =0;

    private float nextArrest = 0;
    private float nextWorkShift = 0;

	private IncrementalGame.Resource[] resources;
   

	// Use this for initialization
	void Start () {
        CellBlockButton.interactable = false;
        rookies = PlayerPrefs.GetInt("rookies");
        guards = PlayerPrefs.GetInt("guards");
        money = (decimal)PlayerPrefs.GetFloat("money");
        prisoners = PlayerPrefs.GetInt("prisoners");
        cellBlocks = PlayerPrefs.GetInt("cellblocks");
        if (cellBlocks < 1)
            cellBlocks = 1;
        InvokeRepeating("Save", 10, 10);
        InvokeRepeating("randomEvent", 60, 60);
	}
    
    private void Save()
    {
        PlayerPrefs.SetInt("rookies", rookies);
        PlayerPrefs.SetInt("guards", guards);
        PlayerPrefs.SetInt("prisoners", prisoners);
        PlayerPrefs.SetFloat("money", (float) money);
        PlayerPrefs.SetInt("cellblocks", cellBlocks);
        PlayerPrefs.Save();
        Debug.Log("Autosave...");
    }
	
	// Update is called once per frame
	void Update () {        
	    if(money > 0 && (guards > 0 || rookies > 0))
        {
            decimal rookieSalary;
            decimal guardSalary;
            if(rookies == 0)
            {
                rookieSalary = 0;
            }
            else
            {
                rookieSalary = (decimal)(Time.deltaTime * ROOKIE_BASE_COST * Mathf.Pow(1.15f, (float)rookies));
            }
            if(guards == 0)
            {
                guardSalary = 0;
            }
            else
            {
                guardSalary = (decimal)(Time.deltaTime * GUARD_BASE_COST * Mathf.Pow(1.15f, (float)guards));
            }
            //Debug.Log("Rookie salary:"+rookieSalary);
            //Debug.Log("Guard salary:"+guardSalary);

            if (money >= guardSalary)
            {
                money -= guardSalary;
                nextWorkShift += guards * GUARD_WORKSHIFT_PER_SECOND * Time.deltaTime;
                if(Mathf.Floor(nextWorkShift) > 0)
                {
                    Work();
                    nextWorkShift = 0;  
                }
            }
            else if(guards > 0)
            {
                money = 0;
                guards--;
            }

            if(money >= rookieSalary)
            {
                money -= rookieSalary;
                nextArrest += rookies * ROOKIE_ARRESTS_PER_SECOND * Time.deltaTime;
                if (Mathf.Floor(nextArrest) > 0)
                {
                    Arrest();
                    nextArrest = 0;
                }
            } else if (rookies > 0) {
                money = 0;
                rookies--;
            }
        }
        updateLabels();
        updateButtons();
	}

    /// <summary>
    /// callback for arrest button
    /// </summary>
    public void Arrest()
    {
        if(!ArrestButton.interactable)
        {
            return;
        }
        prisoners++;
        updateLabels();
        if(prisoners >= cellBlocks * PRISONERS_PER_CELLBLOCK)
        {
            ArrestButton.interactable = false;
        }
    }

    /// <summary>
    /// callback for the work button
    /// </summary>
    public void Work()
    {
        //Debug.Log("money before work" + money);
        money += MONEY_PER_PRISONER * prisoners;
        //Debug.Log("money after work" + money);
        if(money >= nextCellBlockCost())
        {
            CellBlockButton.interactable = true;
        }
        updateLabels();
    }
    
    public void BuildCellBlock()
    {
        money = money - nextCellBlockCost();
        ArrestButton.interactable = true;
        cellBlocks++;
        if(money < nextCellBlockCost())
        {
            CellBlockButton.interactable = false;
        }
        updateLabels();
    }

    public void HireRookie()
    {
        rookies++;
        updateLabels();
    }

    public void HireGuard()
    {
        guards++;
        updateLabels();
    }

    private void updateLabels()
    {
        PrisonerLabel.text = "Prisoners: " + prisoners + "/" + cellBlocks * PRISONERS_PER_CELLBLOCK;
        MoneyLabel.text = "Money:$" + money.ToString("F2");
        PoliceForceLabel.text = "Police Force:\nYou\nRookies:" + rookies;
        GuardsLabel.text = "Guards:" + guards;
    }

    private void updateButtons()
    {
        ArrestButton.interactable = prisoners < PRISONERS_PER_CELLBLOCK * cellBlocks;
        CellBlockButton.interactable = nextCellBlockCost() <= money;
    }

    private decimal nextCellBlockCost()
    {
        return (decimal) (CELL_BLOCK_BASE_COST * Mathf.Pow(1.15f, (float)cellBlocks));
    }

    private void randomEvent()
    {
        Debug.Log("random event check happened");
        float riotChance = prisoners * .05f - guards * .2f;
        riotChance = Mathf.Max(.01f, riotChance);
        Debug.Log("Riot chance:" + riotChance);
        if(Random.Range(0f,1f) <riotChance)
        {
            prisonRiot();
        }
    }

    #region Random Event 

    private void clearEventText()
    {
        EventLabel.text = "";
    }

    /// <summary>
    /// riot random event
    /// once this event occurs we lose 1-20% of guards(round up) and 1-25% of prisoners(round up)
    /// </summary>
    private void prisonRiot()
    {
        Debug.Log("Prison riot runs");
        int guardLosses = Mathf.Max(1, ((int) (Random.Range(.01f, .2f) * guards)));
        int prisonerLosses = ((int)(Random.Range(.05f, .25f) * prisoners));
        guardLosses = Mathf.Min(guardLosses, guards);
        prisonerLosses = Mathf.Min(prisonerLosses, prisoners);
        Debug.Log("minimum prisoner loss:" + .05f * prisoners);
        prisoners -= prisonerLosses;
        guards -= guardLosses;
        EventLabel.text = "A Riot broke out:\n" + guardLosses + " guards killed\n" + prisonerLosses + " prisoners killed.";
        Debug.Log(EventLabel.text);
        Invoke("clearEventText", 10);
    }

    #endregion
}
