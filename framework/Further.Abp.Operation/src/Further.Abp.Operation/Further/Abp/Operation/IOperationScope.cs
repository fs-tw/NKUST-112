using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Volo.Abp.Uow;
using JetBrains.Annotations;
using FluentResults;

namespace Further.Abp.Operation
{
    public interface IOperationScope : IDisposable
    {
        Guid Id { get; }

        event EventHandler<OperationScopeEventArgs> Disposed;

        IOperationScope? Outer { get; }

        bool IsReserved { get; }

        bool IsDisposed { get; }

        bool IsCompleted { get; }

        string? ReservationName { get; }

        OperationInfo? OperationInfo { get; }

        void Initialize(OperationScopeOptions? options = null, OperationInfoInitializeValue? value = null);

        void Reserve([NotNull] string reservationName);

        void SetOuter(IOperationScope? outer);

        Task CompleteAsync(CancellationToken cancellationToken = default);
    }
}
