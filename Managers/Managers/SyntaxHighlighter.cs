using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using MyInterpreter;

namespace Managers
{
    //найти старый текст
    //убрать старый текст
    //найти новый текст
    //добавить новый текст
    public sealed class SyntaxHighlighter
    {
        private const int enableSuspender = 6;

        private ITokenPainter painter;
        private LinkedList<Token> tokenList;

        private LinkedListNode<Token> lastChangedNode;

        public SyntaxHighlighter(ITokenPainter painter)
            : this(string.Empty, painter)
        {
        }

        public SyntaxHighlighter(string text, ITokenPainter painter)
        {
            if (text == null || painter == null)
            {
                throw new ArgumentNullException();
            }

            this.painter = painter;
            InitializeNewText(text);
        }

        public void ClearText()
        {
            tokenList = new LinkedList<Token>();
            lastChangedNode = null;
        }

        public void OnTextChanged(TextChangedEventArgs currentEvent)
        {
            switch (currentEvent.Type)
            {
                case TextChangedEventArgs.EventType.None:
                    break;
                case TextChangedEventArgs.EventType.TextWritten:
                case TextChangedEventArgs.EventType.TextReplaced:
                    TextReplasedHandler(currentEvent);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void Clear(string newText)
        {
            if (newText == null)
            {
                throw new ArgumentNullException();
            }

            lastChangedNode = null;
            InitializeNewText(newText);
        }

        //Handler for TextWritten, TextReplased
        private void TextReplasedHandler(TextChangedEventArgs currentEvent)
        {
            if (tokenList.Count <= 0)
            {
                if (currentEvent.OldStartPosition != 0 || currentEvent.OldText != string.Empty)
                {
                    throw new NotSupportedException();
                }
                InitializeNewText(currentEvent.NewText);
                return;
            }

            LinkedListNode<Token> startTokenNode =
                FindTokenNode(currentEvent.OldStartPosition, lastChangedNode);
            if (startTokenNode.Previous != null)
            {
                startTokenNode = startTokenNode.Previous;
            }
            LinkedListNode<Token> endTokenNode =
                FindTokenNode(currentEvent.OldStartPosition + currentEvent.OldText.Length, lastChangedNode);
            if (endTokenNode != null)
            {
                endTokenNode = endTokenNode.Next;
            }

            string oldTokenText = RestoreTokensText(startTokenNode, endTokenNode);
            string changedTokenText;
            changedTokenText = oldTokenText.Remove(currentEvent.OldStartPosition - startTokenNode.Value.Position.BeginIndex,
                currentEvent.OldText.Length);
            changedTokenText = changedTokenText.Insert(currentEvent.OldStartPosition - startTokenNode.Value.Position.BeginIndex,
                currentEvent.NewText);

            if (endTokenNode != null)
            {
                ChangeTokensPositionAfter(endTokenNode, changedTokenText.Length - oldTokenText.Length);
            }

            LinkedListNode<Token> curLeftTokenNode = startTokenNode, curRightTokenNode = endTokenNode;
            List<Token> reparsingTokenList = Lexer.SplitIntoTokens(changedTokenText, true);

            ReplaceTokensInList(ref curLeftTokenNode, ref curRightTokenNode, reparsingTokenList);

            if (curLeftTokenNode == curRightTokenNode)
            {
                return;
            }

            FindModifiedArea(ref curLeftTokenNode, ref curRightTokenNode, out reparsingTokenList);

            ReplaceTokensInList(ref curLeftTokenNode, ref curRightTokenNode, reparsingTokenList);

            lastChangedNode = curRightTokenNode;

            ColorTokens(curLeftTokenNode, curRightTokenNode);
        }

        /// <summary> Возвращает узел с токеном занимающий указанную позицию. </summary>
        /// <param name="position"> Позиция. </param>
        /// <param name="searchTokenNode"> Узел с которого нужно начинать поиск, если не задан то поиск начинается с начала. </param>
        private LinkedListNode<Token> FindTokenNode(int position, LinkedListNode<Token> searchTokenNode = null)
        {
            if (searchTokenNode == null && (searchTokenNode = tokenList.Last) == null)
            {
                return null;
            }

            while (position < searchTokenNode.Value.Position.BeginIndex)
            {
                searchTokenNode = searchTokenNode.Previous;
            }

            while (position > searchTokenNode.Value.Position.BeginIndex + searchTokenNode.Value.Position.Length)
            {
                searchTokenNode = searchTokenNode.Next;
            }

            return searchTokenNode;
        }

        private string RestoreTokensText(LinkedListNode<Token> startTokenNode, LinkedListNode<Token> endTokenNode)
        {
            if (startTokenNode == null)
            {
                return string.Empty;
            }

            StringBuilder restoredString = new StringBuilder();

            while (startTokenNode != endTokenNode)
            {
                restoredString.Append(Lexer.TokenToOriginalString(startTokenNode.Value));
                startTokenNode = startTokenNode.Next;
            }

            return restoredString.ToString();
        }

        private void FindModifiedArea(ref LinkedListNode<Token> curLeftTokenNode,
            ref LinkedListNode<Token> curRightTokenNode, out List<Token> reparsingTokenList)
        {
            bool isLeftMoved = true;
            bool isRightMoved = true;
            int shift = 0;
            int radiusChange = 1;

            while (true)
            {
                if (isLeftMoved)
                {
                    for (int i = 0; curLeftTokenNode.Previous != null && i < radiusChange; i++)
                    {
                        curLeftTokenNode = curLeftTokenNode.Previous;
                    }
                }

                if (isRightMoved)
                {
                    for (int i = 0; curRightTokenNode != null && i < radiusChange; i++)
                    {
                        curRightTokenNode = curRightTokenNode.Next;
                    }
                }

                string textForReparsingToken = RestoreTokensText(curLeftTokenNode, curRightTokenNode);
                reparsingTokenList = Lexer.SplitIntoTokens(textForReparsingToken, true);

                if (reparsingTokenList.Count <= 0)
                {
                    throw new NotSupportedException();
                }

                if (curLeftTokenNode.Previous == null || reparsingTokenList[0].Equals(curLeftTokenNode.Value))
                {
                    isLeftMoved = false;
                }

                if (curRightTokenNode == null ||
                    (curRightTokenNode.Previous != null) && reparsingTokenList[reparsingTokenList.Count - 1].Equals(curRightTokenNode.Previous.Value))
                {
                    isRightMoved = false;
                }

                if (!(isLeftMoved || isRightMoved))
                {
                    break;
                }

                radiusChange += 1 << shift;
                shift++;

                if (shift > 31)
                {
                    throw new NotSupportedException();
                }
            }
        }

        private void ReplaceTokensInList(ref LinkedListNode<Token> curLeftTokenNode,// !=null
            ref LinkedListNode<Token> curRightTokenNode,//can be ==null
            List<Token> reparsingTokenList)
        {
            while (curLeftTokenNode.Next != curRightTokenNode)
            {
                tokenList.Remove(curLeftTokenNode.Next);
            }
            tokenList.Remove(curLeftTokenNode);
            curLeftTokenNode = curRightTokenNode;

            if (reparsingTokenList.Count <= 0)
            {
                return;
            }

            int index;
            int newTokenPositionCorrection;

            if (curRightTokenNode == null)
            {
                newTokenPositionCorrection = tokenList.Last == null ?
                    HighlighterHelper.WithoutCorrection : tokenList.Last.Value.Position.BeginIndex + tokenList.Last.Value.Position.Length;
                curLeftTokenNode = tokenList.AddLast(
                    HighlighterHelper.CreateTokenWithShiftPosition(reparsingTokenList[0], newTokenPositionCorrection));
                index = 1;
            }
            else if (curRightTokenNode.Previous == null)
            {
                newTokenPositionCorrection = HighlighterHelper.WithoutCorrection;
                curLeftTokenNode = tokenList.AddBefore(curRightTokenNode,
                    HighlighterHelper.CreateTokenWithShiftPosition(reparsingTokenList[0], newTokenPositionCorrection));
                index = 1;
            }
            else
            {
                newTokenPositionCorrection =
                    curRightTokenNode.Previous.Value.Position.BeginIndex + curRightTokenNode.Previous.Value.Position.Length;
                curLeftTokenNode = curRightTokenNode.Previous;
                index = 0;
            }

            for (LinkedListNode<Token> tempTokenNode = curLeftTokenNode; index < reparsingTokenList.Count; index++)
            {
                tempTokenNode = tokenList.AddAfter(tempTokenNode,
                    HighlighterHelper.CreateTokenWithShiftPosition(reparsingTokenList[index], newTokenPositionCorrection));
            }
        }

        private void ChangeTokensPositionAfter(LinkedListNode<Token> rightTokenNode, int positionCorrection)
        {
            while (rightTokenNode != null)
            {
                rightTokenNode.Value = HighlighterHelper.CreateTokenWithShiftPosition(rightTokenNode.Value, positionCorrection);

                rightTokenNode = rightTokenNode.Next;
            }
        }

        private void InitializeNewText(string text)
        {
            tokenList = ToLinkedList(Lexer.SplitIntoTokens(text, true));
            lastChangedNode = null;
            ColorTokens(tokenList.First, null);
        }

        private void ColorTokens(LinkedListNode<Token> firstRecolorToken, LinkedListNode<Token> endRecoloringToken)
        {
            if (firstRecolorToken == null)
            {
                return;
            }

            int counter = 0;

            while (firstRecolorToken != endRecoloringToken)
            {
                ColorToken(firstRecolorToken.Value);
                firstRecolorToken = firstRecolorToken.Next;
                counter++;
                if (counter == enableSuspender)
                {
                    painter.SuspendPaitingDisplay();
                }
            }

            if (counter >= enableSuspender)
            {
                painter.ResumePaintingDisplay();
            }
        }

        private void ColorToken(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Do:
                case TokenType.While:
                case TokenType.For:
                case TokenType.If:
                case TokenType.Else:
                case TokenType.Goto:
                case TokenType.New:
                case TokenType.Return:
                case TokenType.Break:
                    painter.PaintText(ColoredTokenType.KeyWords, token.Position.BeginIndex, token.Position.Length);
                    break;
                case TokenType.WrongStringLiteral:
                case TokenType.RegularStringLiteral:
                case TokenType.VerbatimStringLiteral:
                    painter.PaintText(ColoredTokenType.StringConstant, token.Position.BeginIndex, token.Position.Length);
                    break;
                case TokenType.Number:
                    painter.PaintText(ColoredTokenType.NumberConstant, token.Position.BeginIndex, token.Position.Length);
                    break;
                case TokenType.SingleLineComment:
                    painter.PaintText(ColoredTokenType.Comment, token.Position.BeginIndex, token.Position.Length);
                    break;
                case TokenType.Identifier:
                    {
                        if (Parser.IsValueType(token.Name))
                        {
                            painter.PaintText(ColoredTokenType.KeyWords, token.Position.BeginIndex, token.Position.Length);
                        }
                        else if (Parser.IsFunction(token.Name))
                        {
                            painter.PaintText(ColoredTokenType.Function, token.Position.BeginIndex, token.Position.Length);
                        }
                        else
                        {
                            painter.PaintText(ColoredTokenType.WithoutIllumination, token.Position.BeginIndex, token.Position.Length);
                        }
                        break;
                    }
                default:
                    painter.PaintText(ColoredTokenType.WithoutIllumination, token.Position.BeginIndex, token.Position.Length);
                    break;
            }
        }

        private LinkedList<Token> ToLinkedList(List<Token> tokenList)
        {
            LinkedList<Token> tokenLinkedList = new LinkedList<Token>();

            foreach (Token token in tokenList)
            {
                tokenLinkedList.AddLast(token);
            }

            return tokenLinkedList;
        }
    }
}
