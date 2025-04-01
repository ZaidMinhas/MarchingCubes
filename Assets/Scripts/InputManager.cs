using UnityEngine;

public class InputManager : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.Instance.smoothTerrains();
            
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.flatShadTerrains();
            
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameManager.Instance.resetTerrains();
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            LevelManager.setNextStrategy();
            GameManager.Instance.resetTerrains();
        }
    }
}
