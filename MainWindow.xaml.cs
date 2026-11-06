using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace calculatorX11;

public partial class MainWindow : Window
{
    private string currentInput = "0";
    private string storedValue = string.Empty;
    private string selectedOperator = string.Empty;
    private bool isNewInput = true;

    public MainWindow()
    {
        InitializeComponent();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        DisplayBox.Text = currentInput;
    }

    private void NumberButton_Click(object sender, RoutedEventArgs e)
    {
        if (currentInput == "Error")
            return;

        var button = (Button)sender;
        var digit = button.Tag?.ToString();

        if (string.IsNullOrEmpty(digit))
            return;

        if (isNewInput || currentInput == "0")
        {
            currentInput = digit;
        }
        else
        {
            currentInput += digit;
        }

        isNewInput = false;
        UpdateDisplay();
    }

    private void DecimalButton_Click(object sender, RoutedEventArgs e)
    {
        if (currentInput == "Error")
            return;

        if (isNewInput)
        {
            currentInput = "0.";
        }
        else if (!currentInput.Contains('.'))
        {
            currentInput += ".";
        }

        isNewInput = false;
        UpdateDisplay();
    }

    private void OperatorButton_Click(object sender, RoutedEventArgs e)
    {
        if (currentInput == "Error")
            return;

        var button = (Button)sender;
        var op = button.Tag?.ToString();

        if (string.IsNullOrEmpty(op))
            return;

        if (!string.IsNullOrEmpty(selectedOperator) && !isNewInput)
        {
            ApplyPendingOperation();
        }

        storedValue = currentInput;
        selectedOperator = op;
        isNewInput = true;
    }

    private void EqualsButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(selectedOperator) || currentInput == "Error")
            return;

        ApplyPendingOperation();
        selectedOperator = string.Empty;
        storedValue = string.Empty;
    }

    private void ApplyPendingOperation()
    {
        if (string.IsNullOrEmpty(selectedOperator) || string.IsNullOrEmpty(storedValue))
            return;

        if (!double.TryParse(storedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var left) ||
            !double.TryParse(currentInput, NumberStyles.Float, CultureInfo.InvariantCulture, out var right))
        {
            currentInput = "Error";
            UpdateDisplay();
            return;
        }

        double result;

        switch (selectedOperator)
        {
            case "+":
                result = left + right;
                break;
            case "-":
                result = left - right;
                break;
            case "*":
                result = left * right;
                break;
            case "/":
                if (Math.Abs(right) < double.Epsilon)
                {
                    currentInput = "Error";
                    UpdateDisplay();
                    return;
                }

                result = left / right;
                break;
            default:
                return;
        }

        currentInput = result.ToString("G15", CultureInfo.InvariantCulture);
        storedValue = currentInput;
        isNewInput = true;
        UpdateDisplay();
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        currentInput = "0";
        storedValue = string.Empty;
        selectedOperator = string.Empty;
        isNewInput = true;
        UpdateDisplay();
    }

    private void BackspaceButton_Click(object sender, RoutedEventArgs e)
    {
        if (currentInput == "Error")
        {
            ClearButton_Click(sender, e);
            return;
        }

        if (isNewInput)
        {
            currentInput = "0";
            isNewInput = false;
        }
        else if (currentInput.Length > 1)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
        }
        else
        {
            currentInput = "0";
        }

        UpdateDisplay();
    }

    private void ToggleSignButton_Click(object sender, RoutedEventArgs e)
    {
        if (currentInput == "Error")
            return;

        if (currentInput.StartsWith("-"))
        {
            currentInput = currentInput.Substring(1);
        }
        else if (currentInput != "0")
        {
            currentInput = "-" + currentInput;
        }

        UpdateDisplay();
    }

    private void PercentButton_Click(object sender, RoutedEventArgs e)
    {
        if (currentInput == "Error")
            return;

        if (double.TryParse(currentInput, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            currentInput = (value / 100).ToString("G15", CultureInfo.InvariantCulture);
            UpdateDisplay();
        }
    }

    private void UnaryFunctionButton_Click(object sender, RoutedEventArgs e)
    {
        if (currentInput == "Error")
            return;

        if (!double.TryParse(currentInput, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            currentInput = "Error";
            UpdateDisplay();
            return;
        }

        var button = (Button)sender;
        var function = button.Tag.ToString();

        double result = 0;

        switch (function)
        {
            case "sqrt":
                if (value < 0)
                {
                    currentInput = "Error";
                    UpdateDisplay();
                    return;
                }
                result = Math.Sqrt(value);
                break;
            case "sin":
                result = Math.Sin(value * Math.PI / 180.0);
                break;
            case "cos":
                result = Math.Cos(value * Math.PI / 180.0);
                break;
            case "tan":
                result = Math.Tan(value * Math.PI / 180.0);
                break;
            default:
                currentInput = "Error";
                UpdateDisplay();
                return;
        }

        currentInput = result.ToString("G15", CultureInfo.InvariantCulture);
        isNewInput = true;
        UpdateDisplay();
    }
}
