using System;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Runner : MonoBehaviour
{
    [SerializeField] private TMP_Text _stateField;
    [SerializeField] private TMP_InputField _timeoutField;
    [SerializeField] private Button _stateChangeButton;
    [SerializeField] private TMP_Text _stateChangeLabel;

    private StringBuilder _buffer;
    private CancellationTokenSource _token;
    private float _startTime;
    private TimeSpan _timeout;

    private void Awake()
    {
        _buffer = new StringBuilder();
        _stateChangeButton.onClick.AddListener(OnStateChangeClick);
        _stateField.gameObject.SetActive(false);
    }

    private void OnStateChangeClick()
    {
        if (_token == null)
        {
            if (!double.TryParse(_timeoutField.text, out var timeout) || !double.IsNormal(timeout) || timeout < 0)
            {
                timeout = 0;
            }

            _token = new CancellationTokenSource(_timeout = TimeSpan.FromSeconds(timeout));
            _stateField.gameObject.SetActive(true);
            _timeoutField.gameObject.SetActive(false);
            _stateChangeLabel.SetText("Stop");
            _startTime = Time.realtimeSinceStartup;
            return;
        }
        
        _stateField.gameObject.SetActive(false);
        _timeoutField.gameObject.SetActive(true);
        _stateChangeLabel.SetText("Start");
        _token.Cancel();
        _token.Dispose();
        _token = null;
    }

    private void Update()
    {
        if (_token == null) return;

        var elapsedTime = TimeSpan.FromSeconds(Time.realtimeSinceStartup - _startTime);
        var remainingTime = _timeout - elapsedTime;

        _buffer.Length = 0;
        
        if (remainingTime > TimeSpan.Zero)
        {
            _buffer.Append(Math.Max(remainingTime.TotalSeconds, 0).ToString("#0.00"));
        }
        else
        {
            _buffer.Append("Is Cancelled: ");
            _buffer.Append(_token.IsCancellationRequested.ToString());
        }
        
        _stateField.SetText(_buffer);
    }
}
