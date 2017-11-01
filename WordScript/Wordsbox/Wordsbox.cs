using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Widgets subclass for the Wordsbox settings.
/// </summary>
[Serializable]
public class WordsboxWidgets
{
	public RectTransform Widget;
	public Text TextMain;
	public Text TextName;
	public Image Portrait;
};

/// <summary>
/// Print settings subclass for the Wordsbox settings.
/// </summary>
[Serializable]
public class WordsboxPrintSettings
{
	public float NormalSpeed;
	public float FastSpeed;
	public Punctuation[] Punctuation;
};

/// <summary>
/// Animation settings subclass for the Wordsbox settings.
/// </summary>
[Serializable]
public class WordsboxAnimateSettings
{
	public float SlideTime;
	public float PageScrollTime;
}

/// <summary>
/// Uses a Script, optionally with an IActor, to display dialogue via a dynamically animated dialogue box.
/// <para>The Wordsbox can display and change an accompanying portrait using commands provided in the Script.</para>
/// Using Punctuation data, text speed can be altered on the fly, to better represent pauses and interruptions.
/// </summary>
public class Wordsbox : MonoBehaviour
{
	const string proceedButton = "Submit";

	/// <summary>
	/// Widgets settings.
	/// </summary>
	public WordsboxWidgets Widgets;

	/// <summary>
	/// Printing settings.
	/// </summary>
	public WordsboxPrintSettings PrintSettings;

	/// <summary>
	/// Animation settings.
	/// </summary>
	public WordsboxAnimateSettings AnimateSettings;

	IActor actor;
	InputDevice input;

	// temporary
	GUIStyle style;
	StringBuilder line;
	float textHeight;
	
	RectTransform widget
	{
		get { return Widgets.Widget; }
	}

	Text textMain
	{
		get { return Widgets.TextMain; }
	}

	Text textName
	{
		get { return Widgets.TextName; }
	}

	Image portrait
	{
		get { return Widgets.Portrait; }
	}

	Vector2 widgetClosedPosition
	{
		get { return (widget.anchoredPosition - new Vector2(0, widget.anchoredPosition.y + widget.sizeDelta.y)); }
	}

	float textBoxWidth
	{
		get { return textMain.rectTransform.rect.width; }
	}
	
	float textBoxHeight
	{
		get { return textMain.rectTransform.rect.height; }
	}
	
	/// <summary>
	/// Creates a new Wordbox, attached to the given IActor.
	/// </summary>
	/// <param name="actor">The actor to attach to the new Wordsbox.</param>
	/// <returns>The new Wordsbox.</returns>
	public static Wordsbox Create(IActor actor)
	{
		Debug.Assert(actor != null);
		
		Wordsbox wbox = UIMan.Templates.Wordsbox;

		if(wbox.widget)
		{
			if(wbox.textMain)
			{
				if(wbox.portrait)
				{
					string name = wbox.name;

					wbox = GameObject.Instantiate<Wordsbox>(wbox, null, true);
					wbox.name = name;

					wbox.actor = actor;

					wbox.input = new InputDevice();
					wbox.input.AddButton("Continue", "Submit");
					wbox.input.AddButton("PreventExamine", "Jump");

					wbox.style = wbox.GetTextStyle();
					wbox.line = new StringBuilder();
					wbox.textHeight = 0;

					wbox.textMain.text = "";
					wbox.textName.text = actor.Name;

					return wbox;
				}
				else
				{ Debug.LogError("Wordsbox: can't open; missing portrait reference"); }
			}
			else
			{ Debug.LogError("Wordsbox: can't open; missing grouped widget reference"); }
		}
		else
		{ Debug.LogError("Wordsbox: can't open; missing text reference"); }

		return null;
	}

