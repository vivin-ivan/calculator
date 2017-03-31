// ****************************************************************************
// Calculator - written by Ivan Kotchouro
// Model.cs contains the calculator, input parser, and shunting yard algorithm
// ****************************************************************************

namespace IvanCalculator
{
    using System;
    using System.Collections;
    using System.Text;
    using System.Text.RegularExpressions;
    
    public class Calculator
    {
        public struct ModelValueUpdateEventHandlerArgs { public string newValue; }
        public delegate void ModelValueUpdateEventHandler(ModelValueUpdateEventHandlerArgs e);
        public event ModelValueUpdateEventHandler ResultUpdatedEvent;
        public event ModelValueUpdateEventHandler ErrorUpdatedEvent;
        public event ModelValueUpdateEventHandler InputUpdatedEvent;

        public double currentResult;
        public StringBuilder inputStringBuilder;
        string validInput = " 1234567890.()Xx!*/+-";

        public Calculator()
        {
            UpdateCalculatedResult(0);
            inputStringBuilder = new StringBuilder();
        }

        public void AppendToInputString(string input)
        {
            if (validInput.Contains(input))
            {
                inputStringBuilder.Append(input);
            }
            else
            {
                OnInputUpdate(SetEventArgs(""));
            }
        }

        public void RemoveLastCharFromInputString()
        {
            if (inputStringBuilder.Length > 0)
            {
                inputStringBuilder.Remove(inputStringBuilder.Length - 1, 1);
                OnInputUpdate(SetEventArgs(BuildInputEventString(1)));
            }
        }

        public void ClearLastNumber()
        {
            // Removes last number entered and any operators on either side of that number.

            string tempString = inputStringBuilder.ToString();
            string numericalChars = "1234567890.";
            bool isNumberDiscovered = false;
            bool isNumberFinalized = false;

            if (tempString.Length > 0)
            {
                for (int i = tempString.Length - 1; i >= 0; i--)
                {
                    if (isNumberDiscovered && isNumberFinalized && numericalChars.Contains(tempString[i].ToString()))
                    {
                        // Number to the left of the number to be removed was found.
                        int lengthToBeRemoved = (tempString.Length - 1) - i;
                        inputStringBuilder.Remove(i + 1, lengthToBeRemoved);
                        OnInputUpdate(SetEventArgs(BuildInputEventString(lengthToBeRemoved)));
                        return;
                    }
                    else if (isNumberDiscovered && !isNumberFinalized && !numericalChars.Contains(tempString[i].ToString()))
                    {
                        // Reached the end of the number to be removed by finding an operator.
                        isNumberFinalized = true;
                    }
                    else if (!isNumberDiscovered && numericalChars.Contains(tempString[i].ToString()))
                    {
                        // Just discovered the number to remove.
                        isNumberDiscovered = true;
                    }
                }

                // Loop found the number but didn't finish, clear line and string buffer.
                OnInputUpdate(SetEventArgs(BuildInputEventString(inputStringBuilder.Length)));
                inputStringBuilder.Clear();
            }
            else
            {
                // Input is empty, so clear result value.
                UpdateCalculatedResult(0);
                OnInputUpdate(SetEventArgs(""));
            }
        }

        public void ClearAll()
        {
            UpdateCalculatedResult(0);
            OnInputUpdate(SetEventArgs(BuildInputEventString(inputStringBuilder.Length)));
            inputStringBuilder.Clear();
            OnErrorUpdate(SetEventArgs(" "));
        }

