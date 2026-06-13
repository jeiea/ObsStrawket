using System;

namespace ObsStrawket.Diagnostics {

  /// <summary>
  /// Verbose progress trace (<see cref="PipelineLevel.Debug"/> or <see cref="PipelineLevel.Info"/>).
  /// The former logging scope (method name) is intentionally dropped; only the detail remains.
  /// </summary>
  public sealed class PipelineTrace : PipelineEvent {
    /// <summary>Creates a trace event.</summary>
    /// <param name="level">Severity, either <see cref="PipelineLevel.Debug"/> or <see cref="PipelineLevel.Info"/>.</param>
    /// <param name="message">Detail text.</param>
    /// <param name="exception">Optional associated exception.</param>
    public PipelineTrace(PipelineLevel level, string message, Exception? exception = null) {
      Level = level;
      Message = message;
      Exception = exception;
    }

    /// <inheritdoc/>
    public override PipelineLevel Level { get; }

    /// <inheritdoc/>
    public override string Message { get; }

    /// <inheritdoc/>
    public override Exception? Exception { get; }
  }
}
