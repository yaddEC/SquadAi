using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Formation : int
{
    NONE,
    TURTLE,
    SHIELD,
    SURROUND,
    MOUSE,
    FREEZE
}
public class CurrentFormation : MonoBehaviour
{
    [SerializeField]
    private GameObject _formationUI;
    [SerializeField]
    private Material _currentMat;
    private RawImage _currentImage;



    // Start is called before the first frame update
    void Start()
    {
        _currentImage = _formationUI.transform.GetChild(0).GetComponent<RawImage>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.Alpha1) )
        {
            ChangeFormation(Formation.NONE);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeFormation(Formation.TURTLE);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeFormation(Formation.SHIELD);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeFormation(Formation.SURROUND);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeFormation(Formation.MOUSE);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ChangeFormation(Formation.FREEZE);
            return;
        }

    }

    void ChangeFormation(Formation formation)
    {
        _currentImage.material = null;
        _currentImage = _formationUI.transform.GetChild((int)formation).GetComponent<RawImage>();
        _currentImage.material = _currentMat;
        AIManager.Instance.currentFormation = formation;
                    
    }
  
}
