using NUnit.Framework;
using Chess.Uci.Connector;

namespace Chess.Uci.Connector.Test
{
    public class ConnectorTests
    {
        private const string MOVE_REGEX_PATTERN = "([a-h][1-8]){2}[rnbq]?";

        private Settings _setting;

        [SetUp]
        public void Setup()
        {
            _setting = new Settings
            {
                Threads = 2,
                EnginePath = "stockfish"
            };
        }

        [Test]
        public void ShouldConnectToUciEngine()
        {
            var connector = new UCIConnector(_setting);
            connector.Connect();

            Assert.IsTrue(connector.IsConnected);
        }

        [Test]
        public void ShouldConnectAndDisconnectToUciEngine()
        {
            var connector = new UCIConnector(_setting);
            connector.Connect();
            connector.Disconnect();

            Assert.IsFalse(connector.IsConnected);
        }

        [Test]
        public void ShouldGetNextMoveAsWhite()
        {
            var connector = new UCIConnector(_setting);
            connector.Connect();

            var move = connector.GetNextMove("e2e4", 5);

            connector.Disconnect();

            StringAssert.IsMatch(MOVE_REGEX_PATTERN, move);
        }

        [Test]
        public void ShouldGetNextMoveAsWhiteWithDepthOne()
        {
            var connector = new UCIConnector(_setting);
            connector.Connect();

            var move = connector.GetNextMove("e2e4", 1);

            connector.Disconnect();

            StringAssert.IsMatch(MOVE_REGEX_PATTERN, move);
        }

        [Test]
        public void ShouldGetNextMoveAsBlack()
        {
            var connector = new UCIConnector(_setting);
            connector.Connect();

            var move1 = connector.GetNextMove("", 5);
            var move2 = connector.GetNextMove("e2e4 d7d5", 5);

            connector.Disconnect();

            StringAssert.IsMatch(MOVE_REGEX_PATTERN, move1);
            StringAssert.IsMatch(MOVE_REGEX_PATTERN, move2);
        }

        [Test]
        public void ShouldGetNextMoveAsBlackWithDepthOne()
        {
            var connector = new UCIConnector(_setting);
            connector.Connect();

            var move1 = connector.GetNextMove("", 1);
            var move2 = connector.GetNextMove("e2e4 d7d5", 1);

            connector.Disconnect();

            StringAssert.IsMatch(MOVE_REGEX_PATTERN, move1);
            StringAssert.IsMatch(MOVE_REGEX_PATTERN, move2);
        }

        [Test]
        public void ShouldThrowNotConnectedException()
        {
            var connector = new UCIConnector(_setting);

            Assert.IsFalse(connector.IsConnected);
            Assert.Throws<UCIConnectionException>(()=> connector.GetNextMove("d2d4", 5));
            Assert.Throws<UCIConnectionException>(()=> connector.ConfigureEngine());
            Assert.Throws<UCIConnectionException>(()=> connector.StartGame());
        }
    }
}