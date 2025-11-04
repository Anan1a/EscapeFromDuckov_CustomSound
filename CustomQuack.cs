using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Duckov;
using Duckov.Modding;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CustomQuack;

public class ModBehaviour : Duckov.Modding.ModBehaviour
{
	private InputAction newAction = new InputAction();

	private List<string> soundPath = new List<string>();

	private void Awake()
	{
		Debug.Log("CustomQuack Loaded!!!");
	}

	private void OnEnable()
	{
		InitSoundFilePath();
		if (soundPath.Count < 1)
		{
			Debug.Log("CustomQuack：声音文件不存在！！该 Mod 不会生效！！");
			return;
		}
		InputAction inputAction = GameManager.MainPlayerInput.actions.FindAction("Quack");
		inputAction.Disable();
		newAction.AddBinding(inputAction.controls[0]);
		newAction.performed += PlayTest;
		newAction.Enable();
		Debug.Log("CustomQuack：载入完成。");
	}

	private void OnDisable()
	{
		newAction.performed -= PlayTest;
		newAction.Disable();
		GameManager.MainPlayerInput.actions.FindAction("Quack").Enable();
	}

	private void PlayTest(InputAction.CallbackContext context)
	{
		if (CharacterMainControl.Main != null)
		{
			int index = Random.Range(0, soundPath.Count);
			AudioManager.PostCustomSFX(soundPath[index]);
		}
	}

	public string GetDllDirectory()
	{
		return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
	}

	private void InitSoundFilePath()
	{
		for (int i = 0; i < 99; i++)
		{
			string text = GetDllDirectory() + "/" + i + ".wav";
			if (File.Exists(text))
			{
				soundPath.Add(text);
				Debug.Log("CustomQuack : 已加载音频 " + text);
			}
		}
	}
}