        public void Evaluate()
        {
            string toBeParsed = inputStringBuilder.ToString();
            OnInputUpdate(SetEventArgs(BuildInputEventString(inputStringBuilder.Length)));
            inputStringBuilder.Clear();
            
            // If input string is empty, clear all
            if (toBeParsed.Length == 0)
            {
                ClearAll();
                return;
            }

            // Clear any error on screen
            OnErrorUpdate(SetEventArgs(" "));

            Queue postFixTokenQueue = new Queue();
            if (InputParser.ConvertToRPN(toBeParsed, currentResult, out postFixTokenQueue))
            {
                Stack result = new Stack();
                double oper1 = 0.0, oper2 = 0.0;
                RPNToken token = new RPNToken();

                foreach (object obj in postFixTokenQueue)
                {
                    token = (RPNToken)obj;
                    switch (token.tokenType)
                    {
                        case TokenType.Number:
                            result.Push(token.tokenValue);
                            break;

                        case TokenType.Add:
                            if (result.Count >= 2)
                            {
                                oper2 = (double)result.Pop();
                                oper1 = (double)result.Pop();
                                result.Push(CalculatorOperations.Add(oper1, oper2));
                            }
                            else
                            {
                                OnErrorUpdate(SetEventArgs("Addition evaluation error!"));
                                return;
                            }
                            break;

                        case TokenType.Subtract:
                            if (result.Count >= 2)
                            {
                                oper2 = (double)result.Pop();
                                oper1 = (double)result.Pop();
                                result.Push(CalculatorOperations.Subtract(oper1, oper2));
                            }
                            else
                            {
                                OnErrorUpdate(SetEventArgs("Subtraction evaluation error!"));
                                return;
                            }
                            break;

                        case TokenType.Multiply:
                            if (result.Count >= 2)
                            {
                                oper2 = (double)result.Pop();
                                oper1 = (double)result.Pop();
                                result.Push(CalculatorOperations.Multiply(oper1, oper2));
                            }
                            else
                            {
                                OnErrorUpdate(SetEventArgs("Multiplication evaluation error!"));
                                return;
                            }
                            break;

                        case TokenType.Divide:
                            if (result.Count >= 2)
                            {
                                oper2 = (double)result.Pop();
                                oper1 = (double)result.Pop();
                                result.Push(CalculatorOperations.Divide(oper1, oper2));
                            }
                            else
                            {
                                OnErrorUpdate(SetEventArgs("Division evaluation error!"));
                                return;
                            }
                            break;

                        case TokenType.Factorial:
                            if (result.Count >= 1)
                            {
                                oper1 = (double)result.Pop();
                                result.Push(CalculatorOperations.Factorial(Convert.ToInt32(oper1)));
                            }
                            else
                            {
                                OnErrorUpdate(SetEventArgs("Factorial evaluation error!"));
                                return;
                            }
                            break;

                        case TokenType.Reciprocal:
                            if (result.Count >= 1)
                            {
                                oper1 = (double)result.Pop();
                                result.Push(CalculatorOperations.Reciprocal(oper1));
                            }
                            else
                            {
                                OnErrorUpdate(SetEventArgs("Reciprocal evaluation error!"));
                                return;
                            }
                            break;

                        case TokenType.Negate:
                            if (result.Count >= 1)
                            {
                                oper1 = (double)result.Pop();
                                result.Push(CalculatorOperations.Negate(oper1));
                            }
                            else
                            {
                                OnErrorUpdate(SetEventArgs("Negation evaluation error!"));
                                return;
                            }
                            break;
                    }
                }

                if (result.Count == 1)
                {
                    double foo = (double)result.Peek();
                    UpdateCalculatedResult((double)result.Pop());
                }
                else
                {
                    OnErrorUpdate(SetEventArgs("Input error - too many values!"));
                }
            }
            else // RPN conversion returned false
            {
                OnErrorUpdate(SetEventArgs("Input error - mismatched parenthesis!"));
            }
        }

        public void UpdateCalculatedResult(double newResult)
        {
            currentResult = newResult;
            OnResultUpdate(SetEventArgs(newResult.ToString()));
        }

        public string BuildInputEventString(int length)
        {
            StringBuilder eventString = new StringBuilder();
            return eventString.Append(' ', length).ToString();
        }

