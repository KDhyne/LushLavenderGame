using UnityEngine;

public class MainMenu : MonoBehaviour
{
	enum MenuState
	{
        Intro,
		Main,
		Levels,
		Credits
	}

	MenuState currentMenuState;
    public GUISkin GuiSkin;
    public GameObject Credits;
    private GameObject soundManager;

    public bool Muted = false;

	// Use this for initialization
	void Start ()
	{
	    soundManager = GameObject.Find("SoundManager");

		currentMenuState = MenuState.Main;
	}

    void Update()
    {
        soundManager.GetComponent<SoundManager>().muted = this.Muted;
    }

	void OnGUI()
	{
		switch (currentMenuState)
		{
            case MenuState.Intro:
                break;

			case MenuState.Main:
                /*GUI.Label(new Rect(Screen.width/2 - 276, 50, 553, 103), "", GuiSkin.GetStyle("Title") );

                if (GUI.Button(new Rect((Screen.width / 2f) - 211, (Screen.height / 2f) - 50, 422, 63), "", GuiSkin.GetStyle("Start Button")))
					currentMenuState = MenuState.Levels;

                GUI.Label(new Rect(Screen.width / 2 - 200, 140, 400, 50), "Grab the Invasive Species!", GuiSkin.GetStyle("Instructions"));

                if (GUI.Button(new Rect((Screen.width / 2f) - 211, (Screen.height / 2f) + 125, 422, 63), "", GuiSkin.GetStyle("Credits Button")))
				{
					currentMenuState = MenuState.Credits;
				    iTween.MoveTo(Credits, new Vector3(0, 0, -5), 2.5f);
				}*/
				break;

			case MenuState.Levels:
                /*GUI.Label(new Rect(Screen.width / 2 - 276, 50, 553, 103), "", GuiSkin.GetStyle("Title"));

                if (GUI.Button(new Rect((Screen.width / 2f) - 211, (Screen.height / 2f) - 50, 422, 63), "", GuiSkin.GetStyle("Easy Button")))
                    Application.LoadLevel("Invasive Easy");

                if (GUI.Button(new Rect((Screen.width / 2f) - 211, (Screen.height / 2f) + 25, 422, 63), "", GuiSkin.GetStyle("Medium Button")))
                    Application.LoadLevel("Invasive Medium");

                if (GUI.Button(new Rect((Screen.width / 2f) - 211, (Screen.height / 2f) + 100, 422, 63), "", GuiSkin.GetStyle("Hard Button")))
                    Application.LoadLevel("Invasive Hard");

                if (GUI.Button(new Rect((Screen.width / 2f) - 211, (Screen.height / 2f) + 175, 422, 63), "", GuiSkin.GetStyle("Back Button")))
					currentMenuState = MenuState.Main;*/

				break;

			case MenuState.Credits:
				/*if (GUI.Button(new Rect((Screen.width / 2f) - 105, Screen.height - 50, 211, 32), "", GuiSkin.GetStyle("Back Button")))
				{
					currentMenuState = MenuState.Main;
                    iTween.MoveTo(Credits, new Vector3(120, 0, -5), 2.5f);
				}*/
				break;
		}

	    //this.Muted = GUI.Toggle(new Rect(Screen.width - 180, 10, 50, 50), Muted, "", GuiSkin.GetStyle("Mute Button"));
	}
}
