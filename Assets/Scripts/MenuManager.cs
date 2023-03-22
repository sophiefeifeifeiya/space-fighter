using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update

    public enum MenuState
{
    menu,
    help,
    Acknowledge
}
    public MenuState currentMenuState = MenuState.menu;
    public static MenuManager instance;

    public Canvas menuCanvas;
    public Canvas helpCanvas;
    public Canvas AcknowledgeCanvas;

    private void Awake() {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetMenuState(MenuState state){
        currentMenuState = state;
        switch (state){
            case MenuState.menu:
                menuCanvas.enabled = true;
                helpCanvas.enabled = false;
                AcknowledgeCanvas.enabled = false;
                break;
            case MenuState.help:
                menuCanvas.enabled = false;
                helpCanvas.enabled = true;
                AcknowledgeCanvas.enabled = false;
                break;
            case MenuState.Acknowledge:
                menuCanvas.enabled = false;
                helpCanvas.enabled = false;
                AcknowledgeCanvas.enabled = true;
                break;
        }
    }

    public void StartGame(int index){
        SceneManager.LoadScene(index);
    }

    public void Help(){
        SetMenuState(MenuState.help);

    }

    public void Acknowledge(){
        SetMenuState(MenuState.Acknowledge);
    }

    public void Back(){
        SetMenuState(MenuState.menu);
    }



    public void QuitGame(){
        Application.Quit();
    }



}
