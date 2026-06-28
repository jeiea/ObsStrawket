using System;

namespace ObsStrawket {

  /// <summary>
  /// Controls automatic reconnect attempts after an established OBS websocket connection is lost.
  /// </summary>
#pragma warning disable IDE0032
  public sealed class ObsReconnectOptions {
    private TimeSpan _initialDelay = TimeSpan.FromSeconds(1);
    private TimeSpan _maxDelay = TimeSpan.FromSeconds(30);
    private double _backoffMultiplier = 2;
    private int? _maxAttempts;

    /// <summary>
    /// Whether the client should reconnect after an unexpected connection loss.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Delay before the first reconnect attempt.
    /// </summary>
    public TimeSpan InitialDelay {
      get => _initialDelay;
      set {
        if (value < TimeSpan.Zero) {
          throw new ArgumentOutOfRangeException(nameof(value), "Reconnect delay cannot be negative.");
        }
        _initialDelay = value;
      }
    }

    /// <summary>
    /// Maximum delay between reconnect attempts.
    /// </summary>
    public TimeSpan MaxDelay {
      get => _maxDelay;
      set {
        if (value < TimeSpan.Zero) {
          throw new ArgumentOutOfRangeException(nameof(value), "Reconnect delay cannot be negative.");
        }
        _maxDelay = value;
      }
    }

    /// <summary>
    /// Multiplier applied to the previous reconnect delay after each failed attempt.
    /// </summary>
    public double BackoffMultiplier {
      get => _backoffMultiplier;
      set {
        if (double.IsNaN(value) || value < 1) {
          throw new ArgumentOutOfRangeException(
            nameof(value),
            "Reconnect backoff multiplier must be greater than or equal to 1.");
        }
        _backoffMultiplier = value;
      }
    }

    /// <summary>
    /// Maximum reconnect attempts. Set to <see langword="null"/> for unlimited attempts.
    /// </summary>
    public int? MaxAttempts {
      get => _maxAttempts;
      set {
        if (value < 0) {
          throw new ArgumentOutOfRangeException(nameof(value), "Reconnect attempts cannot be negative.");
        }
        _maxAttempts = value;
      }
    }
  }
#pragma warning restore IDE0032
}
