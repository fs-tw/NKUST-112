using FluentResults;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Json;

namespace Further.Abp.Operation
{
    public class OperationScope : IOperationScope, ITransientDependency
    {
        public const string OperationScopeReservationName = "_FurtherAbpOperationScope";

        public Guid Id { get; private set; }

        public IOperationScope? Outer { get; private set; }

        public bool IsReserved { get; set; }

        public bool IsDisposed { get; private set; }

        public bool IsCompleted { get; private set; }

        public string? ReservationName { get; set; }

        public OperationInfo? OperationInfo { get; set; }

        public event EventHandler<OperationScopeEventArgs> Disposed = default!;

        private Stopwatch stopwatch { get; set; }

        private readonly IServiceProvider serviceProvider;
        private readonly IGuidGenerator guidGenerator;
        private readonly ILogger<OperationScope> logger;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IOperationStore operationStore;

        private OperationScopeOptions options { get; set; }

        public OperationScope(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.guidGenerator = serviceProvider.GetRequiredService<IGuidGenerator>();
            this.logger = serviceProvider.GetRequiredService<ILogger<OperationScope>>();
            this.jsonSerializer = serviceProvider.GetRequiredService<IJsonSerializer>();
            this.operationStore = serviceProvider.GetRequiredService<IOperationStore>();
        }

        public virtual void Initialize(OperationScopeOptions? options = null, OperationInfoInitializeValue? value = null)
        {
            if (Id != Guid.Empty) throw new AbpException("不能對OperationScope重複初始化");

            stopwatch = Stopwatch.StartNew();

            //TODO 在Redis快取Lock處理完之前關閉該功能
            //var id = value?.Id;
            Guid? id = null;

            if (id == null)
            {
                Id = guidGenerator.Create();
                OperationInfo = new OperationInfo(Id);
            }

            if (id != null)
            {
                var operationInfo = operationStore.Get(id.Value);

                if (operationInfo == null)
                {
                    Id = guidGenerator.Create();
                    OperationInfo = new OperationInfo(Id);
                }

                if (operationInfo != null)
                {
                    Id = operationInfo.Id;
                    OperationInfo = operationInfo;
                }
            }

            if (OperationInfo != null)
            {
                OperationInfo.OperationId = value?.OperationId;
                OperationInfo.OperationName = value?.OperationName;
            }

            this.options = options ?? new OperationScopeOptions();

            IsReserved = false;

            if (this.options.EnabledLogger)
            {
                logger.LogInformation("OperationScope {Id} Initialize.", Id);
            }
        }

        public virtual async Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                stopwatch.Stop();

                if (OperationInfo != null)
                {
                    OperationInfo.ExecutionDuration += Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                }

                if (OperationInfo != null && OperationInfo.OperationId != null)
                {
                    await operationStore.SaveAsync(OperationInfo, options, cancellationToken);
                }

                IsCompleted = true;

                if (options.EnabledLogger && OperationInfo != null)
                {
                    var operationInfoString = jsonSerializer.Serialize(OperationInfo);
                    logger.LogInformation("OperationScope {Id} completed.\n {Operation}",
                        Id, operationInfoString);
                }
            }
            catch (Exception ex)
            {
                if (options.EnabledLogger)
                {
                    logger.LogError(ex, "OperationScope {Id} complete error.", Id);
                }
                throw;
            }
        }

        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            Disposed.Invoke(this, new OperationScopeEventArgs(this));

            if (this.options.EnabledLogger)
            {
                logger.LogInformation("OperationScope {Id} Dispose.", Id);
            }
        }

        public virtual void Reserve([NotNull] string reservationName)
        {
            Check.NotNullOrWhiteSpace(reservationName, nameof(reservationName));

            ReservationName = reservationName;
            IsReserved = true;
        }

        public virtual void SetOuter(IOperationScope? outer)
        {
            Outer = outer;
        }
    }
}
