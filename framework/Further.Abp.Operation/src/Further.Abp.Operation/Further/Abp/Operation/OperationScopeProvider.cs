using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace Further.Abp.Operation
{
    public class OperationScopeProvider : IOperationScopeProvider, ISingletonDependency
    {
        private readonly IAmbientOperationScope ambientOperationInfo;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public IOperationScope? CurrentScope => ambientOperationInfo.GetCurrentOperationScope();

        public OperationInfo? Current => ambientOperationInfo.GetOperationInfo();

        public OperationScopeProvider(
            IAmbientOperationScope ambientOperationInfo,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.ambientOperationInfo = ambientOperationInfo;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public virtual IOperationScope Begin(OperationScopeOptions? options = null, OperationInfoInitializeValue? value = null, bool requiresNew = false)
        {
            var currentOperationScope = CurrentScope;

            if (currentOperationScope != null && !requiresNew)
            {
                return new ChildOperationScope(currentOperationScope, value);
            }

            var operationScope = CreateNewOperationScope();
            operationScope.Initialize(options, value);

            return operationScope;
        }

        public virtual IOperationScope Reserve(string reservationName, bool requiresNew = false)
        {
            Check.NotNull(reservationName, nameof(reservationName));

            if (!requiresNew &&
                ambientOperationInfo.OperationScope != null &&
                ambientOperationInfo.OperationScope.IsReservedFor(reservationName))
            {
                return new ChildOperationScope(ambientOperationInfo.OperationScope);
            }

            var operationScope = CreateNewOperationScope();
            operationScope.Reserve(reservationName);

            return operationScope;
        }

        public virtual void BeginReserved(string reservationName, OperationScopeOptions? options = null, OperationInfoInitializeValue? value = null)
        {
            if (!TryBeginReserved(reservationName, options, value))
            {
                throw new AbpException($"Could not begin reserved operation for '{reservationName}'");
            }
        }

        public bool TryBeginReserved(string reservationName, OperationScopeOptions? options = null, OperationInfoInitializeValue? value = null)
        {
            Check.NotNull(reservationName, nameof(reservationName));

            var operationScope = ambientOperationInfo.OperationScope;

            while (operationScope != null && !operationScope.IsReservedFor(reservationName))
            {
                operationScope = operationScope.Outer;
            }

            if (operationScope == null)
            {
                return false;
            }

            operationScope.Initialize(options, value);

            return true;
        }

        private IOperationScope CreateNewOperationScope()
        {
            var scope = serviceScopeFactory.CreateScope();
            try
            {
                var outerOperationScope = ambientOperationInfo.OperationScope;

                var operationScope = scope.ServiceProvider.GetRequiredService<IOperationScope>();

                operationScope.SetOuter(outerOperationScope);

                ambientOperationInfo.SetOperationScope(operationScope);

                operationScope.Disposed += (sender, args) =>
                {
                    ambientOperationInfo.SetOperationScope(outerOperationScope);
                    scope.Dispose();
                };

                return operationScope;
            }
            catch
            {
                scope.Dispose();
                throw;
            }
        }
    }
}
