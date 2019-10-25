using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NCalc;

namespace WpfCalculator2._0.ViewModels
{
    class CalculatorViewModel : ObservableObject
    {
        private enum Operation
        {
            None,
            Add,
            Subtract,
            Multiply,
            Divide,
            Percent,
            Sin,
            Cos,
            Tan,
            Square,
            SquareRoot,
            Equal,
            LeftParent,
            RightParent,
            Clear
        }

        private Dictionary<string, Operation> BinaryOperations = new Dictionary<string, Operation>()
        {
            { "+", Operation.Add },
            { "-", Operation.Subtract },
            { "*", Operation.Multiply },
            { "/", Operation.Divide },
            { "%", Operation.Percent },
            { "=", Operation.Equal },
            { "(", Operation.LeftParent },
            { ")", Operation.RightParent },
            { "CE", Operation.Clear }
        };

        private Dictionary<string, Operation> UnaryOperations = new Dictionary<string, Operation>()
        {
            { "Sin", Operation.Sin },
            { "Cos", Operation.Cos },
            { "Tan", Operation.Tan },
            { "Sqr", Operation.Square },
            { "SqrRt", Operation.SquareRoot }
        };

        private static string _operationString;
        private static string _operandString;
        private static double _operand1;
        private static double _operand2;
        private static Operation _binaryOperator;

        public ICommand ButtonNumberCommand { get; set; }
        public ICommand ButtonOperationCommand { get; set; }
        public ICommand ButtonEqualCommand { get; set; }
        public ICommand ButtonAddOperationCommand { get; set; }
        private Operation CurrentOperation { get; set; }

        private string _displayContent;

        public string DisplayContent
        {
            get { return _displayContent; }
            set
            {
                _displayContent = value;
                OnPropertyChanged("DisplayContent");
            }
        }

        public CalculatorViewModel()
        {
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            _displayContent = "";
            ButtonNumberCommand = new RelayCommand(new Action<object>(UpdateOperandString));
            //ButtonOperationCommand = new RelayCommand(new Action<object>(SetOperation));
            ButtonEqualCommand = new RelayCommand(new Action<object>(PerformCalculation));
            ButtonAddOperationCommand = new RelayCommand(new Action<object>(UpdateOperationString));
        }
        /// <summary>
        /// If operator is binary it adds the operator to the string
        /// If the operator is unary it gives results
        /// </summary>
        /// <param name="obj"></param>
        private void UpdateOperationString(object obj)
        {
            Operation operation = CurrentOperator(obj.ToString());
            if (double.TryParse(_operandString, out double result))
            {
                if (obj.ToString() == "CE")
                {
                    DisplayContent = "";
                }
                else if (BinaryOperations.ContainsValue(operation))
                {
                    DisplayContent += obj.ToString();
                }
                else if (UnaryOperations.ContainsValue(operation))
                {
                    _operand1 = result;
                    DisplayContent = ProcessUnaryOperation(operation).ToString();
                }
                else
                {
                    DisplayContent = "Unknown Operation Encountered";
                }
            }
            else
            {
                DisplayContent = "Please enter a valid number.";
            }
        }
                
        /// <summary>
        /// IF equal sign is pressed the expression will be calculated
        /// WONT WORK WITH UNARY OPERATIONS
        /// </summary>
        /// <param name="obj"></param>
        private void PerformCalculation(object obj)
        {
            try
            {
                Expression e = new Expression(DisplayContent);
                if (!e.HasErrors())
                {
                    DisplayContent = e.Evaluate().ToString();
                }
            }
            catch (EvaluationException e)
            {
                DisplayContent = "Error: " + e.Message;
            }
            
        }
        /// <summary>
        /// Updates DisplayContent with _operandString
        /// </summary>
        /// <param name="obj"></param>
        private void UpdateOperandString(object obj)
        {
            if (obj.ToString() != "CE" && _operationString != "")
            {
                _operandString += obj.ToString();

            }
            if(obj.ToString() != "CE")
            {
                _operandString = obj.ToString();
            }
            if (obj.ToString() == "CE")
            {
                _operandString = "";
            }
            DisplayContent += _operandString;
        }
        /// <summary>
        /// Returns either a binanry or unary Operation
        /// </summary>
        /// <param name="operationString"></param>
        /// <returns></returns>
        private Operation CurrentOperator(string operationString)
        {
            if (BinaryOperations.ContainsKey(operationString))
            {
                return BinaryOperations[operationString];
            }
            else if (UnaryOperations.ContainsKey(operationString))
            {
                return UnaryOperations[operationString];
            }

            return Operation.None;
        }
       
        /// <summary>
        /// Displays results in DisplayContent
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private double ProcessUnaryOperation(Operation operation)
        {
            switch (operation)
            {
                case Operation.Sin:
                    return Math.Sin(_operand1);
                case Operation.Cos:
                    return Math.Cos(_operand1);
                case Operation.Tan:
                    return Math.Tan(_operand1);
                case Operation.Square:
                    return Math.Pow(_operand1, 2);
                case Operation.SquareRoot:
                    return Math.Sqrt(_operand1);
                default:
                    DisplayContent = "Unknown Operation Encountered";
                    return 0;
            }
        }
        
       
    }
}
