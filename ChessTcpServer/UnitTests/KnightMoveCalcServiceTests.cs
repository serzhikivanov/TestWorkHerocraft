using ChessTcpServer.Services;
using NUnit.Framework;

namespace ChessTcpServer.UnitTests
{
    [TestFixture]
    public class KnightMoveCalcServiceTests
    {
        private KnightMoveCalcService _sut;

        [SetUp]
        public void SetUp() 
        {
            _sut = new KnightMoveCalcService();
        }

        [Test]
        public void CalcMoves_StartEqEnd_ReturnsStartCell() 
        {
            // Assign
            var startCell = "e2";
            var endCell = "e2";

            // Act
            string[] res = _sut.CalcKnightPath(startCell, endCell);

            // Assert
            Assert.That(res.Count(), Is.EqualTo(1), $"Result length {res.Count()} was not equal to expected value {1}");
        }

        [TestCase("e2", "e3", "e2,g3,f5,e3")]
        [TestCase("e2", "f4", "e2,f4")]
        [TestCase("e2", "g6", "e2,f4,g6")]
        [TestCase("e2", "h8", "e2,f4,g6,h8")]
        public void CalcMoves_StartNeqEnd_ReturnsStartCell(string startCell, string endCell, string expectedResult)
        {
            // Assign
            var expResArr = expectedResult.Split(',');

            // Act
            string[] res = _sut.CalcKnightPath(startCell, endCell);

            // Assert
            Assert.That(res.Count(), Is.EqualTo(expResArr.Length), $"Result length {res.Count()} was not equal to expected value {1}");
            Assert.That(res.SequenceEqual(expResArr));
        }
    }
}
