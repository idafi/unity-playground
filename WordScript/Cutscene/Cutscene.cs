using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an in-game cutscene, through which IActors can recite Scripts
/// via the Wordsbox.
/// </summary>
public class Cutscene : MonoBehaviour
{
	Script script;
	Dictionary<string, IActor> actors;

	/// <summary>
	/// Creates a new Cutscene, using the given Script and IActors.
	/// </summary>
	/// <param name="script">The Script to use.</param>
	/// <param name="actors">The IActors to use.</param>
	/// <returns>The new Cutscene.</returns>
	public static Cutscene Create(Script script, params IActor[] actors)
	{
		GameObject go = new GameObject("cutscene");
		Cutscene c = go.AddComponent<Cutscene>();

		c.script = script;
		c.actors = new Dictionary<string, IActor>();

		foreach(IActor a in actors)
		{
			c.actors.Add(a.Name, a);
		}

		return c;
	}

	/// <summary>
	/// Begin this Cutscene.
	/// <para>IActors will automatically recite their lines through Wordsboxes,
	/// ending once the user has scrolled through all available dialogue.</para>
	/// </summary>
	public void Play()
	{
		StartCoroutine(_Play());
	}

	IEnumerator _Play()
	{
		foreach(Page page in script.Pages)
		{
			Wordsbox box = Wordsbox.Create(actors[page.Speaker]);

			// box will destroy itself when done
			yield return StartCoroutine(box.Print(page.Words));
		}

		GameObject.Destroy(this.gameObject);
	}
};