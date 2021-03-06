﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TQL.Core.Exceptions;
using TQL.Core.Tokens;
using TQL.CronExpression.Parser.Helpers;
using TQL.CronExpression.Parser.Nodes;

namespace TQL.CronExpression.Parser.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void CheckSyntaxTree_AllStarNodes_ShouldPass()
        {
            var tree = CheckSyntaxTree("* * * * * * *");
            CheckHasAppropiateCountsOfSegments(tree);
            CheckLastSegmentIsOfType<EndOfFileNode>(tree);
        }

        [TestMethod]
        public void CheckSyntaxTree_CheckFullSpanCommaExpression_ShouldPass()
        {
            CheckFullSpan("2,45,5 2,45,5 2,45,5 2,45,5 2,45,5 2,45,5 2,45,5",
                new TextSpan(0, 6),
                new TextSpan(7, 6),
                new TextSpan(14, 6),
                new TextSpan(21, 6),
                new TextSpan(28, 6),
                new TextSpan(35, 6),
                new TextSpan(42, 6));
        }

        [TestMethod]
        public void CheckSyntaxTree_CheckFullSpanHashExpression_ShouldPass()
        {
            CheckFullSpan("2#4 2#4 2#4 2#4 2#4 2#4 2#4",
                new TextSpan(0, 3),
                new TextSpan(4, 3),
                new TextSpan(8, 3),
                new TextSpan(12, 3),
                new TextSpan(16, 3),
                new TextSpan(20, 3),
                new TextSpan(24, 3));
        }

        [TestMethod]
        public void CheckSyntaxTree_CheckFullSpanMixedExpression_ShouldPass()
        {
            CheckFullSpan("1,3,2#5 * 1-23 * 2#5 * ?",
                new TextSpan(0, 7),
                new TextSpan(8, 1),
                new TextSpan(10, 4),
                new TextSpan(15, 1),
                new TextSpan(17, 3),
                new TextSpan(21, 1),
                new TextSpan(23, 1));
        }

        [TestMethod]
        public void CheckSyntaxTree_CheckFullSpanRangeExpression_ShouldPass()
        {
            CheckFullSpan("1-5 1-5 1-5 1-5 1-5 1-5 2000-2005",
                new TextSpan(0, 3),
                new TextSpan(4, 3),
                new TextSpan(8, 3),
                new TextSpan(12, 3),
                new TextSpan(16, 3),
                new TextSpan(20, 3),
                new TextSpan(24, 9));
        }

        [TestMethod]
        public void CheckSyntaxTree_CheckFullSpanStarExpression_ShouldPass()
        {
            CheckFullSpan("* * * * * * *",
                new TextSpan(0, 1),
                new TextSpan(2, 1),
                new TextSpan(4, 1),
                new TextSpan(6, 1),
                new TextSpan(8, 1),
                new TextSpan(10, 1),
                new TextSpan(12, 1));
        }

        [TestMethod]
        public void CheckSyntaxTree_DuplicatedComma_After_ShouldThrow()
        {
            CheckSyntaxTree("1,, * * * * * *", "1,_,_ * * * * * *");
        }

        [TestMethod]
        public void CheckSyntaxTree_DuplicatedComma_Before_ShouldThrow()
        {
            CheckSyntaxTree(",,1 * * * * * *", "_,_,1 * * * * * *");
        }

        [TestMethod]
        public void CheckSyntaxTree_DuplicatedComma_Middle_ShouldThrow()
        {
            CheckSyntaxTree(",1, * * * * * *", "_,1,_ * * * * * *");
        }

        [TestMethod]
        public void CheckSyntaxTree_ExpressionIsTooShort_ShouldPass()
        {
            var tree = CheckSyntaxTree("* * *", false);
        }

        [TestMethod]
        public void CheckSyntaxTree_ExpressionRequireTrim_ShouldPass()
        {
            var tree = CheckSyntaxTree(" * * * * * * * ", "* * * * * * *");
            CheckHasAppropiateCountsOfSegments(tree);
            CheckLastSegmentIsOfType<EndOfFileNode>(tree);
        }

        [TestMethod]
        public void CheckSyntaxTree_IncFollowedByRange_ShouldPass()
        {
            CheckSyntaxTree("* * 1-5/10 * * * *");
        }

        [TestMethod]
        public void CheckSyntaxTree_IrregularWhitespaceBetweenSegments_ShouldPass()
        {
            CheckSyntaxTree("*  *   *   * *        *             *", "* * * * * * *");
        }

        [TestMethod]
        public void CheckSyntaxTree_MissingHashNode_ShouldProduceTreeWithMissingNode()
        {
            var tree = "#".Parse(false);
            Assert.AreEqual("_#_", tree.ToString());
            tree = "1#".Parse(false);
            Assert.AreEqual("1#_", tree.ToString());
            tree = "#1".Parse(false);
            Assert.AreEqual("_#1", tree.ToString());
        }

        [TestMethod]
        public void CheckSyntaxTree_MissingNodeAfterComma_ShouldProduceTreeWithMissingNode()
        {
            var tree = "1,".Parse(false);
            Assert.AreEqual("1,_", tree.ToString());
        }

        [TestMethod]
        public void CheckSyntaxTree_MissingNodeRange_ShouldProduceTreeWithMissingNode()
        {
            var tree = "1-".Parse(false);
            Assert.AreEqual("1-_", tree.ToString());
            tree = "-1".Parse(false);
            Assert.AreEqual("_-1", tree.ToString());
            tree = " -1".Parse(false);
            Assert.AreEqual("_-1", tree.ToString());
            tree = "* * -1 1- * * *".Parse();
            Assert.AreEqual("* * _-1 1-_ * * *", tree.ToString());
            tree = "-1,1-,5".Parse(false);
            Assert.AreEqual("_-1,1-_,5", tree.ToString());
            tree = "-".Parse(false);
            Assert.AreEqual("_-_", tree.ToString());
        }

        [TestMethod]
        public void CheckSyntaxTree_MixedComplexExpression_ShouldPass()
        {
            var lexer = new Lexer("0/5 14,18,3-39,52 * ? JAN,MAR,SEP MON-FRI 2002-2010");
            var parser = new CronParser(lexer);
            parser.ComposeRootComponents();
        }

        [TestMethod]
        public void CheckSyntaxTree_MixedComplexExpression_WithNestedRangeInIncNode_ShouldPass()
        {
            var lexer =
                new Lexer("0,3#2,1-5,2-6,6,1,0 0/5 14,18-20,25 * FEB-MAY,1-8/2,JANUARY,FEBRUARY MON-FRI,1W,1545L,6#3 ?");
            var parser = new CronParser(lexer);
            parser.ComposeRootComponents();
        }

        [TestMethod]
        public void CheckSyntaxTree_MixedMissingNodes_ShouldProduceTreeWithMissingNodes()
        {
            var tree = "-#,,".Parse(false);
            Assert.AreEqual("_-_#_,_,_", tree.ToString());
            tree = ",,".Parse(false);
            Assert.AreEqual("_,_,_", tree.ToString());
        }

        [TestMethod]
        public void CheckSyntaxTree_MoreSegmentsThatSeven_ShouldProduceTree()
        {
            var tree = "* * * * * * * * *".Parse(false, false);
            Assert.AreEqual(9, tree.Desecendants.Count());
        }

        [TestMethod]
        public void CheckSyntaxTree_NodesSeparatedByNewLine_ShouldPass()
        {
            var tree = CheckSyntaxTree(
                "*" + Environment.NewLine +
                "*" + Environment.NewLine +
                "*" + Environment.NewLine +
                "*" + Environment.NewLine +
                "*" + Environment.NewLine +
                "*" + Environment.NewLine +
                "*", "* * * * * * *");

            CheckHasAppropiateCountsOfSegments(tree);
            CheckLastSegmentIsOfType<EndOfFileNode>(tree);
        }

        [TestMethod]
        public void CheckSyntaxTree_StarMixedWithValues_ShouldPass()
        {
            var tree = "0 0 0 29 2,August,November *,2 2015-2016".Parse(false, false);
        }

        [TestMethod]
        public void CheckSyntaxTree_UnexpectedCommaBeforeInteger_ShouldThrow()
        {
            CheckSyntaxTree(",1 * * * * * *", "_,1 * * * * * *");
        }

        [TestMethod]
        public void CheckSyntaxTree_WithCommaNodes_ShouldPass()
        {
            var tree = CheckSyntaxTree("1,2,3,4 7,6,5,4 * * * * *");
            CheckHasAppropiateCountsOfSegments(tree);
            CheckLastSegmentIsOfType<EndOfFileNode>(tree);
        }

        [TestMethod]
        public void CheckSyntaxTree_WithHashNode_ShouldPass()
        {
            CheckSyntaxTree("* 3#5 * * * * *");
        }

        [TestMethod]
        public void CheckSyntaxTree_WithIncNode_ShouldPass()
        {
            var tree = CheckSyntaxTree("1/5 * * 0-7 * * *");
            CheckHasAppropiateCountsOfSegments(tree);
            CheckLastSegmentIsOfType<EndOfFileNode>(tree);
        }

        [TestMethod]
        public void CheckSyntaxTree_WithLOption_ShouldPass()
        {
            var tree = CheckSyntaxTree("* L * * L * *");
            CheckHasAppropiateCountsOfSegments(tree);
            CheckLastSegmentIsOfType<EndOfFileNode>(tree);
        }

        [TestMethod]
        public void CheckSyntaxTree_WithLWOption_ShouldPass()
        {
            var tree = CheckSyntaxTree("LW LW LW * * * *");
            CheckHasAppropiateCountsOfSegments(tree);
            CheckLastSegmentIsOfType<EndOfFileNode>(tree);
        }

        [TestMethod]
        public void CheckSyntaxTree_WithMonthMapping_ShouldPass()
        {
            var lexer = new Lexer("* * * * * MON#5,6#3 ?");
            var parser = new CronParser(lexer);
            parser.ComposeRootComponents();
        }

        [TestMethod]
        public void CheckSyntaxTree_WithMonthsRange_ShouldPass()
        {
            var tree = CheckSyntaxTree("MON-WED * SAT-MON * * * *");
            CheckHasAppropiateCountsOfSegments(tree);
            CheckLastSegmentIsOfType<EndOfFileNode>(tree);
        }

        [TestMethod]
        public void CheckSyntaxTree_WithNumericPrecededLNode_ShouldPass()
        {
            CheckSyntaxTree("* 50L * * * * *");
        }

        [TestMethod]
        public void CheckSyntaxTree_WithNumericPrecededWNode_ShouldPass()
        {
            CheckSyntaxTree("* 50W * * * * *");
        }

        [TestMethod]
        public void CheckSyntaxTree_WithQuestionMark_ShouldPass()
        {
            var tree = CheckSyntaxTree("* * * ? * * *");
            CheckHasAppropiateCountsOfSegments(tree);
            CheckLastSegmentIsOfType<EndOfFileNode>(tree);
        }


        [TestMethod]
        public void CheckSyntaxTree_WithRangeNode_ShouldPass()
        {
            var tree = CheckSyntaxTree("1-5 * * * * * 2000-3000");
            CheckHasAppropiateCountsOfSegments(tree);
            CheckLastSegmentIsOfType<EndOfFileNode>(tree);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownTokenException))]
        public void CheckSyntaxTree_WithUnknownOperator_ShouldThrow()
        {
            var tree = CheckSyntaxTree("& * * * * * *");
        }

        [TestMethod]
        public void CheckSyntaxTree_WithWeekDay_ShouldPass()
        {
            var tree = CheckSyntaxTree("MON TUE WED * * * *");
            CheckHasAppropiateCountsOfSegments(tree);
            CheckLastSegmentIsOfType<EndOfFileNode>(tree);
        }

        [TestMethod]
        public void CheckSyntaxTree_WithWOption_ShouldPass()
        {
            var tree = CheckSyntaxTree("W * * W * * *");
            CheckHasAppropiateCountsOfSegments(tree);
            CheckLastSegmentIsOfType<EndOfFileNode>(tree);
        }

        [TestMethod]
        public void CheckSyntaxTree_WithYearUnspecified_ShouldPassWithAppendedYearStarNode()
        {
            var tree = CheckSyntaxTree("* * * * * *", "* * * * * * *");
            CheckHasAppropiateCountsOfSegments(tree);
            CheckLastSegmentIsOfType<EndOfFileNode>(tree);
        }

        [TestMethod]
        public void CheckSyntaxTree_WithIncorrectValues_ShouldProcueTree()
        {
            var tree = CheckSyntaxTree("*a * * * * * *", "*a * * * * * *", false);
            CheckHasAppropiateCountsOfSegments(tree);
        }

        private static void CheckFullSpan(string expression, params TextSpan[] spans)
        {
            var exp = expression.Parse();
            for (var i = 0; i < 7; ++i)
            {
                var child = exp.Desecendants[i];
                var span = child.FullSpan;
                Assert.AreEqual(spans[i], span);
            }
        }

        private static void CheckHasAppropiateCountsOfSegments(RootComponentNode tree)
        {
            //Seven required segments + EndOfFile
            Assert.AreEqual(8, tree.Desecendants.Count());
        }

        private static void CheckLastSegmentIsOfType<T>(RootComponentNode tree)
        {
            Assert.AreEqual(typeof(T), tree.Desecendants.Last().GetType());
        }

        private static RootComponentNode CheckSyntaxTree(string expression, string expectedOutputExpression,
            bool produceMissingYearSegment = true)
        {
            var ast = expression.Parse(produceMissingYearSegment);
            Assert.AreEqual(expectedOutputExpression, ast.ToString());
            return ast;
        }

        private static RootComponentNode CheckSyntaxTree(string expression, bool produceMissingYearSegment = true)
            => CheckSyntaxTree(expression, expression, produceMissingYearSegment);
    }
}