using System.Collections.Generic;

public class CommandLibrary
{
    private readonly List<string> validCommands;

    // Constructor - initialise the command words.
    public CommandLibrary()
    {
        validCommands = new List<string>();

        validCommands.Add("help");
        validCommands.Add("go");
        validCommands.Add("quit");
        validCommands.Add("look");
        validCommands.Add("status");
        validCommands.Add("take");
        validCommands.Add("drop");
        validCommands.Add("use");
    }

    // Check whether a given string is a valid command word.
    // Return true if it is, false if it isn't.
    public bool IsValidCommandWord(string instring)
    {
        return validCommands.Contains(instring);
    }

    // returns valid commands
    public string GetCommandsString()
    {
        return string.Join(", ", validCommands);
    }
}