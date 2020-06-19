using UnityEngine;
using System;

public class CoinMagnet : Consumable
{
    protected readonly Vector3 k_HalfExtentsBox = new Vector3 (20.0f, 1.0f, 1.0f);
    
    protected const int k_LayerMask =  10;
    
  
    protected CharacterInputController cic;

    public override string GetConsumableName()
    {
        return "Magnet";
    }

    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.COIN_MAG;
    }

    public override int GetPrice()
    {
        return 750;
    }

	public override int GetPremiumCost()
	{
		return 0;
	}

	
    protected Collider[] returnCollss = new Collider[20];

	public override void Tick(CharacterInputController c)
    {
        base.Tick(c);
        TICITICK(c, 0);
        TICITICK(c, 1);
    }

    public void TICITICK(CharacterInputController c , int g)
    {
        Collider[] returnColls = new Collider[20];
        if (g == 0)
        { 
            const int k_LayerMaskk = 1 << 8;
            int nb = Physics.OverlapBoxNonAlloc(c.characterCollider.transform.position, k_HalfExtentsBox, returnColls, c.characterCollider.transform.rotation, k_LayerMaskk);
            for (int i = 0; i < nb; ++i)
            {
                if (g == 0)
                {
                    Coin returnCoin = returnColls[i].GetComponent<Coin>();
                    if (returnCoin != null && !returnCoin.isPremium && !c.characterCollider.magnetCoins.Contains(returnCoin.gameObject))
                    {
                        if (Math.Abs(c.characterCollider.transform.position.x - returnCoin.gameObject.transform.position.x) >= 1 && Math.Abs(c.characterCollider.transform.position.x - returnCoin.gameObject.transform.position.x) < 3)
                        {
                            returnColls[i].transform.SetParent(c.transform);
                            c.characterCollider.magnetCoins.Add(returnColls[i].gameObject);
                        }
                    }
                }
                else
                {
                    Invincibility returnInvincibility = returnColls[i].GetComponent<Invincibility>();
                    if (Math.Abs(c.characterCollider.transform.position.x - returnInvincibility.gameObject.transform.position.x) >= 1 && Math.Abs(c.characterCollider.transform.position.x - returnInvincibility.gameObject.transform.position.x) < 3)
                    {
                        returnColls[i].transform.SetParent(c.transform);
                        c.characterCollider.magnetPowerUp.Add(returnColls[i].gameObject);
                    }
                }

            }
        }
        else
        {
            const int k_LayerMaskk = 1 << 10;
            int nb = Physics.OverlapBoxNonAlloc(c.characterCollider.transform.position, k_HalfExtentsBox, returnColls, c.characterCollider.transform.rotation, k_LayerMaskk);
            for (int i = 0; i < nb; ++i)
            {
                if (g == 0)
                {
                    Coin returnCoin = returnColls[i].GetComponent<Coin>();
                    if (returnCoin != null && !returnCoin.isPremium && !c.characterCollider.magnetCoins.Contains(returnCoin.gameObject))
                    {
                        if (Math.Abs(c.characterCollider.transform.position.x - returnCoin.gameObject.transform.position.x) >= 1 && Math.Abs(c.characterCollider.transform.position.x - returnCoin.gameObject.transform.position.x) < 3)
                        {
                            returnColls[i].transform.SetParent(c.transform);
                            c.characterCollider.magnetCoins.Add(returnColls[i].gameObject);
                        }
                    }
                }
                else
                {
                    Invincibility returnInvincibility = returnColls[i].GetComponent<Invincibility>();
                    if (Math.Abs(c.characterCollider.transform.position.x - returnInvincibility.gameObject.transform.position.x) >= 1 && Math.Abs(c.characterCollider.transform.position.x - returnInvincibility.gameObject.transform.position.x) < 3)
                    {
                        returnColls[i].transform.SetParent(c.transform);
                        c.characterCollider.magnetPowerUp.Add(returnColls[i].gameObject);
                    }
                }

            }
        }

        //int nb = Physics.OverlapBoxNonAlloc(c.characterCollider.transform.position, k_HalfExtentsBox, returnColls, c.characterCollider.transform.rotation, k_LayerMaskk);


      
    }
}
