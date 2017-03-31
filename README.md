# Algebraic Calculator
### Version
1.0 - Initial release
### About
Algebraic calculator written using the command design pattern.
Supports the following operations:
- Addition
- Subtraction
- Multiplication
- Division
- Reciprocal of x
- Factorial of x
- Order of operations including parentheses

### How to use
Executable is located in the Binary folder.
- A - All clear
- C - Clear last entered number
- Q - Quit

### Please note!
##### Decimals
- Decimals are supported, however they require at least one left-hand digit for
the calculator to process them correctly (0.1 works, .1 does not).

##### Factorials
- Factorials can only be calculated using non-negative integers. If the user 
attempts a decimal factorial, the calculator will round to nearest int.
- Negative and very large values will crash the calculator.

### Future TODO
- Build a more complete test library
- Implement an in-app help screen activated via the 'H' key
- Clean up View.cs
- Better, more verbose error handling on bad user input
- Refactor Calculator.Evaluate()
- Better decimal handling
- Better factorial handling