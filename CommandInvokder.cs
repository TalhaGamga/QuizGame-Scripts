using System.Collections;
using System.Collections.Generic;

public class CommandInvoker
{
    private Stack<ICommand> commandHistory;

    public CommandInvoker()
    {
        commandHistory = new Stack<ICommand>();
    }

    public void Execute(ICommand command)
    {
        command.Execute();
    }

    public void UndoLastCommand()
    {
        if (commandHistory.Count > 0)
        {
            ICommand lastCommand = commandHistory.Pop();
            lastCommand.Undo();
        }
    }
}