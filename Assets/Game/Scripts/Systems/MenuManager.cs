using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
	public MenuClassifier mLoadingMenu;

	private readonly Dictionary<string, Menu> mMenus = new Dictionary<string, Menu>();

	public void AddMenu(Menu pMenu)
	{
		if (!mMenus.ContainsKey(pMenu.mMenuClassifier.mMenuName))
		{
            mMenus.Add(pMenu.mMenuClassifier.mMenuName, pMenu);
		}
		else
		{
			Debug.LogError("Multiple menus being added: " + pMenu.mMenuClassifier.mMenuName);
		}
	}

	public T GetMenu<T>(MenuClassifier pClassifier) where T : Menu
	{
		Menu aMenu;
		if (mMenus.TryGetValue(pClassifier.mMenuName, out aMenu))
		{
			return aMenu as T;
		}
		Debug.LogError("Menu does not exist: " + pClassifier.mMenuName);
		return null;
	}

	public void ShowMenu(MenuClassifier pClassifier, string pOptions = "")
	{
		Menu aMenu;
		if (mMenus.TryGetValue(pClassifier.mMenuName, out aMenu))
		{
            aMenu.ShowMenu(pOptions);
		}
	}

	public void HideMenu(MenuClassifier pClassifier, string pOptions = "")
	{
        Menu aMenu;
        if (mMenus.TryGetValue(pClassifier.mMenuName, out aMenu))
        {
            aMenu.HideMenu(pOptions);
        }
    }

    public void ShowLoad(string pOptions = "")
    {
        ShowMenu(mLoadingMenu, pOptions);
    }

    public void HideLoad(string pOptions = "")
    {
        HideMenu(mLoadingMenu, pOptions);
    }
}
