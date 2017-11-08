using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyInterpreter;
using NUnit.Framework;

namespace MyInterpreterTester
{
    [TestFixture]
    public class DoubleTests
    {
        [Test]
        public void DigitTest()
        {
            string inputText = "1";
            string expectedNumber = inputText;
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(CheckNumber(lexer.Peek(), expectedNumber));
        }

        [Test]
        public void IntNumberTest()
        {
            string inputText = "1032512132";
            string expectedNumber = inputText;
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(CheckNumber(lexer.Peek(), expectedNumber));
        }

        [Test]
        public void DigitsPointDigitsTest()
        {
            string inputText = "1234523.3412";
            string expectedNumber = inputText;
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(CheckNumber(lexer.Peek(), expectedNumber));
        }

        [Test]
        public void PointDigitsTest()
        {
            string inputText = ".25423";
            string expectedNumber = inputText;
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(CheckNumber(lexer.Peek(), expectedNumber));
        }

        [Test]
        public void DoubleWithAnsignedExponentTest()
        {
            string inputText = "1242.3214123e10";
            string expectedNumber = inputText;
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(CheckNumber(lexer.Peek(), expectedNumber));
        }

        [Test]
        public void DoubleWithPlusSignedExponentTest()
        {
            string inputText = "921.13102134E+4";
            string expectedNumber = inputText;
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(CheckNumber(lexer.Peek(), expectedNumber));
        }

        [Test]
        public void DoubleWithMinusSignedExponentTest()
        {
            string inputText = "12.43E-155";
            string expectedNumber = inputText;
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(CheckNumber(lexer.Peek(), expectedNumber));
        }

        [Test]
        public void UnfinishedDoubleOnPointTest()
        {
            try
            {
                string inputText = ".";
                Lexer lexer = new Lexer(inputText);
                Assert.IsTrue(lexer.Peek().Type != TokenType.Number);
            }
            catch
            {
                Assert.Pass();
            }
        }

        [Test]
        public void UnfinishedDoubleOnExponentTest()
        {
            string inputText = ".21553E";
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(lexer.Peek().Type != TokenType.Number);
        }

        [Test]
        public void UnfinishedDoubleOnExponentSignTest()
        {
            string inputText = ".21553E-";
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(lexer.Peek().Type != TokenType.Number);
        }

        [Test]
        public void OnlyExponentTest()
        {
            string inputText = "e";
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(lexer.Peek().Type != TokenType.Number);
        }

        [Test]
        public void ExponentDigitsTest()
        {
            string inputText = "e314";
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(lexer.Peek().Type != TokenType.Number);
        }

        [Test]
        public void DigitsExponentDigitsTest()
        {
            string inputText = "12E23";
            string expectedNumber = inputText;
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(CheckNumber(lexer.Peek(), expectedNumber));
        }

        [Test]
        public void DigitsMinusSignedExponentsTest()
        {
            string inputText = "12E-23";
            string expectedNumber = inputText;
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(CheckNumber(lexer.Peek(), expectedNumber));
        }

        [Test]
        public void DigitsPlusSignedExponentsTest()
        {
            string inputText = "9E+23";
            string expectedNumber = inputText;
            Lexer lexer = new Lexer(inputText);
            Assert.IsTrue(CheckNumber(lexer.Peek(), expectedNumber));
        }

        private bool CheckNumber(Token token, string expectedNumber)
        {
            return token.Type == TokenType.Number &&
                string.Equals(token.Name, expectedNumber, StringComparison.InvariantCulture);
        }
    }
}
