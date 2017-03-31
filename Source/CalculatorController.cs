// ****************************************************************************
// Calculator - written by Ivan Kotchouro
// Controller.cs handles invoking commands from View to Model
// ****************************************************************************

namespace IvanCalculator
{
    public class Invoker
    {
        public void ExecuteCommand(Command commandToExecute)
        {
            commandToExecute.Execute();
        }
    }

    #region Commands
    public abstract class Command
    {
        protected Calculator commandTarget;

        public Command(Calculator target)
        {
            this.commandTarget = target;
        }

        public abstract void Execute();
    }

    public class AddInputCommand : Command
    {
        private string input;

        public AddInputCommand(Calculator target, string input) : base(target)
        {
            this.input = input;
        }

        public override void Execute()
        {
            this.commandTarget.AppendToInputString(this.input);
        }
    }

    public class RemoveLastCharCommand : Command
    {
        public RemoveLastCharCommand(Calculator target) : base(target) { }

        public override void Execute()
        {
            this.commandTarget.RemoveLastCharFromInputString();
        }
    }

    public class ClearLastNumberCommand : Command
    {
        public ClearLastNumberCommand(Calculator target) : base(target) { }

        public override void Execute()
        {
            this.commandTarget.ClearLastNumber();
        }
    }

    public class ClearAllCommand : Command
    {
        public ClearAllCommand(Calculator target) : base(target) { }

        public override void Execute()
        {
            this.commandTarget.ClearAll();
        }
    }

    public class EvaluateCommand : Command
    {
        public EvaluateCommand(Calculator target) : base(target) { }

        public override void Execute()
        {
            this.commandTarget.Evaluate();
        }
    }
    #endregion
}