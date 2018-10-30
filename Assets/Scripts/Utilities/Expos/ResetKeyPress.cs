using UnityEngine;

public class ResetKeyPress : MonoBehaviour
{
	[SerializeField] private KeyCode loadGame;
    [Tooltip("If left empty will reload this scene")]
	[SerializeField] private string levelName;

	[SerializeField] private KeyCode loadKids;
	[Tooltip("If left empty will reload this scene")]
	[SerializeField] private string kidsName;

    private void Update()
    {
        if (Input.GetKeyDown(loadGame))
        {
            if (string.IsNullOrEmpty(levelName))
                GameController.Instance.ReloadLevel();
            else
                GameController.Instance.LoadLevel(levelName);
        }

		if (Input.GetKeyDown(loadKids))
		{
			if (string.IsNullOrEmpty(levelName))
				GameController.Instance.ReloadLevel();
			else
				GameController.Instance.LoadLevel(kidsName);
		}
    }
}
