using FluentResults;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Further.Abp.Operation
{
    public class ChildOperationScope : IOperationScope
    {
        public Guid Id => parent.Id;

        public IOperationScope? Outer => parent.Outer;

        public bool IsReserved => parent.IsReserved;

        public bool IsDisposed => parent.IsDisposed;

        public bool IsCompleted => parent.IsCompleted;

        public string? ReservationName => parent.ReservationName;

        public OperationInfo? OperationInfo => parent.OperationInfo;

        public event EventHandler<OperationScopeEventArgs> Disposed = default!;

        private readonly IOperationScope parent;

        public ChildOperationScope([NotNull] IOperationScope parent, OperationInfoInitializeValue? value = null)
        {
            Check.NotNull(parent, nameof(parent));

            this.parent = parent;

            parent.Disposed += (sender, args) => { Disposed.InvokeSafely(sender!, args); };

            if (value != null && OperationInfo != null)
            {
                this.OperationInfo.OperationId = value?.OperationId;
                this.OperationInfo.OperationName = value?.OperationName;
            }
        }

        public Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }

        public void Reserve([NotNull] string reservationName)
        {
            parent.Reserve(reservationName);
        }

        public void SetOuter(IOperationScope? outer)
        {
            parent.SetOuter(outer);
        }

        public void Initialize(OperationScopeOptions? options = null, OperationInfoInitializeValue? value = null)
        {
            parent.Initialize(options,value);
        }
    }
}
