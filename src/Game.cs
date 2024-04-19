using System;
using System.Reflection.Metadata;

class Game
{
	// Private fields
	private Parser parser;
	private Player player;
	Item portalgun = new Item(15, "portalgun");
	Item medkit = new Item(20, "Medkit");

	// Constructor
	public Game()
	{
		parser = new Parser();
		player = new Player();
		CreateRooms();
	}

	// Initialise the Rooms (and the Items)
	private void CreateRooms()
	{
		// Create the rooms
		Room outside = new Room("outside the main entrance of the of a facility (help for  more details on the lore)");
		Room mainhall = new Room("in the main hallway");
		Room canteen = new Room("in the canteen");
		Room lab = new Room("in a strange research lab");
		Room office = new Room("in the computing admin office");
		Room hallway = new Room("in the other hallway");
		Room up = new Room("on the second floor of the facility");
		Room containmentroom = new Room("in the storage room");
		

		outside.AddExit("mainhall", mainhall);
		outside.AddExit("lab", lab);
		outside.AddExit("canteen", canteen);

		mainhall.AddExit("outside", outside);
		mainhall.AddExit("door", hallway);

		hallway.AddExit("mainhall", mainhall);
		hallway.AddExit("stairs", up);

		up.AddExit("down", hallway);
		up.AddExit("containmentroom", containmentroom);

		containmentroom.AddExit("hallway", hallway);

		canteen.AddExit("outside", outside);

		lab.AddExit("outside", outside);
		lab.AddExit("office", office);

		office.AddExit("lab", lab);


		mainhall.Chest.Put("portalgun", portalgun);
		lab.Chest.Put("medkit", medkit);

		player.CurrentRoom = outside;
	}
//  Main play routine. Loops until end of play.
	public void Play()
	{
		PrintWelcome();

// Enter the main command loop. Here we repeatedly read commands and
		// execute them until the player wants to quit.
		bool finished = false;
		while (!finished)
		{
			Command command = parser.GetCommand();
			finished = ProcessCommand(command);
		}
		Console.WriteLine("Bedankt voor het spelen.");
		Console.WriteLine("Druk [Enter] om verder te gaan.");
		Console.ReadLine();
	}

	// Print out the opening message for the player.
	private void PrintWelcome()
	{
		Console.WriteLine();
		// Console.WriteLine("");
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));
	}
// Given a command, process (that is: execute) the command.
	// If this command ends the game, it returns true.
	// Otherwise false is returned.
private bool ProcessCommand(Command command)
{
    bool wantToQuit = false;

    if (!player.IsAlive() && command.CommandWord != "quit")
    {
        Console.WriteLine("You bled out, you died... you failed your mission..");
		Console.WriteLine("You can only use the command:");
		Console.WriteLine("quit");
        return wantToQuit;
    }

    if(command.IsUnknown())
    {
        Console.WriteLine("I don't know what you mean...");
        return wantToQuit; 
    }

    switch (command.CommandWord)
    {
        case "help":
            PrintHelp();
            break;
        case "look":
            Look();
            break;
		case "take":
			Take(command);
			break;
		case "drop":
			Drop(command);
			break;
        case "status":
            Health();
            break;
        case "go":
            GoRoom(command);
            break;
		case "use":
			UseItem(command);
			break;
        case "quit":
            wantToQuit = true;
            break;
    }

    return wantToQuit;
}

// ######################################
	// implementations of user commands:
	// ######################################
	
	// Print out some help information.
	// Here we print the mission and a list of the command words.
	private void PrintHelp()
	{
		Console.WriteLine("You fought in a futuristic war. They hurt you badly and you used your emergency teleporter. You are safe for now...");
		Console.WriteLine("sadly for you, your emergency teleporter didnt work correctly and you got teleported to another universe");
		Console.WriteLine("You gotta find a new one. be fast.. Your slowly bleeding out..");
		Console.WriteLine();
		parser.PrintValidCommands();
	}

	private void Look()
	{
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));

		Dictionary<string, Item> roomItems = player.CurrentRoom.Chest.GetItems();
		if (roomItems.Count > 0)
		{
			Console.WriteLine("Items in this room:");
			foreach (var itemEntry in roomItems)
			{
				Console.WriteLine($"{itemEntry.Value.Description} - ({itemEntry.Value.Weight} kg)");
			}
		}
	}


private void Take(Command command)
{
    if (!command.HasSecondWord())
    {
        Console.WriteLine("Take what?");
        return;
    }

    string itemName = command.SecondWord.ToLower();

    bool success = player.TakeFromChest(itemName);

    if (success && itemName == "portalgun")
    {
        Console.WriteLine("You found a weird gun.. It says you can teleport? You read the sign thats in the room..");
        Console.WriteLine("It does teleport! you use the gun and you teleport.. you suddenly find yourself in a vortex..");
		Console.WriteLine("You get teleported to your timeline and you see your own facility.. its calm... you look around.. you walk into the facility and see everyone else..");
        Console.WriteLine("Your back home!! [the end, thanks for playing!]");
        Console.WriteLine("Press [Enter] to continue.");
        Console.ReadLine();
        Environment.Exit(0); // This will end the game
    }
}


	private void Drop(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("Drop what?");
			return;
		}

		string itemName = command.SecondWord.ToLower();

		bool success = player.DropToChest(itemName);


	}

	private void Health()
	{
		Console.WriteLine($"Your health is: {player.GetHealth()}");

		Dictionary<string, Item> items = player.GetItems();

		if (items.Count > 0)
		{
			Console.WriteLine("Your current items:");

			// Iterate over elk item in player zijn inv
			foreach (var itemEntry in items)
			{
				Console.WriteLine($"- {itemEntry.Key}: ({itemEntry.Value.Weight} kg)");
			}
		}
		else
		{
			Console.WriteLine("You have no items in your inventory.");
		}
	}

	private void GoRoom(Command command)
	{
		if(!command.HasSecondWord())
		{
			Console.WriteLine("Go where?");
			// On no params
			return;
		}

		string direction = command.SecondWord;

		Room nextRoom = player.CurrentRoom.GetExit(direction);
		if (nextRoom == null)
		{
			Console.WriteLine("There is no door to "+direction+"!");
			return;
		}

		player.Damage(25);
		player.CurrentRoom = nextRoom;
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));
		
		if (!player.IsAlive()) 
		{
			Console.WriteLine("Your vision blurs, the world fades. Your wounds draining your strength. You collapse, you have bled out..");
		}
	}

	private void UseItem(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("Use what?");
			return;
		}

		string itemName = command.SecondWord.ToLower();

		player.Use(itemName);
	}

}

