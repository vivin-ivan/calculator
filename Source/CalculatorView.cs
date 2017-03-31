// ****************************************************************************
// Calculator - written by Ivan Kotchouro
// View.cs handles user input, display logic, and issues commands to controller
// ****************************************************************************

namespace IvanCalculator
{
    using System;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            InputManager inputManager = new InputManager();
            inputManager.Start();
        }
    }
    
    public class InputManager
    {
        Invoker controller;
        Calculator calculator;
        View consoleView;

        Command removeLastCharCommand;
        Command clearLastNumCommand;
        Command clearAllCommand;
        Command evaluateCommand;
        Command addInputCommand;

        public void Start()
        {
            controller = new Invoker();
            calculator = new Calculator();
            consoleView = new View(calculator);

            removeLastCharCommand = new RemoveLastCharCommand(calculator);
            clearLastNumCommand = new ClearLastNumberCommand(calculator);
            clearAllCommand = new ClearAllCommand(calculator);
            evaluateCommand = new EvaluateCommand(calculator);

            ReadInput();
        }

        /// <summary>
        /// Listens for user key input and issues commands.
        /// </summary>
        public void ReadInput()
        {
            while (true)
            {
                consoleView.UpdateCurrentCursorPosition();

                ConsoleKeyInfo userInput = Console.ReadKey(false);
                switch (userInput.Key)
                {
                    case ConsoleKey.Q:
                        Environment.Exit(0);
                        break;

                    case ConsoleKey.Backspace:
                        controller.ExecuteCommand(removeLastCharCommand);
                        break;

                    case ConsoleKey.Enter:
                        controller.ExecuteCommand(evaluateCommand);
                        break;

                    case ConsoleKey.A:
                        controller.ExecuteCommand(clearAllCommand);
                        break;

                    case ConsoleKey.C:
                        controller.ExecuteCommand(clearLastNumCommand);
                        break;

                    default: // For all other keystrokes
                        addInputCommand = new AddInputCommand(calculator, userInput.KeyChar.ToString());
                        controller.ExecuteCommand(addInputCommand);
                        break;
                }
            }
        }
    }

    public class View
    {
        public enum CursorOffset { Current, OneToTheLeft, Leftmost }

        public struct Coords
        {
            public int x, y;

            public Coords(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        
        public static Coords OriginPos = new Coords(0, 0);
        public static Coords ResultPos = new Coords(16, 3);
        public static Coords InputPos = new Coords(0, 5);
        public static Coords ErrorPos = new Coords(0, 7);
        private Coords currentCursorPos;

        Calculator calculator;
        int currentResultStringLength = 0;
        int currentErrorStringLength = 0;


        public View(Calculator calcRef)
        {
            this.calculator = calcRef;
            calculator.ResultUpdatedEvent += DoUpdateResultLine;
            calculator.ErrorUpdatedEvent += DoUpdateErrorLine;
            calculator.InputUpdatedEvent += DoUpdateInputLine;
            RenderStaticText();
        }

        public void UpdateCurrentCursorPosition()
        {
            currentCursorPos = new Coords(Console.CursorLeft, Console.CursorTop);
        }
        
        private void DoUpdateResultLine(Calculator.ModelValueUpdateEventHandlerArgs e)
        {
            DisplayNewTextAt(ResultPos, currentResultStringLength, e.newValue);
            currentResultStringLength = e.newValue.Length;
        }

        private void DoUpdateErrorLine(Calculator.ModelValueUpdateEventHandlerArgs e)
        {
            DisplayNewTextAt(ErrorPos, currentErrorStringLength, e.newValue);
            currentErrorStringLength = e.newValue.Length;
        }

        private void DoUpdateInputLine(Calculator.ModelValueUpdateEventHandlerArgs e)
        {
            if (e.newValue.Length > 0)
            {
                Coords newCursorPos = new Coords(currentCursorPos.x - e.newValue.Length, currentCursorPos.y);
                DisplayNewTextAt(newCursorPos, e.newValue.Length, e.newValue);
                Console.SetCursorPosition(newCursorPos.x, newCursorPos.y);
            }
            else // Special case, on bad user input.
            {
                WriteBlanksAtPosition(currentCursorPos, 1);
            }
        }
        
        private void DisplayNewTextAt(Coords targetPos, int lengthToOverwrite, string newTextValue)
        {
            // Temp store current cursor position and overwrite currently displayed text with blanks.
            Coords cachedCursorPos = new Coords(Console.CursorLeft, Console.CursorTop);
            WriteBlanksAtPosition(targetPos, lengthToOverwrite + 1);
            // Update view with new text and reset cursor position.
            Console.Write(newTextValue);
            Console.SetCursorPosition(cachedCursorPos.x, cachedCursorPos.y);
        }

        /// <summary>
        /// Overwrites provided position with a blank space, effectively erasing that character in the console view.
        /// Cursor remains at specified position.
        /// </summary>
        public static void WriteBlanksAtPosition(Coords targetPos, int lengthOfBlankString)
        {
            int x = targetPos.x, y = targetPos.y;
            Console.SetCursorPosition(x, y);

            if (lengthOfBlankString > 0)
            {
                StringBuilder blankString = new StringBuilder();
                blankString.Append(' ', lengthOfBlankString);
                Console.Write(blankString.ToString());
                Console.SetCursorPosition(x, y);
            }
        }

        public void RenderStaticText()
        {
            Console.SetCursorPosition(OriginPos.x, OriginPos.y);
            Console.WriteLine("********************************************************************************");
            Console.WriteLine("*                                Calculator v1.0!                              *");
            Console.WriteLine("********************************************************************************");
            Console.WriteLine("Current Result: ");
            Console.WriteLine("Input: ");
            Console.WriteLine();
            Console.WriteLine("________________________________________________________________________________");
            // Errors on this line
            Console.SetCursorPosition(InputPos.x, InputPos.y);
        }
    }
}