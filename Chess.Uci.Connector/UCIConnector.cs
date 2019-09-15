using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Chess.Uci.Connector
{
    public class UCIConnector
    {
        private const string MOVE_COMMAND_PATTERN = "position startpos moves {0}\ngo depth {1}\n";
        private const string MOVE_COMMAND_PATTERN_BY_FEN = "position fen {0}\ngo depth {1}\n";
        private const string QUIT_COMMAND = "quit\n";
        private const string NEW_GAME_COMMAND = "ucinewgame\n";
        private const string SET_THREADS_COMMAND = "setoption name Threads value {0}\n";
        private const string MOVE_REGEX_PATTERN = "([a-h][1-8]){2}[rnbq]?";
        private const string RESPONSE_PATTERN_DEPTH_ONE = "bestmove " + MOVE_REGEX_PATTERN;
        private const string RESPONSE_PATTERN = "bestmove " + MOVE_REGEX_PATTERN + " ponder " + MOVE_REGEX_PATTERN;
        
        private Settings _settings;
        private Process _process;
        private bool _isConnected;
        private readonly object _lock = new object();

        public UCIConnector()
        {
            _settings = Settings.Default;
        }

        public UCIConnector(Settings settings)
        {
            _settings = settings;
        }

        ~UCIConnector()
        {
            Disconnect();
        }

        public bool IsConnected => _isConnected;

        public void Connect()
        {
            try
            {
                if (_isConnected)
                    return;

                _process = new Process();
                _process.StartInfo = new ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    FileName = _settings.EnginePath
                };
                
                if (!_process.Start())
                    throw new ApplicationException("Failed to start process");
                _isConnected = true;

                ConfigureEngine();
            }
            catch (System.Exception ex)
            {
                throw new UCIConnectionException("Could not connect to UCI engine, verify inner exception for more details", ex);
            }
        }

        public void StartGame()
        {
            WriteCommand(NEW_GAME_COMMAND);
        }

        public string GetNextMove(string move, int depth)
        {
            var cmd = string.Format(MOVE_COMMAND_PATTERN, move, depth);
            return GetUciMove(cmd, depth);
        }

        public string GetNextMoveByFen(string fen, int depth)
        {
            var cmd = string.Format(MOVE_COMMAND_PATTERN_BY_FEN, fen, depth);
            return GetUciMove(cmd, depth);
        }

        private string GetUciMove(string cmd, int depth)
        {
            WriteCommand(cmd);
            return ReadResponse(depth == 1 ? RESPONSE_PATTERN_DEPTH_ONE : RESPONSE_PATTERN, MOVE_REGEX_PATTERN);
        }

        public void Disconnect()
        {
            if (_isConnected)
            {
                WriteCommand(QUIT_COMMAND);
                _isConnected = false;
            }
        }

        public void ConfigureEngine()
        {
            var cmd = string.Format(SET_THREADS_COMMAND, _settings.Threads);
            WriteCommand(cmd);
        }

        private void WriteCommand(string cmd)
        {
            VerifyConnection();

            lock (_lock)
                _process.StandardInput.Write(cmd);
        }

        private string ReadResponse(string linePattern, string stringPattern = "")
        {
            VerifyConnection();

            lock (_lock)
            {
                var lineRegex = new Regex(linePattern);
                var resultRegex = new Regex(stringPattern ?? string.Empty);
                var output = _process.StandardOutput;

                while (true)
                {
                    var line = output.ReadLine();

                    if (!lineRegex.IsMatch(line))
                        continue;

                    if (string.IsNullOrEmpty(stringPattern))
                        return line;
                    
                    var match = resultRegex.Match(line);

                    return match.Value;
                }
            }
        }

        private void VerifyConnection() 
        {
            if (!_isConnected)
                throw new UCIConnectionException("UCI is not yet connected");

            try
            {
                Process.GetProcessById(_process.Id);
            }
            catch (ArgumentException)
            {
                throw new UCIConnectionException("UCI connection was closed");
            }
        }
    }
}
