// ****************************************************************************
// Tests.cs contains unit tests divided into classes by function
// ****************************************************************************

namespace IvanCalculator.Tests
{
    using System.Collections;
    using NUnit.Framework;
    
    [TestFixture]
    public class ModelCalculatorTests
    {
        private Calculator testCalc;

        [SetUp]
        public void Setup()
        {
            testCalc = new Calculator();
        }

        [Test]
        public void TestAppendToInputString_Valid()
        {
            string validInput = "1";
            testCalc.AppendToInputString(validInput);
            Assert.AreEqual(validInput, testCalc.inputStringBuilder.ToString());
        }

        [Test]
        public void TestAppendToInputString_NotValid()
        {
            string badInput = "a";
            testCalc.AppendToInputString(badInput);
            Assert.AreNotEqual(badInput, testCalc.inputStringBuilder.ToString());
        }

        [Test]
        public void TestAppendToInputString_EventDoesFire()
        {
            string badInput = "a";
            bool wasCalled = false;
            testCalc.InputUpdatedEvent += delegate (Calculator.ModelValueUpdateEventHandlerArgs e)
                                                   { wasCalled = true; };
            testCalc.AppendToInputString(badInput);
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void TestAppendToInputString_EventPayloadIsCorrect()
        {
            string badInput = "a";
            string eventPayload = null;
            testCalc.InputUpdatedEvent += delegate (Calculator.ModelValueUpdateEventHandlerArgs e)
            { eventPayload = e.newValue; };
            testCalc.AppendToInputString(badInput);
            Assert.AreEqual("", eventPayload);
        }

        [Test]
        public void TestRemoveLastCharFromInputString()
        {
            testCalc.inputStringBuilder.Append("123");
            testCalc.RemoveLastCharFromInputString();
            Assert.AreEqual("12", testCalc.inputStringBuilder.ToString());
        }

        [Test]
        public void TestRemoveLastChar_EventDoesFire()
        {
            testCalc.inputStringBuilder.Append("123");
            bool wasCalled = false;
            testCalc.InputUpdatedEvent += delegate (Calculator.ModelValueUpdateEventHandlerArgs e)
            { wasCalled = true; };
            testCalc.RemoveLastCharFromInputString();
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void TestRemoveLastChar_EventPayloadIsCorrect()
        {
            testCalc.inputStringBuilder.Append("123");
            string eventPayload = null;
            testCalc.InputUpdatedEvent += delegate (Calculator.ModelValueUpdateEventHandlerArgs e)
            { eventPayload = e.newValue; };
            testCalc.RemoveLastCharFromInputString();
            Assert.AreEqual(1, eventPayload.Length);
        }

        [Test]
        public void TestRemoveLastChar_EventDoesNotFireOnEmptyString()
        {
            bool wasCalled = false;
            testCalc.InputUpdatedEvent += delegate (Calculator.ModelValueUpdateEventHandlerArgs e)
            { wasCalled = true; };
            testCalc.RemoveLastCharFromInputString();
            Assert.IsFalse(wasCalled);
        }

        [Test]
        public void TestClearLastNumber()
        {
            testCalc.inputStringBuilder.Append("1+2");
            testCalc.ClearLastNumber();
            Assert.AreEqual("1", testCalc.inputStringBuilder.ToString());
        }

        [Test]
        public void TestClearLastNumber_EventFired()
        {
            bool wasCalled = false;
            testCalc.inputStringBuilder.Append("1+2");
            testCalc.InputUpdatedEvent += delegate (Calculator.ModelValueUpdateEventHandlerArgs e)
            { wasCalled = true; };
            testCalc.ClearLastNumber();
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void TestClearLastNumber_EventPayloadIsCorrect()
        {
            testCalc.inputStringBuilder.Append("1+2");
            string eventPayload = null;
            testCalc.InputUpdatedEvent += delegate (Calculator.ModelValueUpdateEventHandlerArgs e)
            { eventPayload = e.newValue; };
            testCalc.ClearLastNumber();
            Assert.AreEqual(2, eventPayload.Length);
        }

        [Test]
        public void TestClearAll()
        {
            testCalc.inputStringBuilder.Append("1+2+3");
            testCalc.ClearAll();
            Assert.AreEqual(0, testCalc.inputStringBuilder.Length);
        }

        [Test]
        public void TestClearAll_EventFired()
        {
            bool wasCalled = false;
            testCalc.inputStringBuilder.Append("1+2+3");
            testCalc.InputUpdatedEvent += delegate (Calculator.ModelValueUpdateEventHandlerArgs e)
            { wasCalled = true; };
            testCalc.ClearAll();
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void TestClearAll_EventPayloadIsCorrect()
        {
            testCalc.inputStringBuilder.Append("1+2+3");
            string eventPayload = null;
            testCalc.InputUpdatedEvent += delegate (Calculator.ModelValueUpdateEventHandlerArgs e)
            { eventPayload = e.newValue; };
            testCalc.ClearAll();
            Assert.AreEqual(5, eventPayload.Length);
        }

        [Test]
        public void TestEvaluate_ResultValid()
        {
            double expectedResult = 3;
            testCalc.inputStringBuilder.Append("1+2");
            testCalc.Evaluate();
            Assert.AreEqual(expectedResult, testCalc.currentResult);
        }

        [Test]
        public void TestEvaluate_StringBuilderIsEmptyAfter()
        {
            testCalc.inputStringBuilder.Append("1+2");
            testCalc.Evaluate();
            Assert.AreEqual(0, testCalc.inputStringBuilder.Length);
        }

        [Test]
        public void TestEvaluate_BadAddition()
        {
            testCalc.inputStringBuilder.Append("1++2");
            string eventPayload = null;
            testCalc.ErrorUpdatedEvent += delegate (Calculator.ModelValueUpdateEventHandlerArgs e)
            { eventPayload = e.newValue; };
            testCalc.Evaluate();
            Assert.AreEqual("Addition evaluation error!", eventPayload);
        }

        // Skipping the other error assertions

        [Test]
        public void TestUpdateCurrentResult()
        {
            double newTestResult = 100.0;
            testCalc.UpdateCalculatedResult(newTestResult);
            Assert.AreEqual(newTestResult, testCalc.currentResult);
        }

        [Test]
        public void TestUpdateCurrentResultEventFired()
        {
            bool wasCalled = false;
            double newTestResult = 100.0;
            testCalc.ResultUpdatedEvent += delegate (Calculator.ModelValueUpdateEventHandlerArgs e)
            { wasCalled = true; };
            testCalc.UpdateCalculatedResult(newTestResult);
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void TestUpdateCurrentResultEventPayloadIsCorrect()
        {
            double newTestResult = 100.0;
            string eventPayload = null;
            testCalc.ResultUpdatedEvent += delegate (Calculator.ModelValueUpdateEventHandlerArgs e)
            { eventPayload = e.newValue; };
            testCalc.UpdateCalculatedResult(newTestResult);
            Assert.AreEqual(newTestResult.ToString(), eventPayload);
        }
    }

    [TestFixture]
    public class ModelInputParserTests
    {
        string rawInputString;
        double testDouble;
        Queue outQueue;

        [SetUp]
        public void SetUp()
        {
            rawInputString = "";
            testDouble = 1.0;
        }

        [Test]
        public void TestConvertToRPN_ReturnsTrueValid()
        {
            rawInputString = "1+2";
            bool returnVal = InputParser.ConvertToRPN(rawInputString, testDouble, out outQueue);
            Assert.IsTrue(returnVal);
        }

        [Test]
        public void TestConvertToRPN_ReturnsFalseValid()
        {
            rawInputString = "())";
            bool returnVal = InputParser.ConvertToRPN(rawInputString, testDouble, out outQueue);
            Assert.IsFalse(returnVal);
        }

        [Test]
        public void TestConvertToRPN_OutsValidToken()
        {
            rawInputString = "1";
            RPNToken expectedToken = new RPNToken(TokenType.Number, 1);
            InputParser.ConvertToRPN(rawInputString, testDouble, out outQueue);
            Assert.AreEqual(expectedToken, (RPNToken)outQueue.Peek());
        }

        [Test]
        public void TestConvertToRPN_OutQueueCorrectSize()
        {
            rawInputString = "(1+2-3)!1/x";
            InputParser.ConvertToRPN(rawInputString, testDouble, out outQueue);
            Assert.AreEqual(7, outQueue.Count);
        }

        [Test]
        public void TestParse_ReturnsValidArray()
        {
            rawInputString = "12+3";
            string[] testStringArray = InputParser.Parse(rawInputString);
            Assert.AreEqual(3, testStringArray.Length);
        }

        [Test]
        public void TestParse_UnaryConversion()
        {
            rawInputString = "1--3";
            string[] expectedArray = new string[] { "1", "-", "3", "~" };
            string[] testArray = InputParser.Parse(rawInputString);
            Assert.AreEqual(expectedArray, testArray);
        }

        [Test]
        public void TestParse_ReciprocalConversion()
        {
            rawInputString = "11/x";
            string[] expectedArray = new string[] { "1", "$"};
            string[] testArray = InputParser.Parse(rawInputString);
            Assert.AreEqual(expectedArray, testArray);
        }
    }
}