        public ModelValueUpdateEventHandlerArgs SetEventArgs(string newViewValue)
        {
            ModelValueUpdateEventHandlerArgs e;
            e.newValue = newViewValue;
            return e;
        }

        public void OnResultUpdate(ModelValueUpdateEventHandlerArgs e)
        {
            if (ResultUpdatedEvent != null)
                ResultUpdatedEvent(e);
        }

        public void OnErrorUpdate(ModelValueUpdateEventHandlerArgs e)
        {
            if (ErrorUpdatedEvent != null)
                ErrorUpdatedEvent(e);
        }

        public void OnInputUpdate(ModelValueUpdateEventHandlerArgs e)
        {
            if (InputUpdatedEvent != null)
                InputUpdatedEvent(e);
        }
    }

    public static class CalculatorOperations
    {
        public static double Add(double operand1, double operand2)
        {
            return operand1 + operand2;
        }

        public static double Subtract(double operand1, double operand2)
        {
            return operand1 - operand2;
        }

        public static double Multiply(double operand1, double operand2)
        {
            return operand1 * operand2;
        }

        public static double Divide(double operand1, double operand2)
        {
            return operand1 / operand2;
        }

        public static double Negate(double operand1)
        {
            return -operand1;
        }

        public static double Reciprocal(double operand1)
        {
            return 1 / operand1;
        }

        public static double Factorial(int operand1)
        {
            return RecursiveFactorialHelper(operand1);
        }

        public static int RecursiveFactorialHelper(int n)
        {
            if (n == 0 || n == 1)
            {
                return 1;
            }

            return RecursiveFactorialHelper(n - 1) * n;
        }
    }

    public enum TokenType
    {
        None,
        Number,
        Add,
        Subtract,
        Multiply,
        Divide,
        Factorial,
        Reciprocal,
        Negate,
        LeftParenthesis,
        RightParenthesis
    }

    public struct RPNToken
    {
        // If token is a number, then the value is the numerical value.
        // If token is an operator, then the value is precedence level.

        public TokenType tokenType;
        public double tokenValue;

        public RPNToken(TokenType tokenType, double tokenValue)
        {
            this.tokenType = tokenType;
            this.tokenValue = tokenValue;
        }
    }