	/// <summary>
	/// Prints the given Words to this Wordsbox.
	/// </summary>
	/// <param name="words">The Words to print.</param>
	public IEnumerator Print(Word[] words)
	{
		Debug.Assert(words != null);

		yield return StartCoroutine(SlideOpen());

		// reset from possible previous print
		line.Remove(0, line.Length);
		textHeight = 0;
		
		foreach(Word w in words)
		{
			switch(w.Type)
			{
				case WordType.Word:
					yield return StartCoroutine(PrintWord(w.Value + " "));
					break;
				case WordType.Portrait:
					ChangePortrait(w.Value);
					break;
				case WordType.Break:
					yield return StartCoroutine(NextPage());
					break;
			}
		}

		yield return StartCoroutine(WaitForButton(proceedButton));
		Reset();

		yield return StartCoroutine(SlideClosed());

		input.Close();
		GameObject.Destroy(this.gameObject);
	}

	IEnumerator SlideOpen()
	{
		Vector2 target = widget.anchoredPosition;
		widget.anchoredPosition = widgetClosedPosition;

		yield return StartCoroutine(Slide(target));
	}

	IEnumerator SlideClosed()
	{
		Vector2 target = widgetClosedPosition;

		yield return StartCoroutine(Slide(target));
	}

	IEnumerator Slide(Vector2 target)
	{
		Vector2 start = widget.anchoredPosition;
		float t = 0;
		
		while(t < AnimateSettings.SlideTime)
		{
			yield return null;

			t += Time.deltaTime;
			widget.anchoredPosition = Vector2.Lerp(start, target, t / AnimateSettings.SlideTime);
		}
	}
	
	IEnumerator PrintWord(string word)
	{
		// try adding to line
		line.Append(word);
		
		// check new size
		Vector2 textSize = style.CalcSize(new GUIContent(line.ToString()));
		
		// if line width exceeds text box bounds, add a new line
		if(textSize.x > textBoxWidth)
		{
			textMain.text += "\n";
			textHeight += textSize.y * textMain.lineSpacing + 5;
			line.Remove(0, line.Length - word.Length);
		}
		
		// if line height exceeds text box bounds, "scroll" to next page after user input
		if(textHeight > textBoxHeight)
		{
			yield return StartCoroutine(NextPage());
		}
		
		// print the word
		foreach(char c in word)
		{
			textMain.text += c;
			yield return new WaitForSeconds(1 / GetSpeed(c, word));
		}
	}
	
	void ChangePortrait(string sprite)
	{
		portrait.sprite = actor.Portrait.GetSprite(sprite);
	}
	
	IEnumerator NextPage()
	{
		float t = 0;

		Vector2 start = textMain.rectTransform.anchoredPosition;
		Vector2 target = textMain.rectTransform.anchoredPosition + new Vector2(0, textBoxHeight);

		yield return StartCoroutine(WaitForButton(proceedButton));
		
		while(t < AnimateSettings.PageScrollTime)
		{
			yield return null;
			t += Time.deltaTime;

			Vector2 pos = Vector2.Lerp(start, target, t / AnimateSettings.PageScrollTime);
			textMain.rectTransform.anchoredPosition = pos;
		}

		Reset();
		textMain.rectTransform.anchoredPosition = start;
	}
	
	float GetSpeed(char character, string word)
	{
		Punctuation p = Array.Find<Punctuation>(PrintSettings.Punctuation, x => x.Character == character);
		bool usePunctuation = (p != null && !p.IsException(word));

		if(Input.GetButton(proceedButton))
		{ return (usePunctuation) ? p.FastSpeed : PrintSettings.FastSpeed; }
		else
		{ return (usePunctuation) ? p.NormalSpeed : PrintSettings.NormalSpeed; }
	}

	IEnumerator WaitForButton(string button)
	{
		yield return new WaitUntil(() => Input.GetButtonDown(button));
		yield return null;
	}

	GUIStyle GetTextStyle()
	{
		GUIStyle wboxStyle = new GUIStyle();
		wboxStyle.font = textMain.font;
		wboxStyle.fontSize = textMain.fontSize;
		wboxStyle.fontStyle = textMain.fontStyle;
		wboxStyle.alignment = textMain.alignment;
		wboxStyle.wordWrap = false;

		return wboxStyle;
	}

	void Reset()
	{
		textMain.text = "";
		textHeight = 0;
		line.Remove(0, line.Length);
	}
};