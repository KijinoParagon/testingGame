using Godot;
using System;

public partial class OptionsMenu : Control
{
	ItemList ControlsList;
	Panel ControlsPanel;
	private bool mappingActions = false;
	private string actionBeingMapped;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Set up control list
		ControlsList = this.GetNode<ItemList>("ControlsPanel/ControlsList");
		ControlsPanel = this.GetNode<Panel>("ControlsPanel");
		ControlsList.MaxColumns = 2;
		ControlsList.SameColumnWidth = true;
		ControlsList.FixedColumnWidth = 200;
		ControlsPanel.Visible = false;
		ControlsList.FocusMode = FocusModeEnum.Click;
		ControlsList.AllowSearch = false;

		foreach (StringName actionName in InputMap.GetActions())
		{
			if (actionName.ToString().Contains("ui_"))      //Erase built in controls
			{
				InputMap.EraseAction(actionName);
			}

		}

		actionBeingMapped = null;

		this.GetNode<Button>("MainPanel/ControlsButton").Pressed += toggleControlsMenu;

		CreateControlList();

		//How to handle clicking the items
		ControlsList.ItemSelected += new ItemList.ItemSelectedEventHandler(handleControlSelect);
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void toggleControlsMenu()
	{
		if (ControlsPanel.Visible)
		{
			ControlsPanel.Visible = false;
		}
		else
		{
			ControlsPanel.Visible = true;
		}
	}
	private void handleControlSelect(long index)
	{
		mappingActions = false;
		actionBeingMapped = null;
		int i = (int)index;
		string actionName = "";
		if (i % 2 == 0) //Left side of list, this means it's an action
		{
			actionName = ControlsList.GetItemText(i);
			actionName = actionName.Replace("Gorilla ", "G");
			if (actionName == "")
			{
				return;
			}
			actionBeingMapped = actionName;
			mappingActions = true;
		}
		else            //Right side of list, this is an event
		{
			//Search for the associated action name
			int j = i - 1;
			while (actionName == "")
			{
				actionName = ControlsList.GetItemText(j);
				j -= 2;
			}

			actionName = actionName.Replace("Gorilla ", "G");

			if (ControlsList.GetItemText(i) == "(Unbound)")             //If it's unbound, then we'll bind it
			{
				actionBeingMapped = actionName;
				mappingActions = true;
				return;
			}

			//Find the actual event and remove it
			foreach (InputEvent e in InputMap.ActionGetEvents(actionName))
			{
				if (e.AsText() == ControlsList.GetItemText(i))
				{
					InputMap.ActionEraseEvent(actionName, e);
					ControlsList.Clear();
					CreateControlList();
					break;
				}
			}

		}
	}
	private void CreateControlList()
	{
		//Loop through the actions to create the list of controls
		foreach (StringName actionName in InputMap.GetActions())
		{
			bool first = true;

			if (actionName.ToString()[0] == 'G' && actionName.ToString() != "Guard")            //Because I'm lazy and don't want to go rename actions like gwalk to gorilla walk
			{
				ControlsList.AddItem("Gorilla " + actionName.ToString()[1..]);
			}
			else
			{
				ControlsList.AddItem(actionName);      //Add the action name to the list
			}

			//Get the events for the current action and map them to new list items
			foreach (InputEvent i in InputMap.ActionGetEvents(actionName))
			{
				if (!first)
				{
					ControlsList.AddItem("", null, false);      //To insert blank spaces for actions with multiple events
				}
				else
				{
					first = false;
				}
				ControlsList.AddItem(i.AsText());
			}

			if (first)
			{
				ControlsList.AddItem("(Unbound)");
			}
		}
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && mappingActions)
			{
				InputMap.ActionAddEvent(actionBeingMapped, @event);
				ControlsList.Clear();
				CreateControlList();
			}
			mappingActions = false;
			actionBeingMapped = null;
		}	
	}
}