    public static class InputParser
    {
        /// <summary>
        /// Converts infix notation to reverse polish notation using shunting-yard algorithm.
        /// </summary>
        /// <returns>True if conversion was successful</returns>
        public static bool ConvertToRPN(string rawInputString, double currentResult, out Queue output)
        {
            string[] parsedStringArray = Parse(rawInputString);
            Stack ops = new Stack();
            output = new Queue();

            RPNToken token, opstoken;
            for (int i = 0; i < parsedStringArray.Length; ++i)
            {
                token = new RPNToken(TokenType.None, 0.0);

                try
                {
                    // If the element is a number, then add it to the output queue.
                    token.tokenValue = double.Parse(parsedStringArray[i]);
                    token.tokenType = TokenType.Number;
                    output.Enqueue(token);
                }
                catch
                {
                    switch (parsedStringArray[i])
                    {
                        case "+":
                            token.tokenType = TokenType.Add;
                            token.tokenValue = 1.0;
                            break;

                        case "-":
                            token.tokenType = TokenType.Subtract;
                            token.tokenValue = 1.0;
                            break;

                        case "*":
                            token.tokenType = TokenType.Multiply;
                            token.tokenValue = 2.0;
                            break;

                        case "/":
                            token.tokenType = TokenType.Divide;
                            token.tokenValue = 2.0;
                            break;

                        case "!":
                            token.tokenType = TokenType.Factorial;
                            token.tokenValue = 3.0;
                            break;

                        case "~":
                            token.tokenType = TokenType.Negate;
                            token.tokenValue = 3.0;
                            break;

                        case "$":
                            token.tokenType = TokenType.Reciprocal;
                            token.tokenValue = 3.0;
                            break;

                        case "(":
                            token.tokenType = TokenType.LeftParenthesis;
                            token.tokenValue = 4.0;
                            break;

                        case ")":
                            token.tokenType = TokenType.RightParenthesis;
                            token.tokenValue = 4.0;
                            break;
                    }

                    switch (token.tokenType)
                    {
                        case TokenType.Add:
                        case TokenType.Subtract:
                        case TokenType.Multiply:
                        case TokenType.Divide:
                        case TokenType.Factorial:
                        case TokenType.Reciprocal:
                        case TokenType.Negate:
                            // If the first element isn't a number, create and enqueue token for current result.
                            if (i == 0)
                            {
                                RPNToken firstToken = new RPNToken(TokenType.Number, currentResult);
                                output.Enqueue(firstToken);
                            }

                            // Operator precedence check
                            while (ops.Count > 0)
                            {
                                opstoken = (RPNToken)ops.Peek();
                                if (token.tokenValue <= opstoken.tokenValue && opstoken.tokenType != TokenType.LeftParenthesis)
                                    output.Enqueue(ops.Pop());
                                else break;
                            }

                            ops.Push(token);
                            break;

                        case TokenType.LeftParenthesis:
                            ops.Push(token);
                            break;

                        case TokenType.RightParenthesis:
                            // Pop operators to output until token is a left parenthesis.
                            while (ops.Count > 0)
                            {
                                opstoken = (RPNToken)ops.Peek();
                                if (opstoken.tokenType != TokenType.LeftParenthesis)
                                {
                                    output.Enqueue(ops.Pop());
                                }
                                else break;
                            }

                            if (ops.Count == 0)
                            {
                                return false;
                            }
                            else
                            {
                                // Remove left parenthesis.
                                ops.Pop();
                            }
                            break;
                    }
                }
            }

            while (ops.Count != 0)
            {
                opstoken = (RPNToken)ops.Pop();
                // If the operator token on the top of the stack is a parenthesis
                if (opstoken.tokenType == TokenType.LeftParenthesis)
                {
                    return false;
                }
                else
                {
                    output.Enqueue(opstoken);
                }
            }

            return true;
        }

        /// <summary>
        /// Uses regex to parse user input into a string array to be used by yard shunting algorithm
        /// </summary>
        public static string[] Parse(string rawInputString)
        {
            string tempBuffer = rawInputString.ToLower();

            // Remove all spaces.
            tempBuffer = Regex.Replace(tempBuffer, @"\s+", "");
            // Find all instances of 1/x and replace them with $
            tempBuffer = Regex.Replace(tempBuffer, @"(1/x)", " $ ");
            // Find numbers and separate them with spaces.
            tempBuffer = Regex.Replace(tempBuffer, @"(?<num>\d+(\.\d+)?)", " ${num} ");
            // Find operators and separate them with spaces: + - * / ! ( ).
            tempBuffer = Regex.Replace(tempBuffer, @"(?<ops>[-+*/!()])", " ${ops} ");
            // Next, figure out if the minus operators are binary or unary.
            // Convert all minus operators to a temporary value.
            tempBuffer = Regex.Replace(tempBuffer, "-", "MINUS");
            // Find MINUS surrounded by certain operators and a number. Move behind number for left association.
            tempBuffer = Regex.Replace(tempBuffer, @"((?<ops>[(x+*/]|MINUS)\s+MINUS\s+(?<num>(\d+(\.\d+)?)))",
                                                   "${ops} ${num} ~");
            // Replace the rest with a regular minus operator.
            tempBuffer = Regex.Replace(tempBuffer, "MINUS", "-");
            // Finally, trim up extra spaces.
            tempBuffer = Regex.Replace(tempBuffer, @"\s+", " ").Trim();

            return tempBuffer.Split(" ".ToCharArray());
        }
    }
}