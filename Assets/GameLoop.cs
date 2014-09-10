using UnityEngine;
using System.Collections;

public class GameLoop : MonoBehaviour {

    public UILabel PrisonerLabel;
    public UILabel MoneyLabel;
    public UILabel PoliceForceLabel;
    public UILabel GuardsLabel;
    public UIButton ArrestButton;
    public UIButton CellBlockButton;

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
   

	// Use this for initialization
	void Start () {
        CellBlockButton.isEnabled = false;
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
            Debug.Log("Rookie salary:"+rookieSalary);
            Debug.Log("Guard salary:"+guardSalary);

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
	}

    public void Arrest()
    {
        if(!ArrestButton.isEnabled)
        {
            return;
        }
        prisoners++;
        updateLabels();
        if(prisoners >= cellBlocks * PRISONERS_PER_CELLBLOCK)
        {
            ArrestButton.isEnabled = false;
        }
    }

    public void Work()
    {
        Debug.Log("money before work" + money);
        money += MONEY_PER_PRISONER * prisoners;
        Debug.Log("money after work" + money);
        if(money >= nextCellBlockCost())
        {
            CellBlockButton.isEnabled = true;
        }
        updateLabels();
    }

    public void BuildCellBlock()
    {
        money = money - nextCellBlockCost();
        ArrestButton.isEnabled = true;
        cellBlocks++;
        if(money < nextCellBlockCost())
        {
            CellBlockButton.isEnabled = false;
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

    private decimal nextCellBlockCost()
    {
        return (decimal) (CELL_BLOCK_BASE_COST * Mathf.Pow(1.15f, (float)cellBlocks));
    }
}
